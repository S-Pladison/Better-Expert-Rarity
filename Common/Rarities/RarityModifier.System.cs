using BetterExpertRarity.Utils.DataStructures;
using BetterExpertRarity.Utils.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BetterExpertRarity.Common.Rarities
{
    public class RarityModifierSystem : ModSystem
    {
        // [public properties and fields]

        public readonly IReadOnlyList<RarityModifier> Modifiers;

        // [private properties and fields]

        private readonly List<RarityModifier> modifierInstances;

        private RenderTarget2D lineRenderTarget;
        private string text;

        // [constructors]

        public RarityModifierSystem()
        {
            Modifiers = (modifierInstances = new()).AsReadOnly();
        }

        // [public methods]

        public override void OnModLoad()
        {
            Main.OnResolutionChanged += RecreateTextRenderTarget;

            On_Main.DrawPendingMouseText += (orig) =>
            {
                ref var item = ref Main.HoverItem;

                if (item is not null && item.type > ItemID.None)
                {
                    var sb = Main.spriteBatch;
                    var rarity = item.rare;

                    if (item.expert)
                        item.rare = ItemRarityID.Expert;

                    if (item.master)
                        item.rare = ItemRarityID.Master;

                    sb.End();
                    RedrawTooltipRenderedLine();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, null, null, null, Main.UIScaleMatrix);

                    item.rare = rarity;
                }

                orig();
            };

            IL_Main.MouseTextInner += (il) =>
            {
                var c = new ILCursor(il);

                var xIndex = -1;
                var yIndex = -1;

                if (!c.TryGotoNext(MoveType.Before,
                    i => i.MatchLdsfld(typeof(Main).GetField("spriteBatch")),
                    i => i.MatchLdsfld(typeof(FontAssets).GetField("MouseText")),
                    i => i.MatchCallvirt(typeof(Asset<DynamicSpriteFont>).GetMethod("get_Value")),
                    i => i.MatchLdloc(0),
                    i => i.MatchLdloc(out xIndex),
                    i => i.MatchConvR4(),
                    i => i.MatchLdloc(out yIndex))) return;

                c.Index += 1;

                var label = c.DefineLabel();

                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, 0);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, 1);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, xIndex);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, yIndex);

                c.EmitDelegate((string cursorText, int rarity, int x, int y) =>
                {
                    var modifier = Modifiers.FirstOrDefault(x => x.AppliesToRarity(rarity), null);

                    if (modifier is null) return true;

                    Main.spriteBatch.End(out SpriteBatchData spriteBatchInfo);
                    RenderLine(cursorText);
                    Main.spriteBatch.Begin(spriteBatchInfo);

                    if (!TryGetRenderedLineTexture(cursorText, out var lineTexture)) return true;

                    return modifier?.PreDrawInWorld(cursorText, new Vector2(x, y), lineTexture) ?? true;
                });

                c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label);
                c.Emit(Mono.Cecil.Cil.OpCodes.Pop);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ret);
                c.MarkLabel(label);
            };
        }

        public override void OnModUnload()
        {
            Main.OnResolutionChanged -= RecreateTextRenderTarget;
        }

        public void AddModifier(RarityModifier modifier)
        {
            if (modifierInstances.Contains(modifier)) return;

            modifierInstances.Add(modifier);
        }

        public bool TryGetRenderedLineTexture(string text, out Texture2D texture)
        {
            if (this.text == text)
            {
                texture = lineRenderTarget;
                return true;
            }

            texture = null;
            return false;
        }

        // [private methods]

        private void RecreateTextRenderTarget(Vector2 size)
            => RecreateTextRenderTarget((int)size.X, (int)size.Y);

        private RenderTarget2D RecreateTextRenderTarget(int width, int height)
        {
            lineRenderTarget = new(Main.graphics.GraphicsDevice, width, Math.Min(200, height), false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            text = string.Empty;

            return lineRenderTarget;
        }

        private void RenderLine(string value)
        {
            // I don't know how to do this without RenderTargetUsage.PreserveContents :(

            value ??= "";

            if (text?.Equals(value) ?? false) return;

            var device = Main.graphics.GraphicsDevice;
            var sb = Main.spriteBatch;
            var renderTargetUsage = device.PresentationParameters.RenderTargetUsage;

            lineRenderTarget ??= RecreateTextRenderTarget(Main.ScreenSize.X, Main.ScreenSize.Y);

            device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            device.SetRenderTarget(lineRenderTarget);
            device.Clear(Color.Transparent);

            sb.Begin();
            ChatManager.DrawColorCodedString(sb, FontAssets.MouseText.Value, value.ToSnippets(), Vector2.Zero, Color.White, 0f, Vector2.Zero, Vector2.One, out _, -1f, false);
            sb.End();

            device.SetRenderTargets(null);
            device.PresentationParameters.RenderTargetUsage = renderTargetUsage;

            text = value;
        }

        private void RedrawTooltipRenderedLine()
        {
            var modifier = Modifiers.FirstOrDefault(x => x.AppliesToRarity(Main.HoverItem.rare), null);

            if (modifier is null) return;

            var var1 = 1;
            var var2 = new string[] { Main.HoverItem.HoverName };
            var var3 = new bool[1];
            var var4 = new bool[1];
            var var5 = -1;

            var tooltips = ItemLoader.ModifyTooltips(Main.HoverItem, ref var1, new string[] { "ItemName" }, ref var2, ref var3, ref var4, ref var5, out _, 0);
            var nameLine = tooltips.Find(i => i.Mod == "Terraria" && i.Name == "ItemName");

            if (nameLine == null) return;

            RenderLine(nameLine.Text);
        }
    }
}
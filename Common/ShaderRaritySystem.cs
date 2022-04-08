using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BetterExpertRarity.Common
{
    public class ShaderRaritySystem : ModSystem
    {
        // На все редкости используется одна цель рендеринга
        public RenderedText2D RenderedText { get; private set; }

        public UpdatableEffect ExpertEffect { get; private set; }
        public UpdatableEffect MasterEffect { get; private set; }

        public override void Load()
        {
            RenderedText = new();

            // Чтоб отбросить лишние проблемы, просто обновляем цель рендеринга при изменении окна игры
            On.Terraria.Main.SetDisplayMode += (orig, width, height, fullscreen) =>
            {
                var screen = new Point(Main.screenWidth, Main.screenHeight);

                orig(width, height, fullscreen);

                if (Main.screenWidth == screen.X && Main.screenHeight == screen.Y)
                {
                    return;
                }

                RenderedText.RecreateTarget(Main.screenWidth, Main.screenHeight);
            };

            // Обновляем цель рендеринга при наведении на предмет
            On.Terraria.Main.DrawPendingMouseText += (orig) =>
            {
                var sB = Main.spriteBatch;

                sB.End();
                UpdateRenderedText();
                sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, null, null, null, Main.UIScaleMatrix);

                orig();
            };

            // Игнорируем ванильную отрисовку и рисуем сами
            IL.Terraria.Main.MouseTextInner += (il) =>
            {
                ILCursor c = new(il);

                var mouseTextType = BetterExpertRarity.MouseTextCacheType;
                if (mouseTextType == null) return;

                var cursorTextInfo = BetterExpertRarity.MouseCursorTextInfo;
                var rareInfo = BetterExpertRarity.MouseRareInfo;
                if (cursorTextInfo == null || rareInfo == null) return;

                int cursorTextIndex = -1;
                if (!c.TryGotoNext(MoveType.Before,
                    i => i.MatchLdarg(1),
                    i => i.MatchLdfld(cursorTextInfo),
                    i => i.MatchStloc(out cursorTextIndex))) return;

                int rareIndex = -1;
                if (!c.TryGotoNext(MoveType.Before,
                    i => i.MatchLdarg(1),
                    i => i.MatchLdfld(rareInfo),
                    i => i.MatchStloc(out rareIndex))) return;

                int xIndex = -1;
                if (!c.TryGotoNext(MoveType.Before,
                    i => i.MatchLdsfld<Main>("mouseX"),
                    i => i.MatchLdcI4(14),
                    i => i.MatchAdd(),
                    i => i.MatchStloc(out xIndex))) return;

                int yIndex = -1;
                if (!c.TryGotoNext(MoveType.Before,
                    i => i.MatchLdsfld<Main>("mouseY"),
                    i => i.MatchLdcI4(14),
                    i => i.MatchAdd(),
                    i => i.MatchStloc(out yIndex))) return;

                try
                {
                    c.Goto(c.Body.Instructions.Last(), MoveType.Before);

                    if (!c.TryGotoPrev(MoveType.After,
                        i => i.MatchLdsfld<Main>("HoverItem"),
                        i => i.MatchLdfld<Item>("expert"),
                        i => i.MatchBrtrue(out _),
                        i => i.MatchLdloc(rareIndex),
                        i => i.MatchLdcI4(-12),
                        i => i.MatchBneUn(out _))) return;

                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, cursorTextIndex);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, rareIndex);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, xIndex);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, yIndex);
                    c.EmitDelegate<Action<string, int, int, int>>((text, rare, x, y) =>
                    {
                        ModContent.GetInstance<ShaderRaritySystem>().DrawExpertLine(Main.spriteBatch, new Vector2(x, y));
                    });

                    var label = c.DefineLabel();

                    // Оставил на всякий случай
                    c.EmitDelegate(() => false);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ret);
                    c.MarkLabel(label);
                }
                catch (Exception)
                {
                    ModLoader.GetMod(nameof(BetterExpertRarity))?.Logger.Error("Do you want a cake?");
                }

                try
                {
                    c.Goto(c.Body.Instructions.Last(), MoveType.Before);

                    if (!c.TryGotoPrev(MoveType.After,
                        i => i.MatchLdsfld<Main>("HoverItem"),
                        i => i.MatchLdfld<Item>("master"),
                        i => i.MatchBrtrue(out _),
                        i => i.MatchLdloc(rareIndex),
                        i => i.MatchLdcI4(-13),
                        i => i.MatchBneUn(out _))) return;

                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, cursorTextIndex);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, rareIndex);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, xIndex);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, yIndex);
                    c.EmitDelegate<Action<string, int, int, int>>((text, rare, x, y) =>
                    {
                        ModContent.GetInstance<ShaderRaritySystem>().DrawMasterLine(Main.spriteBatch, new Vector2(x, y));
                    });

                    var label = c.DefineLabel();

                    // Оставил на всякий случай
                    c.EmitDelegate(() => false);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label);
                    c.Emit(Mono.Cecil.Cil.OpCodes.Ret);
                    c.MarkLabel(label);
                }
                catch (Exception)
                {
                    ModLoader.GetMod(nameof(BetterExpertRarity))?.Logger.Error("Do you want a cheese?");
                }
            };

            if (Main.dedServ) return;

            LoadEffects();
        }

        public void DrawExpertLine(SpriteBatch spriteBatch, Vector2 position) => DrawLine(spriteBatch, position, ExpertEffect);
        public void DrawMasterLine(SpriteBatch spriteBatch, Vector2 position) => DrawLine(spriteBatch, position, MasterEffect);

        private void DrawLine(SpriteBatch spriteBatch, Vector2 position, UpdatableEffect effect)
        {
            // Пульсация цвета
            var progress = Main.mouseTextColor / 255f;
            var color = new Color(progress, progress, progress, progress);

            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, RenderedText2D.TextToSnippets(RenderedText.Text), position, new Color(0, 0, 0, color.A), 0f, Vector2.Zero, Vector2.One, -1f, 2f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, effect.GetUpdatedEffect(), Main.UIScaleMatrix);
            spriteBatch.Draw((Texture2D)RenderedText, position, null, color);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        }

        public void LoadEffects()
        {
            var effect = ModContent.Request<Effect>("BetterExpertRarity/Assets/ExpertRarity", AssetRequestMode.ImmediateLoad);
            effect.Value.Parameters["texture1"].SetValue(ModContent.Request<Texture2D>("BetterExpertRarity/Assets/Expert", AssetRequestMode.ImmediateLoad).Value);

            ExpertEffect = new(effect, () =>
            {
                var effect = ExpertEffect.Effect.Value;

                effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
                effect.Parameters["scale"].SetValue(Main.screenWidth / 1920f * 3.5f);
            });

            effect = ModContent.Request<Effect>("BetterExpertRarity/Assets/MasterRarity", AssetRequestMode.ImmediateLoad);
            effect.Value.Parameters["texture1"].SetValue(ModContent.Request<Texture2D>("BetterExpertRarity/Assets/Master", AssetRequestMode.ImmediateLoad).Value);

            MasterEffect = new(effect, () =>
            {
                var effect = MasterEffect.Effect.Value;

                effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.75f);
                effect.Parameters["scale"].SetValue(Main.screenWidth / 1920f * 6f);
            });
        }

        // ...

        private static void UpdateRenderedText()
        {
            var system = ModContent.GetInstance<ShaderRaritySystem>();
            var item = Main.HoverItem;
            if (item != null && (item.rare == ItemRarityID.Expert || item.expert || item.rare == ItemRarityID.Master || item.master))
            {
                var var1 = 1;
                var var2 = new string[] { Main.HoverItem.HoverName };
                var var3 = new bool[1];
                var var4 = new bool[1];
                var var5 = -1;

                var tooltips = ItemLoader.ModifyTooltips(Main.HoverItem, ref var1, new string[] { "ItemName" }, ref var2, ref var3, ref var4, ref var5, out _);
                var nameLine = tooltips.Find(i => i.mod == "Terraria" && i.Name == "ItemName");
                if (nameLine == null) return;

                system.RenderedText.Render(nameLine.text);
            }

            var mouseTextCache = BetterExpertRarity.MouseTextCacheInfo?.GetValue(Main.instance);
            if (mouseTextCache == null) return;

            var cursorTextInfo = BetterExpertRarity.MouseCursorTextInfo;
            var rareInfo = BetterExpertRarity.MouseRareInfo;
            var isValid = BetterExpertRarity.MouseIsValidInfo;
            if (cursorTextInfo == null || rareInfo == null || isValid == null) return;

            var text = (string)cursorTextInfo.GetValue(mouseTextCache);
            var rare = (int)rareInfo.GetValue(mouseTextCache);
            var flag = (bool)isValid.GetValue(mouseTextCache);
            if (!flag || text == string.Empty || !(rare == ItemRarityID.Expert || rare == ItemRarityID.Master)) return;

            system.RenderedText.Render(text);
        }
    }
}

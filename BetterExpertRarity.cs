using BetterExpertRarity.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterExpertRarity
{
    public class BetterExpertRarity : Mod
    {
        public static readonly FieldInfo MouseTextCacheInfo = typeof(Main).GetField("_mouseTextCache", BindingFlags.NonPublic | BindingFlags.Instance);
        public static readonly Type MouseTextCacheType = MouseTextCacheInfo?.GetValue(Main.instance).GetType();

        public static readonly FieldInfo MouseCursorTextInfo = MouseTextCacheType?.GetField("cursorText", BindingFlags.Public | BindingFlags.Instance);
        public static readonly FieldInfo MouseRareInfo = MouseTextCacheType?.GetField("rare", BindingFlags.Public | BindingFlags.Instance);
        public static readonly FieldInfo MouseIsValidInfo = MouseTextCacheType?.GetField("isValid", BindingFlags.Public | BindingFlags.Instance);

        // ...

        public override void Load()
        {
            On.Terraria.Main.DrawPendingMouseText += On_Main_DrawPendingMouseText;
            On.Terraria.Main.SetDisplayMode += On_Main_SetDisplayMode;
            On.Terraria.Item.Prefix += On_Item_Prefix;
            IL.Terraria.Main.MouseTextInner += IL_Main_MouseTextInner;
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawPendingMouseText -= On_Main_DrawPendingMouseText;
            On.Terraria.Main.SetDisplayMode -= On_Main_SetDisplayMode;
            On.Terraria.Item.Prefix -= On_Item_Prefix;
            IL.Terraria.Main.MouseTextInner -= IL_Main_MouseTextInner;
        }

        public override void PostSetupContent()
        {
            ShaderRarity.ClearAllText();
        }

        private static void On_Main_DrawPendingMouseText(On.Terraria.Main.orig_DrawPendingMouseText orig)
        {
            UpdateMouseText();

            var sB = Main.spriteBatch;

            sB.End();
            ShaderRarity.DrawTargets(sB.GraphicsDevice, sB);
            sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, null, null, null, Main.UIScaleMatrix);

            orig();
        }

        private static void UpdateMouseText()
        {
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

                if (ShaderRarity.TryFindRightRarity(item, out ShaderRarity shaderRarity))
                {
                    shaderRarity.Text = nameLine.text;
                    return;
                }
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
            if (!flag || text == string.Empty) return;

            if (ShaderRarity.TryFindRightRarity(rare, out ShaderRarity sr))
            {
                sr.Text = text;
                return;
            }
        }

        private static void On_Main_SetDisplayMode(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            var screen = new Point(Main.screenWidth, Main.screenHeight);

            orig(width, height, fullscreen);

            if (Main.screenWidth == screen.X && Main.screenHeight == screen.Y)
            {
                return;
            }

            ShaderRarity.RecreateRenderTargets(Main.screenWidth, Main.screenHeight);
        }

        private static bool On_Item_Prefix(On.Terraria.Item.orig_Prefix orig, Item item, int pre)
        {
            bool isExpertItem = item.rare == ItemRarityID.Expert;
            bool isMasterItem = item.rare == ItemRarityID.Master;
            bool flag = orig(item, pre);

            if (flag)
            {
                if (isExpertItem) item.rare = ItemRarityID.Expert;
                else if (isMasterItem) item.rare = ItemRarityID.Master;
            }

            return flag;
        }

        private static void IL_Main_MouseTextInner(ILContext il)
        {
            ILCursor c = new(il);

            var type = MouseTextCacheType;
            if (MouseTextCacheType == null) return;

            var cursorTextInfo = MouseCursorTextInfo;
            var rareInfo = MouseRareInfo;
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
                    ModContent.GetInstance<ExpertRarity>().DrawLine(Main.spriteBatch, new Vector2(x, y));
                });

                var label = c.DefineLabel();

                c.EmitDelegate(() => ModContent.GetInstance<ExpertRarity>().Text != string.Empty);
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
                    ModContent.GetInstance<MasterRarity>().DrawLine(Main.spriteBatch, new Vector2(x, y));
                });

                var label = c.DefineLabel();

                c.EmitDelegate(() => ModContent.GetInstance<MasterRarity>().Text != string.Empty);
                c.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, label);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ret);
                c.MarkLabel(label);
            }
            catch (Exception)
            {
                ModLoader.GetMod(nameof(BetterExpertRarity))?.Logger.Error("Do you want a cheese?");
            }
        }
    }
}
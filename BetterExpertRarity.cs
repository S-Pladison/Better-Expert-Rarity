using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BetterExpertRarity
{
    public class BetterExpertRarity : Mod
    {
        public override void Load()
        {
            On.Terraria.Main.CheckMonoliths += On_Main_CheckMonoliths;
            On.Terraria.Main.SetDisplayMode += On_Main_SetDisplayMode;
        }

        public override void Unload()
        {
            On.Terraria.Main.CheckMonoliths -= On_Main_CheckMonoliths;
            On.Terraria.Main.SetDisplayMode -= On_Main_SetDisplayMode;
        }

        private static void On_Main_CheckMonoliths(On.Terraria.Main.orig_CheckMonoliths orig)
        {
            var srs = ShaderRaritySystem.Instance;

            if (!Main.gameMenu && !Main.dedServ)
            {
                var device = Main.graphics.GraphicsDevice;
                var spriteBatch = Main.spriteBatch;

                srs.DrawTarget(device, spriteBatch);
            }

            orig();
        }

        private static void On_Main_SetDisplayMode(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            var screen = new Point(Main.screenWidth, Main.screenHeight);

            orig(width, height, fullscreen);

            if (Main.screenWidth == screen.X && Main.screenHeight == screen.Y)
            {
                return;
            }

            ShaderRaritySystem.Instance.RecreateRenderTarget(Main.screenWidth, Main.screenHeight);
        }
    }
}
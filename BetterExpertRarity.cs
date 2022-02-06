using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BetterExpertRarity
{
    public class BetterExpertRarity : Mod
    {
        public override void Load()
        {
            On.Terraria.Main.DrawPendingMouseText += On_Main_DrawPendingMouseText;
            On.Terraria.Main.SetDisplayMode += On_Main_SetDisplayMode;
        }

        public override void Unload()
        {
            On.Terraria.Main.DrawPendingMouseText -= On_Main_DrawPendingMouseText;
            On.Terraria.Main.SetDisplayMode -= On_Main_SetDisplayMode;
        }

        private static void On_Main_DrawPendingMouseText(On.Terraria.Main.orig_DrawPendingMouseText orig)
        {
            var srs = ShaderRaritySystem.Instance;
            var device = Main.graphics.GraphicsDevice;
            var spriteBatch = Main.spriteBatch;

            srs.Update();
            spriteBatch.End();

            // :( Sorry... I don't know how anymore...
            var renderTargetUsage = device.PresentationParameters.RenderTargetUsage;
            device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            srs.DrawTarget(device, spriteBatch);
            device.PresentationParameters.RenderTargetUsage = renderTargetUsage;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, null, null, null, Main.UIScaleMatrix);
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
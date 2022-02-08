using BetterExpertRarity.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BetterExpertRarity
{
    public class RainbowModName : ILoadable
    {
        private delegate void orig_DrawSelf(object self, SpriteBatch sB);
        private delegate void hook_DrawSelf(orig_DrawSelf orig, object self, SpriteBatch sB);

        private static readonly Type UIModItemType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UIModItem");
        private static readonly MethodInfo ModItemDrawSelfMethod = UIModItemType?.GetMethod("DrawSelf", BindingFlags.NonPublic | BindingFlags.Instance);

        // ...

        void ILoadable.Load(Mod mod)
        {
            if (ModItemDrawSelfMethod != null)
            {
                HookEndpointManager.Add<hook_DrawSelf>(ModItemDrawSelfMethod, On_UIModItem_DrawSelf);
            }
        }

        void ILoadable.Unload()
        {
            if (ModItemDrawSelfMethod != null)
            {
                HookEndpointManager.Remove<hook_DrawSelf>(ModItemDrawSelfMethod, On_UIModItem_DrawSelf);
            }
        }

        // ...

        private static void On_UIModItem_DrawSelf(orig_DrawSelf orig, object self, SpriteBatch sB)
        {
            orig(self, sB);

            var expertRarity = ModContent.GetInstance<ExpertRarity>();
            expertRarity.Text = string.Empty;

            var type = UIModItemType;
            if (type == null) return;

            var modNameInfo = type.GetField("_modName", BindingFlags.NonPublic | BindingFlags.Instance);
            if (modNameInfo == null) return;

            var modName = ModLoader.GetMod(nameof(BetterExpertRarity)).DisplayName;
            if (modNameInfo.GetValue(self) is UIText uiText && uiText.Text.Contains(modName))
            {
                uiText.TextColor = new Color(0, 0, 0, 0);

                var rasterizerState = sB.GraphicsDevice.RasterizerState;
                var scissorRectangle = sB.GraphicsDevice.ScissorRectangle;
                var anisotropicClamp = SamplerState.AnisotropicClamp;
                var position = uiText.GetDimensions().Position() + new Vector2(0, -2);

                sB.End();

                expertRarity.Text = modName;
                expertRarity.DrawTarget(sB.GraphicsDevice, sB);
                expertRarity.Text = string.Empty;

                sB.GraphicsDevice.ScissorRectangle = scissorRectangle;
                sB.GraphicsDevice.RasterizerState = rasterizerState;

                sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
                {
                    Utils.DrawBorderString(sB, uiText.Text, position, Color.White, 1f, 0f, 0f, -1);
                    ChatManager.DrawColorCodedStringShadow(sB, FontAssets.MouseText.Value, ShaderRarity.TextToSnippets(modName), position, Color.Black, 0f, Vector2.Zero, Vector2.One, -1f, 1f);
                }
                sB.End();

                sB.GraphicsDevice.ScissorRectangle = scissorRectangle;
                sB.GraphicsDevice.RasterizerState = rasterizerState;

                sB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, expertRarity.GetUpdatedEffect(), Main.UIScaleMatrix);
                {
                    sB.Draw((Texture2D)expertRarity.Target, position, null, Color.White);
                }
                sB.End();

                sB.GraphicsDevice.ScissorRectangle = scissorRectangle;
                sB.GraphicsDevice.RasterizerState = rasterizerState;

                sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
            }
        }
    }
}

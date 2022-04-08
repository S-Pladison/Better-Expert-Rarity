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

namespace BetterExpertRarity.Common
{
    public class RainbowModName : ILoadable
    {
        private UIText uiText;
        private Mod mod;

        private delegate void orig_DrawSelf(object self, SpriteBatch sB);
        private delegate void hook_DrawSelf(orig_DrawSelf orig, object self, SpriteBatch sB);

        private static readonly Type UIModItemType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UIModItem");
        private static readonly MethodInfo UIModItemOnInitializeMethod = UIModItemType?.GetMethod("OnInitialize", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo UIModItemDrawMethod = UIModItemType?.GetMethod("Draw", BindingFlags.Public | BindingFlags.Instance);

        private delegate void orig_OnInitialize(object self);
        private delegate void hook_OnInitialize(orig_OnInitialize orig, object self);
        private delegate void orig_Draw(object self, SpriteBatch sb);
        private delegate void hook_Draw(orig_Draw orig, object self, SpriteBatch sb);

        // ...

        void ILoadable.Load(Mod mod)
        {
            this.mod = mod;

            if (UIModItemOnInitializeMethod == null || UIModItemDrawMethod == null) return;

            HookEndpointManager.Add<hook_OnInitialize>(UIModItemOnInitializeMethod, ModifyOnInitialize);
            HookEndpointManager.Add<hook_Draw>(UIModItemDrawMethod, ModifyDrawMethod);
        }

        void ILoadable.Unload()
        {
            if (UIModItemOnInitializeMethod == null || UIModItemDrawMethod == null) return;

            HookEndpointManager.Remove<hook_OnInitialize>(UIModItemOnInitializeMethod, ModifyOnInitialize);
            HookEndpointManager.Remove<hook_Draw>(UIModItemDrawMethod, ModifyDrawMethod);
        }

        // ...

        private static void ModifyOnInitialize(orig_OnInitialize orig, object self)
        {
            orig(self);

            var type = UIModItemType;
            if (type == null) return;

            var modNameInfo = type.GetField("_modName", BindingFlags.NonPublic | BindingFlags.Instance);
            if (modNameInfo == null) return;

            var instance = ModContent.GetInstance<RainbowModName>();
            if (modNameInfo.GetValue(self) is UIText uiText && uiText.Text.Contains(instance.mod.DisplayName))
            {
                instance.uiText = uiText;
            }
        }

        private static void ModifyDrawMethod(orig_DrawSelf orig, object self, SpriteBatch sB)
        {
            orig(self, sB);

            var instance = ModContent.GetInstance<RainbowModName>();
            var uiText = instance.uiText;
            if (uiText == null) return;

            var modName = instance.mod.DisplayName;
            var rasterizerState = sB.GraphicsDevice.RasterizerState;
            var scissorRectangle = sB.GraphicsDevice.ScissorRectangle;
            var position = uiText.GetDimensions().Position() + new Vector2(0, -2);
            var system = ModContent.GetInstance<ShaderRaritySystem>();

            sB.End();
            {
                system.RenderedText.Render(modName);
            }
            sB.GraphicsDevice.ScissorRectangle = scissorRectangle;
            sB.GraphicsDevice.RasterizerState = rasterizerState;
            sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
            {
                ChatManager.DrawColorCodedStringShadow(sB, FontAssets.MouseText.Value, RenderedText2D.TextToSnippets(modName), position, Color.Black, 0f, Vector2.Zero, Vector2.One, -1f, 1f);
            }
            sB.End();
            sB.GraphicsDevice.ScissorRectangle = scissorRectangle;
            sB.GraphicsDevice.RasterizerState = rasterizerState;
            sB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, system.ExpertEffect.GetUpdatedEffect(), Main.UIScaleMatrix);
            {
                sB.Draw((Texture2D)system.RenderedText, position, null, Color.White);
            }
            sB.End();
            sB.GraphicsDevice.ScissorRectangle = scissorRectangle;
            sB.GraphicsDevice.RasterizerState = rasterizerState;
            sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BetterExpertRarity.Rarities
{
    public abstract class ShaderRarity : ILoadable
    {
        public Mod Mod { get; private set; }
        public Asset<Effect> Effect { get; private set; }
        public RenderTarget2D Target { get; private set; }
        public string Text { get; set; } = String.Empty;

        public ShaderRarity(Asset<Effect> effect)
        {
            Effect = effect;
        }

        // ...

        public virtual bool Condition(Item item) => false;
        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual Effect GetUpdatedEffect() => Effect.Value;

        // ...

        public void RecreateRenderTarget(int width, int height)
        {
            Target = new(Main.graphics.GraphicsDevice, width, height);
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 pos)
        {
            float progress = Main.mouseTextColor / 255f;
            Color color = new(progress, progress, progress, progress);

            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, ShaderRarity.TextToSnippets(Text), pos, new Color(0, 0, 0, color.A), 0f, Vector2.Zero, Vector2.One, -1f, 2f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, this.GetUpdatedEffect(), Main.UIScaleMatrix);
            spriteBatch.Draw((Texture2D)Target, pos, null, color);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

            Text = string.Empty;
        }

        public void DrawTarget(GraphicsDevice device, SpriteBatch spriteBatch, Action drawMethod = null)
        {
            if (Text == string.Empty) return;

            Target ??= new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            // :( Sorry... I don't know how anymore...
            var renderTargetUsage = device.PresentationParameters.RenderTargetUsage;

            device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            device.SetRenderTarget(Target);
            device.Clear(Color.Transparent);

            spriteBatch.Begin();
            if (drawMethod != null)
            {
                drawMethod.Invoke();
            }
            else
            {
                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, ShaderRarity.TextToSnippets(Text), Vector2.Zero, Color.White, 0f, Vector2.Zero, Vector2.One, out _, -1f, false);
            }
            spriteBatch.End();

            device.SetRenderTargets(null);
            device.PresentationParameters.RenderTargetUsage = renderTargetUsage;
        }

        void ILoadable.Load(Mod mod)
        {
            _rarities.Add(this);

            Mod = mod;
            this.Load();
        }

        void ILoadable.Unload()
        {
            this.Unload();

            _rarities.Remove(this);
        }

        // ...

        public static TextSnippet[] TextToSnippets(string text)
        {
            TextSnippet[] snippets = ChatManager.ParseMessage(text ?? " ", Color.White).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);
            return snippets;
        }

        public static void RecreateRenderTargets(int width, int height)
        {
            foreach (var elem in _rarities.ToArray())
            {
                elem.RecreateRenderTarget(width, height);
            }
        }

        public static void DrawTargets(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            foreach (var elem in _rarities)
            {
                elem.DrawTarget(device, spriteBatch);
            }
        }

        public static void ClearAllText()
        {
            foreach (var elem in _rarities)
            {
                elem.Text = string.Empty;
            }
        }

        public static bool TryFindRightRarity(Item item, out ShaderRarity shaderRarity)
        {
            foreach (var elem in _rarities)
            {
                if (elem.Condition(item))
                {
                    shaderRarity = elem;
                    return true;
                }
            }

            shaderRarity = null;
            return false;
        }

        public static bool TryFindRightRarity(int rare, out ShaderRarity shaderRarity)
        {
            Item item = new();
            item.rare = rare;

            return TryFindRightRarity(item, out shaderRarity);
        }

        private static readonly List<ShaderRarity> _rarities = new();
    }
}

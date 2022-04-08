using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace BetterExpertRarity.Common
{
    public class RenderedText2D
    {
        public string Text { get; private set; }
        public RenderTarget2D Target { get; private set; }
        private string OldText { get; set; }

        public RenderTarget2D RecreateTarget(int width, int height)
        {
            Target = new(Main.graphics.GraphicsDevice, width, height);
            Text = String.Empty;
            OldText = String.Empty;

            return Target;
        }

        public void Render(string text = null)
        {
            if (text != null)
            {
                Text = text;
            }

            if (OldText != string.Empty && OldText == Text)
            {
                return;
            }

            var device = Main.graphics.GraphicsDevice;
            var sb = Main.spriteBatch;
            var renderTargetUsage = device.PresentationParameters.RenderTargetUsage;

            Target ??= RecreateTarget(Main.screenWidth, Main.screenHeight);

            device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            device.SetRenderTarget(Target);
            device.Clear(Color.Transparent);

            sb.Begin();
            ChatManager.DrawColorCodedString(sb, FontAssets.MouseText.Value, TextToSnippets(Text), Vector2.Zero, Color.White, 0f, Vector2.Zero, Vector2.One, out _, -1f, false);
            sb.End();

            device.SetRenderTargets(null);
            device.PresentationParameters.RenderTargetUsage = renderTargetUsage;

            OldText = Text;
        }

        // ...

        public static TextSnippet[] TextToSnippets(string text)
        {
            TextSnippet[] snippets = ChatManager.ParseMessage(text ?? " ", Color.White).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);
            return snippets;
        }

        public static explicit operator Texture2D(RenderedText2D renderedText2D)
        {
            return renderedText2D.Target;
        }
    }
}

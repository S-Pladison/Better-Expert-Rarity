using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BetterExpertRarity
{
    public class ShaderRaritySystem : ModSystem
    {
        public static ShaderRaritySystem Instance => ModContent.GetInstance<ShaderRaritySystem>();

        // ...

        public RenderTarget2D Target { get; private set; }
        public Asset<Effect> Effect { get; private set; }
        public bool CanDraw { get => Text != String.Empty; }
        public string Text { get; set; }

        // ...

        public void RecreateRenderTarget(int width, int height)
        {
            Target = new(Main.graphics.GraphicsDevice, width, height);
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 pos)
        {
            float progress = Main.mouseTextColor / 255f;
            Color color = new(progress, progress, progress, progress);

            Effect effect = Effect.Value;
            effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["scale"].SetValue(Main.screenWidth / 1920f * 3.5f);

            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, this.TextToSnippets(), pos, new Color(0, 0, 0, color.A), 0f, Vector2.Zero, Vector2.One, -1f, 2f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, effect, Main.UIScaleMatrix);

            spriteBatch.Draw((Texture2D)Target, pos, null, color);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        }

        public void DrawTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (Text == string.Empty) return;

            Target ??= new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);

            device.SetRenderTarget(Target);
            device.Clear(Color.Transparent);

            spriteBatch.Begin();
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, this.TextToSnippets(), Vector2.Zero, Color.White, 0f, Vector2.Zero, Vector2.One, out _, -1f, false);
            spriteBatch.End();

            device.SetRenderTarget(null);
        }

        public TextSnippet[] TextToSnippets()
        {
            TextSnippet[] snippets = ChatManager.ParseMessage(Text ?? " ", Color.White).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);
            return snippets;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            Text = string.Empty;

            if (Main.HoverItem != null && (Main.HoverItem.rare == ItemRarityID.Expert || Main.HoverItem.expert))
            {
                var var1 = 1;
                var var2 = new string[] { Main.HoverItem.HoverName };
                var var3 = new bool[1];
                var var4 = new bool[1];
                var var5 = -1;

                var tooltips = ItemLoader.ModifyTooltips(Main.HoverItem, ref var1, new string[] { "ItemName" }, ref var2, ref var3, ref var4, ref var5, out _);
                var nameLine = tooltips.Find(i => i.mod == "Terraria" && i.Name == "ItemName");
                if (nameLine == null) return;

                Text = nameLine.text;
                return;
            }
        }

        public override void Load()
        {
            if (Main.dedServ) return;

            Effect = ModContent.Request<Effect>("BetterExpertRarity/expertRarity", AssetRequestMode.ImmediateLoad);
            Effect.Value.Parameters["texture1"].SetValue(ModContent.Request<Texture2D>("BetterExpertRarity/rainbow", AssetRequestMode.ImmediateLoad).Value);
        }
    }
}

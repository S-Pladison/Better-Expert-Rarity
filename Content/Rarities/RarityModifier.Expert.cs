using BetterExpertRarity.Common.Rarities;
using BetterExpertRarity.Utils.DataStructures;
using BetterExpertRarity.Utils.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BetterExpertRarity.Content.Rarities
{
    public class ExpertRarityModifier : RarityModifier
    {
        // [public properties and fields]

        public Asset<Effect> Effect { get; private set; }

        // [public methods]

        public override bool AppliesToRarity(int rarity)
            => rarity == ItemRarityID.Expert;

        public override void Load()
        {
            Effect = ModContent.Request<Effect>("BetterExpertRarity/Assets/ExpertRarity", AssetRequestMode.ImmediateLoad);
            Effect.Value.Parameters["texture1"].SetValue(ModContent.Request<Texture2D>("BetterExpertRarity/Assets/Expert", AssetRequestMode.ImmediateLoad).Value);
        }

        public override bool PreDrawInTooltips(DrawableTooltipLine line, Texture2D renderedLineTexture)
        {
            var spriteBatch = Main.spriteBatch;
            var position = new Vector2(line.X, line.Y);

            PrepareBeforeDraw(out float pulsation);

            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, line.Text.ToSnippets(), position, new Color(0, 0, 0, pulsation), line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

            spriteBatch.End(out SpriteBatchData spriteBatchInfo);
            spriteBatch.Begin(spriteBatchInfo, Effect.Value);

            var color = Colors.AlphaDarken(Color.White);
            color.A = 0;

            for (int i = 0; i < 5; i++)
            {
                var angle = i / 5.0f * MathHelper.TwoPi;
                var coords = position + angle.ToRotationVector2() * pulsation * 1.33f;
                spriteBatch.Draw(renderedLineTexture, coords, null, Color.Black, line.Rotation, line.Origin, line.BaseScale.Y, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < 5; i++)
            {
                var angle = i / 5.0f * MathHelper.TwoPi;
                var coords = position + angle.ToRotationVector2() * pulsation * 0.5f;
                spriteBatch.Draw(renderedLineTexture, coords, null, color * 0.3f, line.Rotation, line.Origin, line.BaseScale.Y, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < 5; i++)
            {
                var angle = i / 5.0f * MathHelper.TwoPi;
                var coords = position + (angle + Main.GlobalTimeWrappedHourly).ToRotationVector2() * pulsation * 3f;
                spriteBatch.Draw(renderedLineTexture, coords, null, color * 0.1f, line.Rotation, line.Origin, line.BaseScale.Y, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(spriteBatchInfo);

            return false;
        }

        public override bool PreDrawInWorld(string text, Vector2 position, Texture2D renderedLineTexture)
        {
            var spriteBatch = Main.spriteBatch;

            PrepareBeforeDraw(out float pulsation);

            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, text.ToSnippets(), position, new Color(0, 0, 0, pulsation), 0f, Vector2.Zero, Vector2.One, -1f, 2f);

            spriteBatch.End(out SpriteBatchData spriteBatchInfo);
            spriteBatch.Begin(spriteBatchInfo, Effect.Value);

            var color = Colors.AlphaDarken(Color.White);
            color.A = 0;

            for (int i = 0; i < 5; i++)
            {
                var angle = i / 5.0f * MathHelper.TwoPi;
                var coords = position + angle.ToRotationVector2() * pulsation * 1.33f;
                spriteBatch.Draw(renderedLineTexture, coords, null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < 5; i++)
            {
                var angle = i / 5.0f * MathHelper.TwoPi;
                var coords = position + angle.ToRotationVector2() * pulsation * 0.5f;
                spriteBatch.Draw(renderedLineTexture, coords, null, color * 0.3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < 5; i++)
            {
                var angle = i / 5.0f * MathHelper.TwoPi;
                var coords = position + (angle + Main.GlobalTimeWrappedHourly).ToRotationVector2() * pulsation * 3f;
                spriteBatch.Draw(renderedLineTexture, coords, null, color * 0.1f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(spriteBatchInfo);

            return false;
        }

        // [private methods]

        private void PrepareBeforeDraw(out float pulsation)
        {
            pulsation = Main.mouseTextColor / 255f;

            var effect = Effect.Value;
            effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.35f);
            effect.Parameters["scale"].SetValue(Main.screenWidth / 1920f * 5f);
        }
    }
}
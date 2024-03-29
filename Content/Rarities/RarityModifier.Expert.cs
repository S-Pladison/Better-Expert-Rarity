﻿using BetterExpertRarity.Common.Rarities;
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
        public override int RarityType { get => ItemRarityID.Expert; }

        // [public methods]

        public override void Load()
        {
            Effect = ModContent.Request<Effect>("BetterExpertRarity/Assets/ExpertRarity", AssetRequestMode.ImmediateLoad);
            Effect.Value.Parameters["texture1"].SetValue(ModContent.Request<Texture2D>("BetterExpertRarity/Assets/Expert", AssetRequestMode.ImmediateLoad).Value);
        }

        public override void Draw(DrawData data)
        {
            var spriteBatch = Main.spriteBatch;
            var pulsation = Main.mouseTextColor / 255f * (data.ShadowSpread / 2f);

            var effect = Effect.Value;
            effect.Parameters["screenResolution"].SetValue(Main.ScreenSize.ToVector2());
            effect.Parameters["textPosition"].SetValue(data.Position);
            effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.35f);
            effect.Parameters["repetitions"].SetValue(Main.screenWidth / (1920f * Main.UIScale) * 5f);

            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, data.Text.ToSnippets(), data.Position, Colors.AlphaDarken(Color.Black), data.Rotation, data.Origin, data.Scale, data.MaxWidth, data.ShadowSpread);

            spriteBatch.End(out SpriteBatchData spriteBatchData);
            spriteBatch.Begin(spriteBatchData with { Effect = Effect.Value });

            var color = Colors.AlphaDarken(Color.White) with { A = 0 };

            for (int i = 0; i < 5; i++)
            {
                var angle = i / 5.0f * MathHelper.TwoPi;
                var coords = data.Position + angle.ToRotationVector2() * pulsation * 1.33f;

                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, data.Text, coords, Color.Black, data.Rotation, data.Origin, data.Scale, data.MaxWidth);
            }

            for (int i = 0; i < 5; i++)
            {
                var angle = i / 5.0f * MathHelper.TwoPi;
                var coords = data.Position + angle.ToRotationVector2() * pulsation * 0.5f;

                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, data.Text, coords, color * 0.3f, data.Rotation, data.Origin, data.Scale, data.MaxWidth);
            }

            for (int i = 0; i < 5; i++)
            {
                var angle = i / 5.0f * MathHelper.TwoPi;
                var coords = data.Position + (angle + Main.GlobalTimeWrappedHourly).ToRotationVector2() * pulsation * 3f;

                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, data.Text, coords, color * 0.1f, data.Rotation, data.Origin, data.Scale, data.MaxWidth);
            }

            spriteBatch.End();
            spriteBatch.Begin(spriteBatchData);
        }
    }
}
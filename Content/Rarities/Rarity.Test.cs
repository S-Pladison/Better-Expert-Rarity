using BetterExpertRarity.Common.Rarities;
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
    [Autoload(false)]
    public class TestRarity : DrawableModRarity
    {
        // [...]

        [Autoload(false)]
        private class TestRarityGlobalItem : GlobalItem
        {
            public override bool AppliesToEntity(Item entity, bool lateInstantiation)
                => ItemID.AdamantiteBar == entity.type;

            public override void SetDefaults(Item entity)
                => entity.rare = ModContent.RarityType<TestRarity>();
        }

        // [public properties and fields]

        public Asset<Effect> Effect { get; private set; }

        // [public methods]

        public override void Draw(RarityModifier.DrawData data)
        {
            var spriteBatch = Main.spriteBatch;

            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, data.Text.ToSnippets(), data.Position, Terraria.ID.Colors.AlphaDarken(Color.Red), data.Rotation, data.Origin, data.Scale, data.MaxWidth, data.ShadowSpread);
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, data.Text, data.Position, Colors.AlphaDarken(Color.Black), data.Rotation, data.Origin, data.Scale, data.MaxWidth);
        }
    }
}
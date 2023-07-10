using BetterExpertRarity.Common.Rarities;
using BetterExpertRarity.Utils.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

#if DEBUG
namespace BetterExpertRarity.Content.Rarities
{
    public class TestRarity : DrawableModRarity
    {
        // [...]

        private class TestRarityGlobalItem : GlobalItem
        {
            public override bool AppliesToEntity(Item entity, bool lateInstantiation)
                => ItemID.AdamantiteBar == entity.type;

            public override void SetDefaults(Item entity)
                => entity.rare = ModContent.RarityType<TestRarity>();
        }

        // [public methods]

        public override void Draw(RarityModifier.DrawData data)
        {
            var spriteBatch = Main.spriteBatch;

            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, data.Text.ToSnippets(), data.Position, Terraria.ID.Colors.AlphaDarken(Color.Red), data.Rotation, data.Origin, data.Scale, data.MaxWidth, data.ShadowSpread);
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, data.Text, data.Position, Colors.AlphaDarken(Color.Black), data.Rotation, data.Origin, data.Scale, data.MaxWidth);
        }
    }
}
#endif
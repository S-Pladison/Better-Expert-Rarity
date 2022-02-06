using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterExpertRarity
{
    public class ExpertItems : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool _) => item.rare == ItemRarityID.Expert || item.expert;

        public override void SetDefaults(Item item)
        {
            item.rare = ItemRarityID.Expert;
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            return ShaderRaritySystem.Instance.CanDraw;
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod != "Terraria" || line.Name != "ItemName") return true;

            var srs = ShaderRaritySystem.Instance;
            if (!srs.CanDraw) return true;

            ShaderRaritySystem.Instance.DrawLine(Main.spriteBatch, new Vector2(line.X, line.Y));
            return false;
        }
    }
}

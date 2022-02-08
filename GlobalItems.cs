using BetterExpertRarity.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterExpertRarity
{
    public class GlobalItems : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool _)
        {
            bool flag = false;
            flag |= item.rare == ItemRarityID.Expert;
            flag |= item.expert;
            flag |= item.rare == ItemRarityID.Master;
            flag |= item.master;
            return flag;
        }

        public override void SetDefaults(Item item)
        {
            if (item.expert)
            {
                item.rare = ItemRarityID.Expert;
                return;
            }

            if (item.master)
            {
                item.rare = ItemRarityID.Master;
                return;
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod != "Terraria" || line.Name != "ItemName") return true;

            if (ShaderRarity.TryFindRightRarity(item, out ShaderRarity sr))
            {
                sr.DrawLine(Main.spriteBatch, new Vector2(line.X, line.Y));
                return false;
            }

            return true;
        }
    }
}

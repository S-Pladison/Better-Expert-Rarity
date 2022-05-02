using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterExpertRarity.Common
{
    public class ModGlobalItem : GlobalItem
    {
        public override void Load()
        {
            On.Terraria.Item.Prefix += (orig, item, pre) =>
            {
                bool isExpertItem = item.rare == ItemRarityID.Expert;
                bool isMasterItem = item.rare == ItemRarityID.Master;
                bool flag = orig(item, pre);

                if (flag)
                {
                    if (isExpertItem) item.rare = ItemRarityID.Expert;
                    else if (isMasterItem) item.rare = ItemRarityID.Master;
                }

                return flag;
            };

            On.Terraria.PopupText.Update += (orig, self, whoAmI) =>
            {
                orig(self, whoAmI);

                if (self.rarity == ItemRarityID.Expert)
                {
                    self.color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Main.mouseTextColor);
                }
                else if (self.rarity == ItemRarityID.Master || self.master)
                {
                    Color color = Color.Lerp(new Color(185, 139, 54), new Color(241, 196, 81), (MathF.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1) * 0.5f);
                    color.A = Main.mouseTextColor;

                    self.color = color;
                }
            };
        }
    }

    public class ExpertGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool _)
        {
            bool flag = false;
            flag |= item.rare == ItemRarityID.Expert;
            flag |= item.expert;
            return flag;
        }

        public override void SetDefaults(Item item)
        {
            item.rare = ItemRarityID.Expert;
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            return line.Mod != "Terraria" || line.Name != "ItemName";
        }

        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
            if (line.Mod != "Terraria" || line.Name != "ItemName") return;

            ModContent.GetInstance<ShaderRaritySystem>().DrawExpertLine(Main.spriteBatch, new Vector2(line.X, line.Y));
        }
    }

    public class MasterGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool _)
        {
            bool flag = false;
            flag |= item.rare == ItemRarityID.Master;
            flag |= item.master;
            return flag;
        }

        public override void SetDefaults(Item item)
        {
            item.rare = ItemRarityID.Master;
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            return line.Mod != "Terraria" || line.Name != "ItemName";
        }

        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
            if (line.Mod != "Terraria" || line.Name != "ItemName") return;

            ModContent.GetInstance<ShaderRaritySystem>().DrawMasterLine(Main.spriteBatch, new Vector2(line.X, line.Y));
        }
    }
}

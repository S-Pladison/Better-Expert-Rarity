using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace BetterExpertRarity.Common.Rarities
{
    public class RarityModifierGlobalItem : GlobalItem
    {
        public override void Load()
        {
            On_PopupText.Update += (orig, self, whoAmI) =>
            {
                orig(self, whoAmI);

                var system = ModContent.GetInstance<RarityModifierSystem>();
                var modifier = system.Modifiers.FirstOrDefault(x => x.AppliesToRarity(self.rarity), null);

                modifier?.ModifyPopupText(self);
            };
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod != "Terraria" || line.Name != "ItemName") return true;

            var system = ModContent.GetInstance<RarityModifierSystem>();

            if (!system.TryGetRenderedLineTexture(line.Text, out var lineTexture)) return true;

            var modifier = system.Modifiers.FirstOrDefault(x => x.AppliesToRarity(item.rare), null);

            return modifier?.PreDrawInTooltips(line, lineTexture) ?? true;
        }
    }
}

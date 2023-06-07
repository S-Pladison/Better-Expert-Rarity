using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterExpertRarity.Common.Rarities
{
    public class RarityModifierGlobalItem : GlobalItem
    {
        // [private static properties and fields]

        private static int rarityInfo;
        private static int[] hotbarRarityInfo;

        // [static constructors]

        static RarityModifierGlobalItem()
        {
            hotbarRarityInfo = new int[10];
        }

        // [public methods]

        public override void Load()
        {
            On_PopupText.Update += (orig, self, whoAmI) =>
            {
                orig(self, whoAmI);

                var system = ModContent.GetInstance<RarityModifierSystem>();
                var modifier = system.Modifiers.FirstOrDefault(x => x.AppliesToRarity(self.rarity), null);

                modifier?.ModifyPopupText(self);
            };

            On_Main.GUIHotbarDrawInner += (orig, self) =>
            {
                for (int i = 0; i < hotbarRarityInfo.Length; i++)
                {
                    ref var item = ref Main.LocalPlayer.inventory[i];

                    hotbarRarityInfo[i] = item.rare;

                    if (item.expert)
                        item.rare = ItemRarityID.Expert;

                    if (item.master)
                        item.rare = ItemRarityID.Master;
                }

                orig(self);

                for (int i = 0; i < hotbarRarityInfo.Length; i++)
                {
                    ref var item = ref Main.LocalPlayer.inventory[i];

                    item.rare = hotbarRarityInfo[i];
                }
            };

            MonoModHooks.Add(typeof(ItemLoader).GetMethod("PreDrawTooltip", BindingFlags.Public | BindingFlags.Static), ModifiedPreDrawTooltip);
            MonoModHooks.Add(typeof(ItemLoader).GetMethod("PostDrawTooltip", BindingFlags.Public | BindingFlags.Static), ModifiedPostDrawTooltip);
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod != "Terraria" || line.Name != "ItemName") return true;

            var system = ModContent.GetInstance<RarityModifierSystem>();

            if (!system.TryGetRenderedLineTexture(line.Text, out var lineTexture)) return true;

            var modifier = system.Modifiers.FirstOrDefault(x => x.AppliesToRarity(item.rare), null);

            return modifier?.PreDrawInTooltips(line, lineTexture) ?? true;
        }

        // [private methods]

        private bool ModifiedPreDrawTooltip(OrigPreDrawTooltip orig, Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            rarityInfo = item.rare;

            if (item.expert)
                item.rare = ItemRarityID.Expert;

            if (item.master)
                item.rare = ItemRarityID.Master;

            return orig(item, lines, ref x, ref y);
        }

        private void ModifiedPostDrawTooltip(OrigPostDrawTooltip orig, Item item, ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            orig(item, lines);

            item.rare = rarityInfo;
        }

        // [...]

        private delegate bool OrigPreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y);
        private delegate void OrigPostDrawTooltip(Item item, ReadOnlyCollection<DrawableTooltipLine> lines);
    }
}

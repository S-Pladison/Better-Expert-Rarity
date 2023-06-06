using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterExpertRarity.Common
{
    public class BERGlobalItem : GlobalItem
    {
        public override void Load()
        {
            MonoModHooks.Add(typeof(ItemLoader).GetMethod("SetDefaults", BindingFlags.NonPublic | BindingFlags.Static), ModifySetDefaults);

            On_Item.Prefix += (orig, item, pre) =>
            {
                var isExpertItem = item.rare == ItemRarityID.Expert;
                var isMasterItem = item.rare == ItemRarityID.Master;

                bool flag = orig(item, pre);

                if (isExpertItem) item.rare = ItemRarityID.Expert;
                else if (isMasterItem) item.rare = ItemRarityID.Master;

                return flag;
            };
        }

        // [private methods]

        private void ModifySetDefaults(OrigSetDefaults orig, Item item, bool createModItem)
        {
            orig(item, createModItem);

            if (item.expert)
                item.rare = ItemRarityID.Expert;

            if (item.master)
                item.rare = ItemRarityID.Master;
        }

        // [...]

        private delegate void OrigSetDefaults(Item item, bool createModItem);
    }
}
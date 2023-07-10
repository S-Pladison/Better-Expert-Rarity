using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace BetterExpertRarity.Common.Rarities
{
    public class RarityModifierSystem : ModSystem
    {
        // [public properties and fields]

        public static readonly IReadOnlyList<RarityModifier> Modifiers;

        // [private properties and fields]

        private static readonly List<RarityModifier> modifierInstances;

        // [static constructors]

        static RarityModifierSystem()
        {
            Modifiers = (modifierInstances = new()).AsReadOnly();
        }

        // [public static methods]

        public static void AddModifier(RarityModifier modifier)
        {
            if (modifierInstances.Contains(modifier)) return;

            modifierInstances.Add(modifier);
        }

        public static bool TryGetModifier(int rarity, out RarityModifier modifier)
        {
            modifier = modifierInstances.FirstOrDefault(x => x.AppliesToRarity(rarity), null);
            return modifier is not null;
        }

        // [public methods]

        public override void Unload()
        {
            modifierInstances.Clear();
        }

        public override void PostSetupContent()
        {
            Mod.Logger.Info(":| " + modifierInstances.Count);
        }
    }
}
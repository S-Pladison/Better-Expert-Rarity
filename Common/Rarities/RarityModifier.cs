using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace BetterExpertRarity.Common.Rarities
{
    public abstract class RarityModifier : ModType
    {
        // [...]

        public struct DrawData
        {
            public string Text;
            public Vector2 Position;
            public Color Color;
            public float Rotation;
            public Vector2 Origin;
            public Vector2 Scale;
            public float MaxWidth;
            public float ShadowSpread;
        }

        // [public methods]

        public abstract bool AppliesToRarity(int rarity);
        public abstract void Draw(DrawData data);

        // [protected methods]

        protected sealed override void Register()
        {
            ModTypeLookup<RarityModifier>.Register(this);
            RarityModifierSystem.AddModifier(this);
        }
    }
}
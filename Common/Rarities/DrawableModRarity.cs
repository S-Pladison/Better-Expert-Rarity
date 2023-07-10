using Terraria.ModLoader;

namespace BetterExpertRarity.Common.Rarities
{
    public abstract class DrawableModRarity : ModRarity
    {
        // [...]

        [Autoload(false)]
        private class DrawableModRarityModifier : RarityModifier
        {
            // [public properties and fields]

            public DrawableModRarity ModRarity { get; init; }

            // [constructors]

            public DrawableModRarityModifier(DrawableModRarity modRarity)
            {
                ModRarity = modRarity;
            }

            // [public methods]

            public override bool AppliesToRarity(int rarity)
                => ModRarity.Type == rarity;

            public override void Draw(DrawData data)
            {
                ModRarity.Draw(data);
            }
        }

        // [public methods]

        public sealed override void Load()
        {
            var modifier = new DrawableModRarityModifier(this);

            Mod.AddContent(modifier);

            Load(modifier);
        }

        public virtual void Load(RarityModifier modifier) { }
        public abstract void Draw(RarityModifier.DrawData data);
    }
}
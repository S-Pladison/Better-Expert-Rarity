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
            public override int RarityType { get => ModRarity.Type; }

            // [constructors]

            public DrawableModRarityModifier(DrawableModRarity modRarity)
            {
                ModRarity = modRarity;
            }

            // [public methods]

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
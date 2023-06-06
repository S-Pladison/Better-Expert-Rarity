using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BetterExpertRarity.Common.Rarities
{
    public abstract class RarityModifier : ModType
    {
        // [public methods]

        public abstract bool AppliesToRarity(int rarity);
        public virtual bool PreDrawInTooltips(DrawableTooltipLine line, Texture2D renderedLineTexture) { return true; }
        public virtual bool PreDrawInWorld(string text, Vector2 position, Texture2D renderedLineTexture) { return true; }
        public virtual void ModifyPopupText(PopupText popup) { }

        // [too lazy to implement]

        // public virtual void PostDrawInTooltips(DrawableTooltipLine line, Texture2D renderedLineTexture) { }
        // public virtual void PostDrawInWorld(string text, Vector2 position, Texture2D renderedLineTexture) { }

        // [protected methods]

        protected sealed override void Register()
        {
            ModContent.GetInstance<RarityModifierSystem>().AddModifier(this);
        }
    }
}
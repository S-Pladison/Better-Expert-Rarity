using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterExpertRarity.Rarities
{
    public class ExpertRarity : ShaderRarity
    {
        public ExpertRarity() : base(ModContent.Request<Effect>("BetterExpertRarity/ExpertRarity", AssetRequestMode.ImmediateLoad)) { }

        public override bool Condition(Item item) => item.expert || item.rare == ItemRarityID.Expert;

        public override void Load()
        {
            if (Main.dedServ) return;

            Effect.Value.Parameters["texture1"].SetValue(ModContent.Request<Texture2D>("BetterExpertRarity/rainbow", AssetRequestMode.ImmediateLoad).Value);
        }

        public override Effect GetUpdatedEffect()
        {
            var effect = Effect.Value;

            effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.Parameters["scale"].SetValue(Main.screenWidth / 1920f * 3.5f);

            return effect;
        }
    }
}
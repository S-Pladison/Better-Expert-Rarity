using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BetterExpertRarity.Rarities
{
    public class MasterRarity : ShaderRarity
    {
        public MasterRarity() : base(ModContent.Request<Effect>("BetterExpertRarity/MasterRarity", AssetRequestMode.ImmediateLoad)) { }

        public override bool Condition(Item item) => item.master || item.rare == ItemRarityID.Master;

        public override void Load()
        {
            if (Main.dedServ) return;

            Effect.Value.Parameters["texture1"].SetValue(ModContent.Request<Texture2D>("BetterExpertRarity/master", AssetRequestMode.ImmediateLoad).Value);
        }

        public override Effect GetUpdatedEffect()
        {
            var effect = Effect.Value;

            effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.8f);
            effect.Parameters["scale"].SetValue(Main.screenWidth / 1920f * 2f);

            return effect;
        }
    }
}
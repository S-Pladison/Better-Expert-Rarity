using BetterExpertRarity.Utils.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace BetterExpertRarity.Utils.Extensions
{
    public static class SpriteBatchExtensions
    {
        // [public static methods]

        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchData spriteBatchData)
        {
            spriteBatch.Begin
            (
                spriteBatchData.SortMode, spriteBatchData.BlendState, spriteBatchData.SamplerState, spriteBatchData.DepthStencilState,
                spriteBatchData.RasterizerState, spriteBatchData.Effect, spriteBatchData.Matrix
            );
        }

        public static void End(this SpriteBatch spriteBatch, out SpriteBatchData spriteBatchInfo)
        {
            spriteBatchInfo = new SpriteBatchData(spriteBatch);
            spriteBatch.End();
        }
    }
}
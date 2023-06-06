using BetterExpertRarity.Utils.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace BetterExpertRarity.Utils.Extensions
{
    public static class SpriteBatchExtensions
    {
        // [static fields]

        private static readonly FieldInfo beginCalledFieldInfo;

        // [constructors]

        static SpriteBatchExtensions()
        {
            beginCalledFieldInfo = typeof(SpriteBatch).GetField("beginCalled", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        // [public static methods]

        public static bool CheckBeginWasCalled(this SpriteBatch spriteBatch)
            => (bool)beginCalledFieldInfo.GetValue(spriteBatch);

        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchData spriteBatchData)
            => spriteBatch.Begin
            (
                spriteBatchData.SortMode, spriteBatchData.BlendState, spriteBatchData.SamplerState, spriteBatchData.DepthStencilState,
                spriteBatchData.RasterizerState, spriteBatchData.Effect, spriteBatchData.Matrix
            );

        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchData spriteBatchData, BlendState blendState)
            => spriteBatch.Begin
            (
                spriteBatchData.SortMode, blendState, spriteBatchData.SamplerState, spriteBatchData.DepthStencilState,
                spriteBatchData.RasterizerState, spriteBatchData.Effect, spriteBatchData.Matrix
            );

        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchData spriteBatchData, Effect effect)
            => spriteBatch.Begin
            (
                spriteBatchData.SortMode, spriteBatchData.BlendState, spriteBatchData.SamplerState, spriteBatchData.DepthStencilState,
                spriteBatchData.RasterizerState, effect, spriteBatchData.Matrix
            );

        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchData spriteBatchData, SpriteSortMode sortMode, BlendState blendState, Effect effect)
            => spriteBatch.Begin
            (
                sortMode, blendState, spriteBatchData.SamplerState, spriteBatchData.DepthStencilState,
                spriteBatchData.RasterizerState, effect, spriteBatchData.Matrix
            );

        public static void End(this SpriteBatch spriteBatch, out SpriteBatchData spriteBatchInfo)
        {
            spriteBatchInfo = new SpriteBatchData(spriteBatch);
            spriteBatch.End();
        }
    }
}
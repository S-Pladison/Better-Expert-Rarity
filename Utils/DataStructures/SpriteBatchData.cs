using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

namespace BetterExpertRarity.Utils.DataStructures
{
    public struct SpriteBatchData
    {
        // [static fields]

        private static readonly FieldInfo sortModeFieldInfo;
        private static readonly FieldInfo blendStateFieldInfo;
        private static readonly FieldInfo samplerStateFieldInfo;
        private static readonly FieldInfo depthStencilStateFieldInfo;
        private static readonly FieldInfo rasterizerStateFieldInfo;
        private static readonly FieldInfo effectFieldInfo;
        private static readonly FieldInfo matrixFieldInfo;

        // [fields]

        public SpriteSortMode SortMode;
        public BlendState BlendState;
        public SamplerState SamplerState;
        public DepthStencilState DepthStencilState;
        public RasterizerState RasterizerState;
        public Effect Effect;
        public Matrix Matrix;

        // [constructors]

        static SpriteBatchData()
        {
            var type = typeof(SpriteBatch);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            sortModeFieldInfo = type.GetField("sortMode", flags);
            blendStateFieldInfo = type.GetField("blendState", flags);
            samplerStateFieldInfo = type.GetField("samplerState", flags);
            depthStencilStateFieldInfo = type.GetField("depthStencilState", flags);
            rasterizerStateFieldInfo = type.GetField("rasterizerState", flags);
            effectFieldInfo = type.GetField("customEffect", flags);
            matrixFieldInfo = type.GetField("transformMatrix", flags);
        }

        public SpriteBatchData(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null)
                throw new ArgumentNullException(nameof(spriteBatch));

            SortMode = (SpriteSortMode)sortModeFieldInfo.GetValue(spriteBatch);
            BlendState = (BlendState)blendStateFieldInfo.GetValue(spriteBatch);
            SamplerState = (SamplerState)samplerStateFieldInfo.GetValue(spriteBatch);
            DepthStencilState = (DepthStencilState)depthStencilStateFieldInfo.GetValue(spriteBatch);
            RasterizerState = (RasterizerState)rasterizerStateFieldInfo.GetValue(spriteBatch);
            Effect = (Effect)effectFieldInfo.GetValue(spriteBatch);
            Matrix = (Matrix)matrixFieldInfo.GetValue(spriteBatch);
        }
    }
}
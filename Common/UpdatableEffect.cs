using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;

namespace BetterExpertRarity.Common
{
    public class UpdatableEffect
    {
        public Asset<Effect> Effect { get; private set; }
        private readonly Action _updateMethod;

        public UpdatableEffect(Asset<Effect> effect, Action updateMethod)
        {
            Effect = effect;

            _updateMethod = updateMethod;
        }

        public Effect GetUpdatedEffect()
        {
            _updateMethod?.Invoke();
            return Effect.Value;
        }
    }
}

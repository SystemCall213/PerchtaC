using System;
using UnityEngine;

namespace Combat.Arena
{
    public enum RadialPositionSelectorType
    {
        BalancedRandom,
        Random,
    }

    [Serializable]
    public struct RadialPositionSelectorData
    {
        public RadialPositionSelectorType type;
        [SerializeReference] public IRadialPositionSelector selector;

        public void ValidateAndInitialize()
        {
            if (selector != null && IsMatchingType()) return;

            selector = CreateSelector(type);
        }

        private bool IsMatchingType()
        {
            return type switch
            {
                RadialPositionSelectorType.BalancedRandom => selector is PositionSelectors.BalancedRandomIRadialPositionSelector,
                RadialPositionSelectorType.Random => selector is PositionSelectors.RandomRadialPositionSelector,
                _ => false
            };
        }

        private static IRadialPositionSelector CreateSelector(RadialPositionSelectorType type)
        {
            return type switch
            {
                RadialPositionSelectorType.BalancedRandom => new PositionSelectors.BalancedRandomIRadialPositionSelector(),
                RadialPositionSelectorType.Random => new PositionSelectors.RandomRadialPositionSelector(),
                _ => null
            };
        }
    }
}
using System;

namespace Combat.Arena.PositionSelectors
{
    [Serializable]
    [RadialPositionSelector(RadialPositionSelectorType.Random)]
    public class RandomRadialPositionSelector : IRadialPositionSelector
    {
        public float GetNextPositionFactor()
        {
            return UnityEngine.Random.Range(0f, 1f);
        }
    }
}
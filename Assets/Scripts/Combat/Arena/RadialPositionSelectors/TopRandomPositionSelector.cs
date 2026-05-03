using System;
using UnityEngine;

namespace Combat.Arena.RadialPositionSelectors
{
    [Serializable]
    [RadialPositionSelector(RadialPositionSelectorType.TopRandom)]
    public class TopRandomPositionSelector : IRadialPositionSelector
    {
        [SerializeField] private float spread = 20f;

        public float GetNextPositionFactor()
        {
            return (90f + UnityEngine.Random.Range(-spread, spread)) / 360f;
        }
    }
}
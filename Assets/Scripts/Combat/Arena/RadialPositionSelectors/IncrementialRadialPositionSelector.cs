using System;
using UnityEngine;
using Random = System.Random;

namespace Combat.Arena.PositionSelectors
{
    [Serializable]
    [RadialPositionSelector(RadialPositionSelectorType.Incremential)]
    public class IncrementialRadialPositionSelector : IRadialPositionSelector
    {
        [SerializeField] private float increment = 5f;

        private float offset;
        private Random random;
        
        public IncrementialRadialPositionSelector()
        {
            random = new Random();
            offset = random.Next(0, 360);
        }
        public float GetNextPositionFactor()
        {
            return (offset += increment)/360f;
        }
    }
}
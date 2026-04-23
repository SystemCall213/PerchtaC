using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Combat.Arena.PositionSelectors
{
    [Serializable]
    [RadialPositionSelector(RadialPositionSelectorType.SideRelativeRandom)]
    public class SideRelativeRandomPositionSelector : IRadialPositionSelector
    {
        [SerializeField] private float maxOffset = 20f;
        
        private List<int> _sides = new List<int>(){ 0, 1, 2, 3 }.Shuffle();
        private int lastSide;
        public float GetNextPositionFactor()
        {
            if (lastSide >= _sides.Count)
            {
                lastSide = 0;
                _sides = _sides.Shuffle();
            }

            float result =  (90*_sides[lastSide++] + UnityEngine.Random.Range(-maxOffset, maxOffset)) / 360;
            return result;
        }
    }
}
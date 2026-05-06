using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Combat.Arena.SideRelativePositionSelectors
{
    [Serializable]
    [SideRelativePositionSelector(SideRelativePositionSelectorType.BalancedRandom)]
    public class BalancedRandomSideRelativePositionSelector : ISideRelativePositionSelector
    {
        private List<int> _sides = new List<int> { 0, 1, 2, 3 }.Shuffle();
        private int _lastSideIndex;

        public ArenaPositionSideFactor GetNextPositionFactor()
        {
            if (_lastSideIndex >= _sides.Count)
            {
                _lastSideIndex = 0;
                _sides = _sides.Shuffle();
            }

            return new ArenaPositionSideFactor
            {
                Side = (ArenaPositionSideFactor.SideFactor)_sides[_lastSideIndex++],
                Factor = UnityEngine.Random.value
            };
        }
    }
}

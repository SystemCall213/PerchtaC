using System;
using UnityEngine;

namespace Combat.Arena.SideRelativePositionSelectors
{
    [Serializable]
    [SideRelativePositionSelector(SideRelativePositionSelectorType.Incremental)]
    public class IncrementalSideRelativePositionSelector : ISideRelativePositionSelector
    {
        [SerializeField] private float increment = 0.1f;
        
        private float _currentFactor;
        private int _currentSide;

        public ArenaPositionSideFactor GetNextPositionFactor()
        {
            _currentFactor += increment;
            while (_currentFactor >= 1f)
            {
                _currentFactor -= 1f;
                _currentSide = (_currentSide + 1) % 4;
            }

            return new ArenaPositionSideFactor
            {
                Side = (ArenaPositionSideFactor.SideFactor)_currentSide,
                Factor = _currentFactor
            };
        }
    }
}

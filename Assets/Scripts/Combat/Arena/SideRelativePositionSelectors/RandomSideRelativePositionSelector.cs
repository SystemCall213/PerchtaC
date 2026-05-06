using System;
using UnityEngine;

namespace Combat.Arena.SideRelativePositionSelectors
{
    [Serializable]
    [SideRelativePositionSelector(SideRelativePositionSelectorType.Random)]
    public class RandomSideRelativePositionSelector : ISideRelativePositionSelector
    {
        public ArenaPositionSideFactor GetNextPositionFactor()
        {
            return new ArenaPositionSideFactor
            {
                Side = (ArenaPositionSideFactor.SideFactor)UnityEngine.Random.Range(0, 4),
                Factor = UnityEngine.Random.value
            };
        }
    }
}

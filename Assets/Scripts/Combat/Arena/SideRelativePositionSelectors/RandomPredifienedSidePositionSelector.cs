using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Combat.Arena.SideRelativePositionSelectors
{
    [Serializable]
    [SideRelativePositionSelector(SideRelativePositionSelectorType.PredifienedSideRandom)]
    public class RandomPredifienedSidePositionSelector : ISideRelativePositionSelector
    {
        [SerializeField] private ArenaPositionSideFactor.SideFactor side;

        public ArenaPositionSideFactor GetNextPositionFactor()
        {
            return new ArenaPositionSideFactor
            {
                Side = side,
                Factor = UnityEngine.Random.value
            };
        }
    }
}
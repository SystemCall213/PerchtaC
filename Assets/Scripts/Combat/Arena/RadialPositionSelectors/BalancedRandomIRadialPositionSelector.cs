using System;
using UnityEngine;

namespace Combat.Arena.PositionSelectors
{
    [Serializable]
    [RadialPositionSelector(RadialPositionSelectorType.BalancedRandom)]
    public class BalancedRandomIRadialPositionSelector : IRadialPositionSelector
    {
        [SerializeField] private float highDifferenceFactor;
        [SerializeField] private float lowDifferenceFactor;
        
        private float _lastPositionFactor;

        public float GetNextPositionFactor()
        {
            float difference = Mathf.Sign(UnityEngine.Random.Range(-1f, 1f)) * highDifferenceFactor + UnityEngine.Random.Range(-lowDifferenceFactor, lowDifferenceFactor);
            _lastPositionFactor += difference;
            return _lastPositionFactor/360f;
        }
    }
}
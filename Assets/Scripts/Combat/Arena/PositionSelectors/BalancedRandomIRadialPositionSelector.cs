using System;
using UnityEngine;

namespace Combat.Arena.PositionSelectors
{
    [Serializable]
    public class BalancedRandomIRadialPositionSelector : IRadialPositionSelector
    {
        [SerializeField] private float highDifferenceFactor;
        [SerializeField] private float lowDifferenceFactor;
        
        private float _lastPositionFactor;

        public float GetNextPositionFactor()
        {
            float difference = Mathf.Sign(UnityEngine.Random.Range(-1f, 1f)) * highDifferenceFactor + lowDifferenceFactor * UnityEngine.Random.Range(-1f, 1f);
            _lastPositionFactor += difference;
            return _lastPositionFactor/360f;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Interfaces;
using DefaultNamespace;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "CombatScenario", menuName = "Combat/Scenario")]
    public class CombatScenario : ScriptableObject, ICombatScenario
    {
        [SerializeField] private List<AttackStrategy> attackQueue;
        [SerializeField] private ScriptableObject intermediateStrategy;
        
        private List<AttackStrategy> _shuffledQueue;
        private int _currentIndex = 0;

        public void Initialize()
        {
            ResetAttackQueue();
        }

        private void ResetAttackQueue()
        {
            _currentIndex = 0;
            _shuffledQueue = attackQueue.Shuffle();
        }

        public IAttackStrategy GetNextAttack()
        {
            if (_currentIndex >= _shuffledQueue.Count)
            {
                ResetAttackQueue();
            }
            var attack = _shuffledQueue[_currentIndex++];
            return attack;
        }

        public IAttackStrategy GetIntermediateStrategy()
        {
            return intermediateStrategy as IAttackStrategy;
        }
        
    }
}

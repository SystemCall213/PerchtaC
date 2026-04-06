using System.Collections.Generic;
using Combat.Interfaces;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "CombatScenario", menuName = "Combat/Scenario")]
    public class CombatScenario : ScriptableObject, ICombatScenario
    {
        [SerializeField] private List<ScriptableObject> attackQueue;
        [SerializeField] private ScriptableObject intermediateStrategy;

        private int _currentIndex = 0;

        public IAttackStrategy GetNextAttack()
        {
            if (attackQueue == null || attackQueue.Count == 0) return null;
            
            var attack = attackQueue[_currentIndex] as IAttackStrategy;
            if (attack != null)
            {
                _currentIndex = (_currentIndex + 1) % attackQueue.Count;
            }
            return attack;
        }

        public IAttackStrategy GetIntermediateStrategy()
        {
            return intermediateStrategy as IAttackStrategy;
        }
    }
}

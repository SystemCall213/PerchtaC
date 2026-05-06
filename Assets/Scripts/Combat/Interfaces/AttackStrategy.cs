using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Combat.Interfaces
{
    public abstract class AttackStrategy : ScriptableObject, IAttackStrategy
    {
        [SerializeReference] List<ICombatUXElement> uxElements;
        public event Action OnAttackStarted;
        public event Action OnAttackFinished;
        public abstract void StartAttack(CancellationToken ct);

        public abstract bool IsAttacking();
        
        protected void RaiseAttackStarted()
        {
            OnAttackStarted?.Invoke();
        }
        
        protected void RaiseAttackFinished()
        {
            OnAttackFinished?.Invoke();
        }

        private void OnEnable()
        {
            foreach (var uxElement in uxElements)
            {
                uxElement.Initialize(this);
            }
        }
    }
}
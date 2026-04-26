using System.Threading;
using UnityEngine;

namespace Combat.Interfaces
{
    public abstract class AttackStrategy : ScriptableObject, IAttackStrategy
    {
        public abstract void StartAttack(CancellationToken ct);

        public abstract bool IsAttacking();
    }
}
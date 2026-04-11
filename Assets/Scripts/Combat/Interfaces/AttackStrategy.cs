using System.Threading;
using UnityEditor.Experimental.Rendering;
using UnityEngine;

namespace Combat.Interfaces
{
    public abstract class AttackStrategy : ScriptableObject, IAttackStrategy
    {
        public abstract void StartAttack(CancellationToken ct);

        public abstract bool IsAttacking();
    }
}
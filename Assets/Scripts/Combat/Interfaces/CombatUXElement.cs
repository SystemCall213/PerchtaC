using UnityEngine;

namespace Combat.Interfaces
{
    public abstract class CombatUXElement : MonoBehaviour, ICombatUXElement
    {
        public abstract void Initialize(IAttackStrategy attackStrategy);
    }
}
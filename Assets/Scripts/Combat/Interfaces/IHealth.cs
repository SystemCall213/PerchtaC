using System;

namespace Combat.Interfaces
{
    public interface IHealth
    {
        event Action<int> OnDamage;
        event Action<int> OnHeal;
        event Action OnDeath;
        
        void TakeDamage(int damage);
        void Heal(int heal);
        void Die();
        bool IsDead();
        int GetMaxHealth();
    }
}
namespace Combat.Interfaces
{
    public interface IHealth
    {
        void TakeDamage(int damage);
        void Heal(int heal);
        void Die();
        bool IsDead();
    }
}
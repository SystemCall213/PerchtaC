namespace Combat.Interfaces
{
    public interface IAttackStrategy
    {
        void StartAttack();
        bool IsAttacking();
    }
}
namespace Combat.Interfaces
{
    public interface ICombatScenario
    {
        void Initialize();
        IAttackStrategy GetNextAttack();
        IAttackStrategy GetIntermediateStrategy();
    }
}
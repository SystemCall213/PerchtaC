namespace Combat.Interfaces
{
    public interface ICombatScenario
    {
        IAttackStrategy GetNextAttack();
        IAttackStrategy GetIntermediateStrategy();
    }
}
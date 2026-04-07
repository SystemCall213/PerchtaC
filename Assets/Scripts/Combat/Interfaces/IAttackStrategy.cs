using System.Threading;

namespace Combat.Interfaces
{
    public interface IAttackStrategy
    {
        void StartAttack(CancellationToken ct);
        bool IsAttacking();
    }
}
using System;
using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Combat
{
    public class CombatController : IInitializable, IDisposable
    {
        private readonly ICombatScenario _scenario;
        private readonly DiContainer _container;
        private CancellationTokenSource _cts;

        public CombatController(ICombatScenario scenario, DiContainer container)
        {
            _scenario = scenario;
            _container = container;
        }

        public void Initialize()
        {
            _cts = new CancellationTokenSource();
            RunCombatLoop(_cts.Token).Forget();
        }

        private async UniTaskVoid RunCombatLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // Attack Phase
                IAttackStrategy attack = _scenario.GetNextAttack();
                if (attack != null)
                {
                    _container.Inject(attack);
                    attack.StartAttack();
                    await UniTask.WaitWhile(() => attack.IsAttacking(), cancellationToken: token);
                }

                // Intermediate Phase
                IAttackStrategy intermediate = _scenario.GetIntermediateStrategy();
                if (intermediate != null)
                {
                    _container.Inject(intermediate);
                    intermediate.StartAttack();
                    await UniTask.WaitWhile(() => intermediate.IsAttacking(), cancellationToken: token);
                }
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
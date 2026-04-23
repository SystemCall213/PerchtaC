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
            _scenario.Initialize();
            while (!token.IsCancellationRequested)
            {
                // Attack Phase
                IAttackStrategy attack = _scenario.GetNextAttack();
                _container.Inject(attack);
                attack.StartAttack(token);
                await UniTask.WaitWhile(() => attack.IsAttacking(), cancellationToken: token);

                // Intermediate Phase
                IAttackStrategy intermediate = _scenario.GetIntermediateStrategy();
                _container.Inject(intermediate);
                intermediate.StartAttack(token);
                await UniTask.WaitWhile(() => intermediate.IsAttacking(), cancellationToken: token);
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
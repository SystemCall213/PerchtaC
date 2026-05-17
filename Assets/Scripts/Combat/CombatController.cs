using System;
using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using UI;
using Zenject;

namespace Combat
{
    public class CombatController : IInitializable, IDisposable
    {
        private readonly ICombatScenario _scenario;
        private readonly DiContainer _container;
        private readonly TutorialMenu _tutorialMenu;
        private CancellationTokenSource _cts;

        public CombatController(ICombatScenario scenario, DiContainer container, TutorialMenu tutorialMenu)
        {
            _scenario = scenario;
            _container = container;
            _tutorialMenu = tutorialMenu;
        }

        public void Initialize()
        {
            _tutorialMenu.TryOpenTutorial();
            _cts = new CancellationTokenSource();
            RunCombatLoop(_cts.Token).Forget();
        }

        private async UniTaskVoid RunCombatLoop(CancellationToken token)
        {
            _scenario.Initialize();
            while (!token.IsCancellationRequested)
            {
                // Intermediate Phase
                IAttackStrategy intermediate = _scenario.GetIntermediateStrategy();
                _container.Inject(intermediate);
                intermediate.StartAttack(token);
                await UniTask.WaitWhile(() => intermediate.IsAttacking(), cancellationToken: token);
                
                // Attack Phase
                IAttackStrategy attack = _scenario.GetNextAttack();
                _container.Inject(attack);
                attack.StartAttack(token);
                await UniTask.WaitWhile(() => attack.IsAttacking(), cancellationToken: token);
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
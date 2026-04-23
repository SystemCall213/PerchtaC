using System.Threading;
using Combat.Arena;
using Combat.Interfaces;
using Combat.Misc;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "RandomSpawnAttack", menuName = "Combat/Strategies/RandomSpawn")]
    public class DelayedSpawnAttackStrategy : AttackStrategy
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int spawnCount = 10;
        [SerializeField] private float delayBetweenSpawns = 0.5f;
        [SerializeField] private RadialPositionSelectorData positionSelectorData;

        [Inject] private CombatArena _arena;
        [Inject] private IInstantiator _instantiator;
        private bool _isAttacking;

        private void OnEnable()
        {
            positionSelectorData.ValidateAndInitialize();
        }

        public override void StartAttack(CancellationToken ct)
        {
            ExecuteAsync(ct).Forget();
        }

        private async UniTaskVoid ExecuteAsync(CancellationToken ct)
        {
            _isAttacking = true;
            
            for (int i = 0; i < spawnCount; i++)
            {
                if (ct.IsCancellationRequested) break;
                
                SpawnProjectile(positionSelectorData.selector.GetNextPositionFactor());
                await UniTask.Delay((int)(delayBetweenSpawns * 1000), cancellationToken: ct);
            }

            if (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(1000, cancellationToken: ct);
            }
            
            _isAttacking = false;
        }

        private void SpawnProjectile(float factor)
        {
            Vector2 spawnPos = _arena.GetPositionOutside(factor);
            GameObject proj = _instantiator.InstantiatePrefab(prefab, spawnPos, Quaternion.identity, null);
        }

        public override bool IsAttacking() => _isAttacking;
    }
}

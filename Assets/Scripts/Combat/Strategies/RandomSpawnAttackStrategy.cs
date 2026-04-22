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
    public class RandomSpawnAttackStrategy : AttackStrategy
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
                
                positionSelectorData.ValidateAndInitialize();
                if (positionSelectorData.selector != null)
                {
                    SpawnProjectile(positionSelectorData.selector.GetNextPositionFactor());
                }
                
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
            if (_arena == null) return;

            Vector2 spawnPos = _arena.GetPositionOutside(factor);
            Vector2 targetPos = _arena.GetRandomPointInsideCenter();
            
            GameObject proj = _instantiator.InstantiatePrefab(prefab, spawnPos, Quaternion.identity, null);
            Vector2 direction = (targetPos - spawnPos).normalized;
            proj.transform.up = direction;
        }

        public override bool IsAttacking() => _isAttacking;
    }
}

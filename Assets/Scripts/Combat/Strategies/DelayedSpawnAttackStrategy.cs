using System.Collections.Generic;
using System.Threading;
using Combat.Arena;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "RandomSpawnAttack", menuName = "Combat/Strategies/RandomSpawn")]
    public class DelayedSpawnAttackStrategy : AttackStrategy
    {
        [SerializeField] private List<GameObject> prefabs;
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
            RaiseAttackStarted();
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
            RaiseAttackFinished();
        }

        private void SpawnProjectile(float factor)
        {
            Vector2 spawnPos = _arena.GetPositionOutside(factor);
            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject proj = _instantiator.InstantiatePrefab(randomPrefab, spawnPos, Quaternion.identity, null);
        }


        public override bool IsAttacking() => _isAttacking;
    }
}

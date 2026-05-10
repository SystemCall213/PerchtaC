using System.Collections.Generic;
using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "SpawnerDelayedAttack", menuName = "Combat/Strategies/SpawnerDelayedAttack")]
    public class SpawnerDelayedAttackStrategy : AttackStrategy
    {
        [SerializeField] private List<GameObject> prefabs;
        [SerializeField] private int spawnCount = 10;
        [SerializeField] private float delayBetweenSpawns = 0.5f;

        [Inject] private SpawnerPositionSelector _spawnerSelector;
        [Inject] private IInstantiator _instantiator;
        
        private bool _isAttacking;

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
                
                SpawnProjectile(_spawnerSelector.GetNextPosition());
                await UniTask.Delay((int)(delayBetweenSpawns * 1000), cancellationToken: ct);
            }

            if (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(1000, cancellationToken: ct);
            }
            
            _isAttacking = false;
            RaiseAttackFinished();
        }

        private void SpawnProjectile(Vector3 position)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject spawned = _instantiator.InstantiatePrefab(prefab, position, Quaternion.identity, null);
        }

        public override bool IsAttacking() => _isAttacking;
    }
}
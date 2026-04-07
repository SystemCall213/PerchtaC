using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "RandomSpawnAttack", menuName = "Combat/Strategies/RandomSpawn")]
    public class RandomBulletAttackStrategy : ScriptableObject, IAttackStrategy
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int spawnCount = 10;
        [SerializeField] private float delayBetweenSpawns = 0.5f;

        [Inject] private CombatArena _arena;
        private bool _isAttacking;

        public void StartAttack(CancellationToken ct)
        {
            ExecuteAsync(ct).Forget();
        }

        private async UniTaskVoid ExecuteAsync(CancellationToken ct)
        {
            _isAttacking = true;

            for (int i = 0; i < spawnCount; i++)
            {
                if (ct.IsCancellationRequested) break;
                SpawnProjectile();
                await UniTask.Delay((int)(delayBetweenSpawns * 1000), cancellationToken: ct);
            }

            if (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(1000, cancellationToken: ct);
            }
            
            _isAttacking = false;
        }

        private void SpawnProjectile()
        {
            if (_arena == null) return;

            Vector2 spawnPos = _arena.GetRandomPositionOutside();
            Vector2 targetPos = _arena.GetRandomPointInsideCenter();
            
            GameObject proj = Instantiate(prefab, spawnPos, Quaternion.identity);
            Vector2 direction = (targetPos - spawnPos).normalized;
            proj.transform.up = direction;
        }

        public bool IsAttacking() => _isAttacking;
    }
}

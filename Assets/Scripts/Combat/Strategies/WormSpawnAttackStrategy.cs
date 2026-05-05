using System;
using System.Threading;
using Combat.Arena;
using Combat.Interfaces;
using Combat.Misc;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "WormSpawnAttackStrategy", menuName = "Combat/Attack Strategies/Worm Spawn Attack")]
    public class WormSpawnAttackStrategy : AttackStrategy
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int spawnCountPerWave = 2;
        [SerializeField] private float waveCount = 4;
        [SerializeField] private float delayBetweenSpawns = 0.5f;
        
        [Inject] private IInstantiator instantiator;
        [Inject] private IPlayerMovement playerMovement;
        [Inject] private CombatArena arena;
        
        private bool isAttacking;
        
        public override void StartAttack(CancellationToken ct)
        {
            SpawnWorm(ct).Forget();
        }

        public async UniTaskVoid SpawnWorm(CancellationToken ct)
        {
            isAttacking = true;
            for (int i = 0; i < waveCount; i++)
            {
                for (int j = 0; j < spawnCountPerWave; j++)
                {
                    if (ct.IsCancellationRequested) break;
                    Vector2 position = arena.GetRandomPositionOutside(30);
                    Vector2 dir = (playerMovement.Position - position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    Quaternion rotation = Quaternion.Euler(0, 0, angle);
                    GameObject worm = instantiator.InstantiatePrefab(prefab, position, rotation, null);
                }
                await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenSpawns), cancellationToken: ct);
            }
            isAttacking = false;
        }

        public override bool IsAttacking()
        {
            return isAttacking;
        }
    }
}
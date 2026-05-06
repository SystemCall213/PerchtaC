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
        [SerializeField] private RadialPositionSelectorData positionSelectorData;
        
        [Inject] private IInstantiator instantiator;
        [Inject] private IPlayerMovement playerMovement;
        [Inject] private CombatArena arena;
        
        private bool isAttacking;
        
        private void OnEnable()
        {
            positionSelectorData.ValidateAndInitialize();
        }
        
        public override void StartAttack(CancellationToken ct)
        {
            if (isAttacking) return;
            SpawnWorm(ct).Forget();
            RaiseAttackStarted();
        }

        public async UniTaskVoid SpawnWorm(CancellationToken ct)
        {
            isAttacking = true;
            for (int i = 0; i < waveCount; i++)
            {
                for (int j = 0; j < spawnCountPerWave; j++)
                {
                    Debug.Log("Spawning worm");
                    if (ct.IsCancellationRequested) break;
                    float factor = positionSelectorData.selector.GetNextPositionFactor();
                    Vector2 position = arena.GetPositionOutside(factor, 14f);
                    Vector2 dir = (playerMovement.Position - position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    Quaternion rotation = Quaternion.Euler(0, 0, angle);
                    GameObject worm = instantiator.InstantiatePrefab(prefab, position, rotation, null);
                    Debug.Log("Worm spawned");
                }
                await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenSpawns), cancellationToken: ct);
            }
            isAttacking = false;
            RaiseAttackFinished();
        }

        public override bool IsAttacking()
        {
            return isAttacking;
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using Combat.Arena;
using Combat.Arena.SideRelativePositionSelectors;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "SideRelativeDelayedSpawnAttack", menuName = "Combat/Strategies/SideRelativeDelayedSpawn")]
    public class SideRelativeDelayedSpawnAttackStrategy : AttackStrategy
    {
        [SerializeField] private List<GameObject> prefabs;
        [SerializeField] private int spawnCount = 10;
        [SerializeField] private float delayBetweenSpawns = 0.5f;
        [SerializeField] private SideRelativePositionSelectorData positionSelectorData;
        
        [SerializeField] private float projectileOffset = 0f;

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

        private void SpawnProjectile(ArenaPositionSideFactor factor)
        {
            Vector2 spawnPos = _arena.GetSidedPosition(factor, projectileOffset);
            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject proj = _instantiator.InstantiatePrefab(randomPrefab, spawnPos, Quaternion.identity, null);
        }

        public override bool IsAttacking() => _isAttacking;
    }
}
using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "PredeterminedSpawn", menuName = "Combat/Strategies/PredeterminedSpawn")]
    public class PredeterminedSpawn : AttackStrategy
    {
        [SerializeField] private GameObject spawnedObject;
        [SerializeField] private Vector2 spawnPosition;
        [SerializeField] private float attackLegth = 10f;
        
        [Inject] private IInstantiator _instantiator;
        
        private bool _isAttacking;
        public override void StartAttack(CancellationToken ct)
        {
            if (spawnedObject == null)
            {
                Debug.LogError("Spawned object is not set for PredeterminedSpawn strategy.");
                return;
            }
            _isAttacking = true;
            AttackCoroutine(ct).Forget();
        }
        
        private async UniTask AttackCoroutine(CancellationToken ct)
        {
            _instantiator.InstantiatePrefab(spawnedObject, spawnPosition, Quaternion.identity, null);
            await UniTask.Delay((int)(attackLegth * 1000), cancellationToken: ct);
            _isAttacking = false;

        }

        public override bool IsAttacking()
        {
            return _isAttacking;
        }
    }
}
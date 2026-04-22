using System.Threading;
using Combat.Arena;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "StampAttack", menuName = "Combat/Strategies/StampAttack")]
    public class StampAttackStrategy : AttackStrategy
    {
        [SerializeField] private GameObject warningAreaPrefab;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float duration = 2.0f;
        [SerializeField] private int damage = 2;
        [SerializeField] private int bulletCount = 8;
        [SerializeField] private float bulletSpeed = 5f;
        [SerializeField] private int stampCount = 3;
        [SerializeField] private float stampDelay = 1f;

        [Inject] private PlayerMovement player;
        [Inject] private CombatArena arena;

        private bool isAttacking;

        public override void StartAttack(CancellationToken ct)
        {
            ExecuteAsync(ct).Forget();
        }

        private async UniTaskVoid ExecuteAsync(CancellationToken cancellationToken)
        {
            if (player == null) return;
            
            isAttacking = true;

            for (int i = 0; i < stampCount; i++)
            {
                if (cancellationToken.IsCancellationRequested) break;

                Vector3 spawnPos = player.transform.position;
                GameObject warningObj = Instantiate(warningAreaPrefab, spawnPos, Quaternion.identity);
                
                if (warningObj.TryGetComponent<WarningArea>(out var warningArea))
                {
                    warningArea.Initialize(duration, damage, bulletCount, bulletPrefab, bulletSpeed, cancellationToken);
                }

                await UniTask.Delay((int)(stampDelay * 1000), cancellationToken: cancellationToken);
            }

            isAttacking = false;
        }

        public override bool IsAttacking() => isAttacking;
    }
}

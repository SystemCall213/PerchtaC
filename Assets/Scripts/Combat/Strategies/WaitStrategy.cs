using System;
using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "WaitStrategy", menuName = "Combat/Strategies/Wait")]
    public class WaitStrategy : AttackStrategy
    {
        [SerializeField] private float duration = 2f;
        private bool _isAttacking;

        public override void StartAttack(CancellationToken ct)
        {
            ExecuteAsync(ct).Forget();
        }

        private async UniTaskVoid ExecuteAsync(CancellationToken ct)
        {
            _isAttacking = true;
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
            _isAttacking = false;
        }

        public override bool IsAttacking() => _isAttacking;
    }
}

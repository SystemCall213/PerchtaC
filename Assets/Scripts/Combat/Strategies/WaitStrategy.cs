using System;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Strategies
{
    [CreateAssetMenu(fileName = "WaitStrategy", menuName = "Combat/Strategies/Wait")]
    public class WaitStrategy : ScriptableObject, IAttackStrategy
    {
        [SerializeField] private float duration = 2f;
        private bool _isAttacking;

        public void StartAttack()
        {
            ExecuteAsync().Forget();
        }

        private async UniTaskVoid ExecuteAsync()
        {
            _isAttacking = true;
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            _isAttacking = false;
        }

        public bool IsAttacking() => _isAttacking;
    }
}

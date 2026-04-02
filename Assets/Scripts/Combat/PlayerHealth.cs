using System;
using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Combat
{
    public class PlayerHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private float ImmunityTime;
        
        private SpriteRenderer spriteRenderer;
        private int health;
        private float currentImmunityTimer;
        private CancellationTokenSource immunityCts;
        private Tween immunityTween;

        public event Action OnDeath;

        public bool IsImmune => currentImmunityTimer > 0;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            health = maxHealth;
        }

        private void OnDestroy()
        {
            CancelImmunity();
        }

        public void TakeDamage(int damage)
        {
            if (IsImmune) return;

            health -= damage;

            if (IsDead())
            {
                Die();
            }
            else
            {
                GainImmunity(ImmunityTime).Forget();
            }
        }

        public void Heal(int heal)
        {
            health = Mathf.Min(health + heal, maxHealth);
        }

        public bool IsDead()
        {
            return health <= 0;
        }

        public async UniTaskVoid GainImmunity(float duration)
        {
            if (IsImmune)
            {
                currentImmunityTimer = Mathf.Max(currentImmunityTimer, duration);
                return;
            }

            CancelImmunity();
            immunityCts = new CancellationTokenSource();
            
            currentImmunityTimer = duration;

            immunityTween = spriteRenderer.DOFade(0f, 0.1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            try
            {
                while (currentImmunityTimer > 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, immunityCts.Token);
                    currentImmunityTimer -= Time.deltaTime;
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                StopImmunityVisuals();
                currentImmunityTimer = 0;
            }
        }

        private void CancelImmunity()
        {
            immunityCts?.Cancel();
            immunityCts?.Dispose();
            immunityCts = null;
        }

        private void StopImmunityVisuals()
        {
            immunityTween?.Kill();
            immunityTween = null;
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        public void Die()
        {
            Debug.Log("Player Died");
            OnDeath?.Invoke();
        }
    }
}
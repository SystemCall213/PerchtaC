using Cysharp.Threading.Tasks;
using UnityEngine;
using Combat.Misc;

namespace Combat.Strategies
{
    public class WarningArea : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float shimmerFrequency = 5f;
        [SerializeField] private float shimmerMinAlpha = 0.3f;
        [SerializeField] private float shimmerMaxAlpha = 0.8f;

        private float duration;
        private int damage;
        private int bulletCount;
        private GameObject bulletPrefab;
        private float bulletSpeed;
        private float elapsedTime;

        public void Initialize(float duration, int damage, int bulletCount, GameObject bulletPrefab, float bulletSpeed)
        {
            this.duration = duration;
            this.damage = damage;
            this.bulletCount = bulletCount;
            this.bulletPrefab = bulletPrefab;
            this.bulletSpeed = bulletSpeed;
            
            StartAttackSequence().Forget();
        }

        private async UniTaskVoid StartAttackSequence()
        {
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / duration);
                Color targetColor = Color.Lerp(Color.white, Color.red, progress);
                float shimmer = Mathf.Lerp(shimmerMinAlpha, shimmerMaxAlpha, (Mathf.Sin(Time.time * shimmerFrequency) + 1f) / 2f);
                targetColor.a = shimmer;
                
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = targetColor;
                }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            PerformStamp();

            Destroy(gameObject, 0.1f);
        }

        private void PerformStamp()
        {
            float radius = transform.localScale.x * 1.5f;
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, radius);
            
            foreach (var col in hitPlayers)
            {
                if (col.TryGetComponent<PlayerHealth>(out var playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                }
            }

            float angleStep = 360f / bulletCount;
            for (int i = 0; i < bulletCount; i++)
            {
                float angle = i * angleStep;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                GameObject bulletObj = Instantiate(bulletPrefab, transform.position, rotation);
                
                if (bulletObj.TryGetComponent<Bullet>(out var bullet))
                {
                    bullet.SetSpeed(bulletSpeed);
                }
            }
        }
    }
}

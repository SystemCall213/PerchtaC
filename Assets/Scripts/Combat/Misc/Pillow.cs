using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Combat.Misc
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Pillow : MonoBehaviour
    {
        [Header("Fall Animation")]
        [SerializeField] private float fallDuration = 1.0f;
        [SerializeField] private float startScale = 2.0f;
        [SerializeField] private float endScale = 1.0f;

        [Header("Fade Out Animation")]
        [SerializeField] private float fadeOutDuration = 0.2f;

        [Header("Collision & Damage")]
        [SerializeField] private int damage = 1;
        [SerializeField] private float collisionRadius = 0.5f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Spawning")]
        [SerializeField] private List<GameObject> prefabsToSpawn;
        [SerializeField] private int spawnCount = 1;

        [Inject] private IInstantiator _instantiator;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            StartFalling();
        }

        private void StartFalling()
        {
            SetInitialState();
            AnimateFall();
        }

        private void SetInitialState()
        {
            transform.localScale = Vector3.one * startScale;
            
            Color color = spriteRenderer.color;
            color.a = 0f;
            spriteRenderer.color = color;
        }

        private void AnimateFall()
        {
            Sequence fallSequence = DOTween.Sequence();
            
            fallSequence.Append(spriteRenderer.DOFade(1f, fallDuration));
            fallSequence.Join(transform.DOScale(endScale, fallDuration));
            
            fallSequence.OnComplete(OnFallCompleted);
        }

        private void OnFallCompleted()
        {
            PerformCollisionCheck();
            StartFadeOut();
        }

        private void PerformCollisionCheck()
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, collisionRadius, playerLayer);
            
            foreach (var col in hitPlayers)
            {
                if (col.TryGetComponent<PlayerHealth>(out var playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }

        private void StartFadeOut()
        {
            spriteRenderer.DOFade(0f, fadeOutDuration).OnComplete(() =>
            {
                SpawnPrefab();
                Destroy(gameObject);
            });
        }

        private void SpawnPrefab()
        {
            if (prefabsToSpawn is { Count: > 0 })
            {
                for (int i = 0; i < spawnCount; i++)
                {
                    GameObject wool = _instantiator.InstantiatePrefab(prefabsToSpawn[i % prefabsToSpawn.Count], transform.position, Quaternion.identity, null);
                    wool.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, collisionRadius);
        }
    }
}
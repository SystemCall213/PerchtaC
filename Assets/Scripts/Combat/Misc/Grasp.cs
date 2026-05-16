using System;
using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Combat.Misc
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(DamageOnCollision))]
    public class Grasp : MonoBehaviour
    {
        [Inject] private IPlayerMovement playerMovement;
        
        [SerializeField] private float force = 10f;
        [SerializeField] private float lifetime = 30f;
        [SerializeField] private float grabDuration = 5f;
        [SerializeField] private float sineFrequency = 2f;
        [SerializeField] private float sineMagnitude = 5f;
        [SerializeField] private Rigidbody2D hold;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite hand_open;
        [SerializeField] private Sprite hand_closed;
        
        private Rigidbody2D rb;
        private bool isGrabbing = true;
        private DamageOnCollision damageOnCollision;
        private CancellationTokenSource handSpriteCts;
        
        public void StopGrabbing()
        {
            isGrabbing = false;
            Destroy(gameObject, 10f);
        }
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            damageOnCollision = GetComponent<DamageOnCollision>();
            damageOnCollision.OnDamage += _ => PlayerDamaged();
        }

        private void Start()
        {
            DamageOnCollision damageOnCollision = gameObject.GetComponent<DamageOnCollision>();
            if (damageOnCollision != null)
            {
                damageOnCollision.OnDamage += _ => StopGrabbing();
            }
            else
            {
                Debug.LogError("Hold object must have DamageOnCollision component");
            }
            Destroy(gameObject, lifetime);
            StopGrabbingAfterDelay().Forget();
            transform.position -= hold.transform.localPosition * transform.localScale[0];
        }

        private async UniTaskVoid StopGrabbingAfterDelay()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(grabDuration), delayTiming: PlayerLoopTiming.Update, cancellationToken: this.GetCancellationTokenOnDestroy());
            StopGrabbing();
        }

        private void FixedUpdate()
        {
            if (isGrabbing)
            {
                ApplyForceTowardsPlayer();
            }
            else
            {
                ApplyForceBackwards();
            }
        }

        private void ApplyForceTowardsPlayer()
        {
            Vector2 playerPos = playerMovement.Position;
            Vector2 currentPos = transform.position;
            Vector2 direction = (playerPos - currentPos).normalized;
            
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            
            float offset = Mathf.Sin(Time.time * sineFrequency) * sineMagnitude;
            
            Vector2 finalDirection = (direction * (force * Time.fixedDeltaTime)) + (perpendicular * (offset * Time.fixedDeltaTime));
            rb.AddForce(finalDirection, ForceMode2D.Impulse);
        }

        private void ApplyForceBackwards()
        {
            Vector2 direction = (hold.transform.position.ConvertToVector2() - playerMovement.Position).normalized;
            hold.AddForce(direction * (force * 400 * Time.fixedDeltaTime), ForceMode2D.Impulse);
        }

        private void PlayerDamaged()
        {
            FlashHandClosedForSeconds();
        }
        
        private void FlashHandClosedForSeconds(float seconds = 1f)
        {
            // cancel previous flash if any
            if (handSpriteCts != null)
            {
                handSpriteCts.Cancel();
                handSpriteCts.Dispose();
            }
        
            handSpriteCts = new CancellationTokenSource();
            ChangeHandSpriteAsync(seconds, handSpriteCts.Token).Forget();
        }
        
        private async UniTaskVoid ChangeHandSpriteAsync(float seconds, CancellationToken ct)
        {
            if (spriteRenderer == null || hand_closed == null || hand_open == null)
                return;
        
            try
            {
                spriteRenderer.sprite = hand_closed;
        
                // wait for the duration or until cancelled
                await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: ct);
            }
            catch (OperationCanceledException)
            {
                // cancelled: do nothing (we'll try to restore sprite below only if not cancelled)
            }
        
            if (!ct.IsCancellationRequested && spriteRenderer != null)
            {
                spriteRenderer.sprite = hand_open;
            }
        }
        
    }
}
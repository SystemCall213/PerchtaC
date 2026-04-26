using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Misc
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(DamageOnCollision))]
    public class Grasp : MonoBehaviour
    {
        [Inject] private PlayerMovement playerMovement;
        
        [SerializeField] private float force = 10f;
        [SerializeField] private float lifetime = 30f;
        [SerializeField] private float grabDuration = 5f;
        [SerializeField] private float sineFrequency = 2f;
        [SerializeField] private float sineMagnitude = 5f;
        [SerializeField] private Rigidbody2D hold;
        
        private Rigidbody2D rb;
        private bool isGrabbing = true;
        
        public void StopGrabbing()
        {
            isGrabbing = false;
            Destroy(gameObject, 10f);
        }
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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
            Vector2 playerPos = playerMovement.transform.position;
            Vector2 currentPos = transform.position;
            Vector2 direction = (playerPos - currentPos).normalized;
            
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            
            float offset = Mathf.Sin(Time.time * sineFrequency) * sineMagnitude;
            
            Vector2 finalDirection = (direction * (force * Time.fixedDeltaTime)) + (perpendicular * (offset * Time.fixedDeltaTime));
            rb.AddForce(finalDirection, ForceMode2D.Impulse);
        }

        private void ApplyForceBackwards()
        {
            Vector2 direction = (hold.transform.position - playerMovement.transform.position).normalized;
            hold.AddForce(direction * (force * 400 * Time.fixedDeltaTime), ForceMode2D.Impulse);
        }
    }
}
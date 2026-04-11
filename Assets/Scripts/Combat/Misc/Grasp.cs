using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat.Misc
{
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
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            Destroy(transform.parent.gameObject, lifetime);
            StopGrabbingAfterDelay().Forget();
        }

        private async UniTaskVoid StopGrabbingAfterDelay()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(grabDuration), delayTiming: PlayerLoopTiming.Update, cancellationToken: this.GetCancellationTokenOnDestroy());
            isGrabbing = false;
            Destroy(transform.parent.gameObject, 5f);
        }

        private void Update()
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
            
            Vector2 finalDirection = (direction * force) + (perpendicular * offset);
            rb.AddForce(finalDirection, ForceMode2D.Impulse);
        }

        private void ApplyForceBackwards()
        {
            Vector2 direction = (hold.transform.position - playerMovement.transform.position).normalized;
            hold.AddForce(direction * (force * 400), ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                isGrabbing = false;
                playerHealth.TakeDamage(1); 
                Destroy(transform.parent.gameObject, 10f);
            }
        }
    }
}
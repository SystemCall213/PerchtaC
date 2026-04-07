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
            Destroy(transform.parent.gameObject, 10f);
        }

        private void Update()
        {
            ApplyForceTowardsPlayer();
        }

        private void ApplyForceTowardsPlayer()
        {
            if (!isGrabbing) return;
            var direction = playerMovement.transform.position - transform.position;
            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
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
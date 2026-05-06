using System;
using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using Zenject;

namespace Combat.Misc
{
    public class Worm : MonoBehaviour
    {
        [SerializeField] private float turnSpeed = 90f;
        [SerializeField] private float forwardForce = 10f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float lifetime = 20f;
        [SerializeField] private float chaseTime = 10f;
        
        [Inject] private IPlayerMovement playerMovement;
        
        private Vector2 direction;
        private bool isChasing = true;
        
        private void Awake()
        {
            Destroy(gameObject.transform.parent.gameObject, lifetime);
        }

        private void Start()
        {
            ChasePlayer(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid ChasePlayer(CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(chaseTime), cancellationToken: ct);
            if( !ct.IsCancellationRequested)
            {
                isChasing = false;
            }
            
        }
        
        
        private void FixedUpdate()
        {
            float angle = isChasing ? Vector2.SignedAngle((playerMovement.Position - transform.position.ConvertToVector2()).normalized,transform.up) : 0;
            direction = -transform.up * (forwardForce * (isChasing? Math.Clamp(-Mathf.Cos(angle * Mathf.Deg2Rad), 0, 1) : 1));
            rb.AddTorque(angle * turnSpeed * Time.fixedDeltaTime);
            rb.AddForce(direction, ForceMode2D.Force);
        }
    }
}
using System;
using System.Threading;
using Combat.Interfaces;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Combat.Misc
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Ball : MonoBehaviour
    {
        [Header("Scaling Settings")]
        [SerializeField] private float _scaleUpDuration = 0.5f;
        [SerializeField] private float _scaleDownDuration = 0.5f;
        [SerializeField] private Vector3 _normalScale = Vector3.one;

        [Header("Movement Settings")]
        [SerializeField] private float _impulseForce = 10f;
        [SerializeField] private float _bounceDuration = 5f;
        [SerializeField] private float _delayBetweenBounces = 0.5f;
        [SerializeField] private float _bounceRandomYOffset = 0.1f;

        [Header("Collision Settings")]
        [SerializeField] private LayerMask _wallLayers;

        [Inject] private IPlayerMovement _playerMovement;

        private Rigidbody2D _rb;
        private bool _isActive;
        private CancellationTokenSource _bounceCts;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            transform.localScale = Vector3.zero;
        }

        private void Start()
        {
            InitializeSequenceAsync();
        }

        private void InitializeSequenceAsync()
        {
            ScaleUpAsync();
        }

        private void ScaleUpAsync()
        {
             transform.DOScale(_normalScale, _scaleUpDuration)
                .SetEase(Ease.OutBack).OnComplete(StartBouncingPhase);
        }

        private void StartBouncingPhase()
        {
            _isActive = true;
            ApplyImpulseTowardsPlayer();
            BouncingLifetimeAsync().Forget();
        }

        private async UniTaskVoid BouncingLifetimeAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_bounceDuration), cancellationToken: this.GetCancellationTokenOnDestroy());
            
            if (_isActive)
            {
                EndBouncingPhase();
            }
        }

        private void ApplyImpulseTowardsPlayer()
        {
            if (_playerMovement == null) return;

            Vector2 direction = (_playerMovement.Position - transform.position.ConvertToVector2()).normalized;
            direction.y = Mathf.Sign(UnityEngine.Random.Range(-1,1)) * 1;

            _rb.velocity = Vector2.zero;
            _rb.AddForce(direction * _impulseForce, ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_isActive) return;

            if (IsWall(collision.gameObject))
            {
                HandleWallHit();
            }
        }

        private bool IsWall(GameObject obj)
        {
            return ((1 << obj.layer) & _wallLayers) != 0;
        }

        private void HandleWallHit()
        {
            CancelBounce();

            _rb.velocity = Vector2.zero;
            _bounceCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            WaitAndBounceAsync(_bounceCts.Token).Forget();
        }

        private async UniTaskVoid WaitAndBounceAsync(CancellationToken token)
        {
            bool canceled = await UniTask.Delay(TimeSpan.FromSeconds(_delayBetweenBounces), cancellationToken: token)
                .SuppressCancellationThrow();

            if (!canceled && _isActive)
            {
                ApplyImpulseTowardsPlayer();
                ClearBounceCts();
            }
        }

        private void EndBouncingPhase()
        {
            _isActive = false;
            CancelBounce();

            _rb.velocity = Vector2.zero;
            _rb.isKinematic = true;
            ScaleDownAsync();
        }

        private void ScaleDownAsync()
        {
            transform.DOScale(Vector3.zero, _scaleDownDuration)
                .SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        }

        private void CancelBounce()
        {
            if (_bounceCts != null)
            {
                _bounceCts.Cancel();
                _bounceCts.Dispose();
                _bounceCts = null;
            }
        }

        private void ClearBounceCts()
        {
            if (_bounceCts != null)
            {
                _bounceCts.Dispose();
                _bounceCts = null;
            }
        }

        private void OnDestroy()
        {
            CancelBounce();
        }
    }
}
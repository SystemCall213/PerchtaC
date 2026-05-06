using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Combat.Misc
{
    public class Puncher : MonoBehaviour
    {
        [Header("Telegraph")]
        [SerializeField] private float telegraphDistance = 0.5f;
        [SerializeField] private float telegraphDuration = 1f;
        
        [Header("Attack")]
        [SerializeField] private float delayBeforePunch = 0.5f;
        [SerializeField] private float punchDistance = 2f;
        [SerializeField] private float punchDuration = 0.1f;
        
        [Header("Cleanup")]
        [SerializeField] private float returnDuration = 1f;
        
        [Header("LifeTime")]
        [SerializeField] private float lifeTime = 5f;

        private bool _isPunching;

        [ContextMenu("Punch")]
        private void Start()
        {
            Punch();
            Destroy(gameObject, lifeTime);
        }

        public void Punch()
        {
            PunchAsync().Forget();
        }

        public async UniTask PunchAsync()
        {
            if (_isPunching) return;
            _isPunching = true;

            Vector3 punchStartLocalPos = transform.localPosition;
            var ct = this.GetCancellationTokenOnDestroy();
            try
            {
                Vector3 localForward = transform.localRotation * Vector3.down;

                // 1. Slowly move forward (telegraph)
                await transform.DOLocalMove(punchStartLocalPos + localForward * telegraphDistance, telegraphDuration)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(ct);

                // 2. Wait for the delay
                await UniTask.Delay(TimeSpan.FromSeconds(delayBeforePunch), cancellationToken: ct);

                // 3. Quickly move forward (the attack)
                await transform.DOLocalMove(punchStartLocalPos + localForward * (telegraphDistance + punchDistance), punchDuration)
                    .SetEase(Ease.OutQuad)
                    .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(ct);

                // 4. Slowly move back (cleanup)
                await transform.DOLocalMove(punchStartLocalPos, returnDuration)
                    .SetEase(Ease.InOutQuad)
                    .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation if needed
            }
            finally
            {
                _isPunching = false;
            }
        }
    }
}
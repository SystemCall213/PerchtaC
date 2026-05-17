using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace.Shnaps
{
    public class ShnapsFoundTextLabel : MonoBehaviour
    {
        [SerializeField] private float duration = 2f;     // how long to float before disappearing
        [SerializeField] private float distance = 1f;     // how far up to float in local space
        [SerializeField] private bool fade = true;        // if true and CanvasGroup exists, fade alpha

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            // Try to get CanvasGroup for fading. If none, fading will be skipped.
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Activate()
        {
            // Start the float-and-disappear behaviour and forget it (fire-and-forget).
            // Cancellation will happen automatically when GameObject is destroyed.
            FloatAndDisappear(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid FloatAndDisappear(CancellationToken ct)
        {
            float elapsed = 0f;
            Vector3 start = transform.localPosition;
            Vector3 end = start + Vector3.up * distance;

            // initial alpha preparation if needed
            if (canvasGroup != null && fade)
            {
                canvasGroup.alpha = 1f;
            }

            // animate over duration
            while (elapsed < duration && !ct.IsCancellationRequested)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                transform.localPosition = Vector3.Lerp(start, end, t);

                if (canvasGroup != null && fade)
                {
                    canvasGroup.alpha = 1f - t;
                }

                // yield until next frame, support cancellation
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            // If not canceled, destroy the GameObject (or optionally disable it / return to pool)
            if (!ct.IsCancellationRequested)
            {
                Destroy(gameObject);
            }
        }
    }
}

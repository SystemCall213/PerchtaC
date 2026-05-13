using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image image;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private Tween fadeTween;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (canvas == null) canvas = GetComponentInParent<Canvas>();
            if (image == null) image = GetComponentInChildren<Image>();

            SetAlpha(0f);
            canvas.enabled = false;
            image.raycastTarget = false;
        }

        public UniTask FadeIn()
        {
            canvas.enabled = true;
            image.raycastTarget = true;
            return FadeTo(1f, fadeInDuration);
        }

        public async UniTask FadeOut()
        {
            await FadeTo(0f, fadeOutDuration);

            canvas.enabled = false;
            image.raycastTarget = false;
        }

        private UniTask FadeTo(float alpha, float duration)
        {
            fadeTween?.Kill();

            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
            fadeTween = image.DOFade(alpha, duration)
                .SetUpdate(true)
                .OnComplete(() => completionSource.TrySetResult());

            return completionSource.Task;
        }

        private void SetAlpha(float alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        private void OnDestroy()
        {
            fadeTween?.Kill();
        }
    }
}
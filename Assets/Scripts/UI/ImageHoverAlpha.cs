using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class ImageHoverAlpha : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverAlpha = 1f;
        [SerializeField] private float normalAlpha = 0.5f;
        [SerializeField] private float duration = 0.2f;
        [SerializeField] [Range(0f, 1f)] private float alphaThreshold = 0.1f;

        private Image _image;
        private Tween _fadeTween;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.alphaHitTestMinimumThreshold = alphaThreshold;
            
            // Set initial alpha
            Color color = _image.color;
            color.a = normalAlpha;
            _image.color = color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            FadeTo(hoverAlpha);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            FadeTo(normalAlpha);
        }

        private void FadeTo(float targetAlpha)
        {
            _fadeTween?.Kill();
            _fadeTween = _image.DOFade(targetAlpha, duration);
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
        }
    }
}

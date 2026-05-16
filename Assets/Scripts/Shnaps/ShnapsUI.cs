using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Shnaps
{
    public class ShnapsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform leftBeak;
        [SerializeField] private RectTransform rightBeak;
        [SerializeField] private RectTransform emptyHand;
        [SerializeField] private RectTransform fullHand;
        [SerializeField] private RectTransform shnaps;
        [SerializeField] private RectTransform beaksContainer;

        [Header("Animation Settings")]
        [SerializeField] private float moveDownAmount = 200f;
        [SerializeField] private float moveDuration = 0.5f;
        [SerializeField] private float openRotationAngle = 30f;
        [SerializeField] private float rotateDuration = 0.2f;
        [SerializeField] private int clackCount = 3;
        [SerializeField] private float clackDuration = 0.1f;
        [SerializeField] private float offscreenOffset = 100f;

        private Vector2 _originalBeaksPos;
        private Vector2 _originalShnapsPos;
        private Sequence _currentSequence;
        private CanvasGroup _canvasGroup;
        
        public event System.Action OnShnapsGiven;
        public event System.Action OnNoShnapsGiven;

        private void Awake()
        {
            _originalBeaksPos = beaksContainer.anchoredPosition;
            _originalShnapsPos = shnaps.anchoredPosition;
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            _currentSequence?.Kill();
        }

        public void PlayGivingShnapsAnimation()
        {
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence().SetLink(gameObject);

            emptyHand.gameObject.SetActive(false);
            fullHand.gameObject.SetActive(true);
            shnaps.gameObject.SetActive(true);
            
            _canvasGroup.blocksRaycasts = true;
            _currentSequence.Append(_canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutSine));

            // Move down
            _currentSequence.Append(beaksContainer.DOAnchorPosY(_originalBeaksPos.y - moveDownAmount, moveDuration));

            // Open beaks
            _currentSequence.Append(leftBeak.DORotate(new Vector3(0, 0, openRotationAngle), rotateDuration));
            _currentSequence.Join(rightBeak.DORotate(new Vector3(0, 0, -openRotationAngle), rotateDuration));

            // Close beaks
            _currentSequence.Append(leftBeak.DORotate(Vector3.zero, rotateDuration));
            _currentSequence.Join(rightBeak.DORotate(Vector3.zero, rotateDuration));

            // Move back up with shnaps
            _currentSequence.Append(beaksContainer.DOAnchorPosY(_originalBeaksPos.y, moveDuration));
            _currentSequence.Join(shnaps.DOAnchorPosY(_originalShnapsPos.y, moveDuration)).OnComplete(() => OnShnapsGiven?.Invoke()); 
            _currentSequence.Append(_canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutSine)).OnComplete(() => _canvasGroup.blocksRaycasts = false);
        }

        public void PlayNoShnapsAnimation()
        {
            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence().SetLink(gameObject);

            emptyHand.gameObject.SetActive(true);
            fullHand.gameObject.SetActive(false);
            shnaps.gameObject.SetActive(false);
            _canvasGroup.blocksRaycasts = true;
            
            _currentSequence.Append(_canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutSine));
            // Move down
            _currentSequence.Append(beaksContainer.DOAnchorPosY(_originalBeaksPos.y - moveDownAmount/2, moveDuration));

            // Aggressive clacking
            for (int i = 0; i < clackCount; i++)
            {
                _currentSequence.Append(leftBeak.DORotate(new Vector3(0, 0, openRotationAngle), clackDuration));
                _currentSequence.Join(rightBeak.DORotate(new Vector3(0, 0, -openRotationAngle), clackDuration));
                _currentSequence.Append(leftBeak.DORotate(Vector3.zero, clackDuration));
                _currentSequence.Join(rightBeak.DORotate(Vector3.zero, clackDuration));
            }
            _currentSequence.Append(beaksContainer.DOAnchorPosY(_originalBeaksPos.y - moveDownAmount, moveDuration));
            _currentSequence.Append(leftBeak.DORotate(new Vector3(0, 0, openRotationAngle), clackDuration));
            _currentSequence.Join(rightBeak.DORotate(new Vector3(0, 0, -openRotationAngle), clackDuration));
            
            // Move back up
            _currentSequence.Append(beaksContainer.DOAnchorPosY(_originalBeaksPos.y + offscreenOffset, moveDuration));
            _currentSequence.Join(emptyHand.DOAnchorPosY(_originalShnapsPos.y + offscreenOffset, moveDuration)).OnComplete(() =>
            {
                OnNoShnapsGiven?.Invoke();
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutSine).SetDelay(2);
            });
        }
    }
}
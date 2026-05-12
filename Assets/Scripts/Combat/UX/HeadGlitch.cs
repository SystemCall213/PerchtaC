using DG.Tweening;
using UnityEngine;

namespace Combat.UX
{
    public class HeadGlitch : MonoBehaviour
    {
        [Header("Slow Sway")]
        [SerializeField] private Vector3 slowRotation = new Vector3(0f, 0f, 4f);
        [SerializeField] private float slowDuration = 1.5f;

        [Header("Intense Glitch")]
        [SerializeField] private Vector2 randomDelayRange = new Vector2(2f, 5f);
        [SerializeField] private Vector3 intenseRotation = new Vector3(8f, 6f, 10f);
        [SerializeField] private float intenseStepDuration = 0.06f;
        [SerializeField] private int intenseShakeCount = 6;

        private Quaternion _startRotation;
        private Sequence _slowSequence;
        private Sequence _glitchSequence;

        private void OnEnable()
        {
            _startRotation = transform.localRotation;
            StartSlowSway();
            ScheduleNextGlitch();
        }

        private void OnDisable()
        {
            _slowSequence?.Kill();
            _glitchSequence?.Kill();
            transform.localRotation = _startRotation;
        }

        private void StartSlowSway()
        {
            _slowSequence?.Kill();
            transform.localRotation = _startRotation;

            _slowSequence = DOTween.Sequence()
                .Append(transform.DOLocalRotateQuaternion(_startRotation * Quaternion.Euler(slowRotation), slowDuration).SetEase(Ease.InOutSine))
                .Append(transform.DOLocalRotateQuaternion(_startRotation * Quaternion.Euler(-slowRotation), slowDuration * 2f).SetEase(Ease.InOutSine))
                .Append(transform.DOLocalRotateQuaternion(_startRotation, slowDuration).SetEase(Ease.InOutSine))
                .SetLoops(-1);
        }

        private void ScheduleNextGlitch()
        {
            _glitchSequence?.Kill();

            _glitchSequence = DOTween.Sequence()
                .AppendInterval(Random.Range(randomDelayRange.x, randomDelayRange.y))
                .AppendCallback(PlayIntenseGlitch);
        }

        private void PlayIntenseGlitch()
        {
            _slowSequence?.Pause();
            _glitchSequence = DOTween.Sequence();

            for (int i = 0; i < intenseShakeCount; i++)
            {
                Vector3 rotation = i % 2 == 0 ? intenseRotation : -intenseRotation;
                _glitchSequence.Append(transform.DOLocalRotateQuaternion(_startRotation * Quaternion.Euler(rotation), intenseStepDuration).SetEase(Ease.Linear));
            }

            _glitchSequence
                .Append(transform.DOLocalRotateQuaternion(_startRotation, intenseStepDuration).SetEase(Ease.Linear))
                .AppendCallback(() =>
                {
                    _slowSequence?.Restart();
                    ScheduleNextGlitch();
                });
        }
    }
}
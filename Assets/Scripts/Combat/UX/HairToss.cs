using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Combat.UX
{
    public class HairToss : MonoBehaviour
    {
        [SerializeField] private float idleAngle = 1f;
        [SerializeField] private float idleDuration = 1.5f;
        [SerializeField] private float angle = 4f;
        [SerializeField] private float duration = 0.6f;
        [SerializeField] private float minDelay = 2f;
        [SerializeField] private float maxDelay = 3f;
        [SerializeField] private Ease ease = Ease.InOutSine;

        private Tween tween;
        private Coroutine routine;
        private Quaternion initialLocalRotation;

        private void OnEnable()
        {
            initialLocalRotation = transform.localRotation;
            StartIdleTween();
            routine = StartCoroutine(TossRoutine());
        }

        private void OnDisable()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            tween?.Kill();
            tween = null;
            transform.localRotation = initialLocalRotation;
        }

        private IEnumerator TossRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minDelay, Mathf.Max(minDelay, maxDelay)));

                tween?.Kill();
                tween = DOTween.Sequence()
                    .Append(transform.DOLocalRotateQuaternion(initialLocalRotation * Quaternion.Euler(0f, 0f, angle), duration * 0.5f))
                    .Append(transform.DOLocalRotateQuaternion(initialLocalRotation * Quaternion.Euler(0f, 0f, -angle), duration))
                    .Append(transform.DOLocalRotateQuaternion(initialLocalRotation, duration * 0.5f))
                    .SetEase(ease);

                yield return tween.WaitForCompletion();
                StartIdleTween();
            }
        }

        private void StartIdleTween()
        {
            tween?.Kill();
            tween = DOTween.Sequence()
                .Append(transform.DOLocalRotateQuaternion(initialLocalRotation * Quaternion.Euler(0f, 0f, idleAngle), idleDuration * 0.5f))
                .Append(transform.DOLocalRotateQuaternion(initialLocalRotation * Quaternion.Euler(0f, 0f, -idleAngle), idleDuration))
                .Append(transform.DOLocalRotateQuaternion(initialLocalRotation, idleDuration * 0.5f))
                .SetEase(ease)
                .SetLoops(-1);
        }
    }
}
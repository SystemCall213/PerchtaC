using DG.Tweening;
using UnityEngine;

namespace Combat.UX
{
    public class TeethClacking : MonoBehaviour
    {
        [SerializeField] private GameObject TeethUp;
        [SerializeField] private GameObject TeethDown;
        [SerializeField] private float minDelay = 1f;
        [SerializeField] private float maxDelay = 3f;
        [SerializeField] private float rotationAngle = 15f;
        [SerializeField] private float rotationDuration = 0.08f;
        [SerializeField] private float idleRotationAngle = 5f;
        [SerializeField] private float idleRotationDuration = 0.5f;
        [SerializeField] private float glitchRotationAngle = 3f;
        [SerializeField] private float glitchRotationDuration = 0.025f;
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;

        private Quaternion _teethUpStartRotation;
        private Quaternion _teethDownStartRotation;
        private Sequence _clackingSequence;
        private Sequence _idleSequence;
        private bool _isClackingInitialized;

        private void OnEnable()
        {
            if (TeethUp == null || TeethDown == null)
                return;

            _teethUpStartRotation = TeethUp.transform.localRotation;
            _teethDownStartRotation = TeethDown.transform.localRotation;
            _isClackingInitialized = true;

            PlayClackingLoop();
        }

        private void OnDisable()
        {
            _clackingSequence?.Kill();
            _clackingSequence = null;
            _idleSequence?.Kill();
            _idleSequence = null;

            if (!_isClackingInitialized)
                return;

            if (TeethUp != null)
                TeethUp.transform.localRotation = _teethUpStartRotation;

            if (TeethDown != null)
                TeethDown.transform.localRotation = _teethDownStartRotation;

            _isClackingInitialized = false;
        }

        private void PlayClackingLoop()
        {
            _clackingSequence?.Kill();
            PlayIdleRotation();

            float delay = Random.Range(Mathf.Min(minDelay, maxDelay), Mathf.Max(minDelay, maxDelay));
            Vector3 normalizedAxis = rotationAxis == Vector3.zero ? Vector3.forward : rotationAxis.normalized;
            Quaternion teethUpTargetRotation = _teethUpStartRotation * Quaternion.AngleAxis(rotationAngle, normalizedAxis);
            Quaternion teethDownTargetRotation = _teethDownStartRotation * Quaternion.AngleAxis(-rotationAngle, normalizedAxis);
            Quaternion teethUpGlitchForwardRotation = _teethUpStartRotation * Quaternion.AngleAxis(rotationAngle + glitchRotationAngle, normalizedAxis);
            Quaternion teethDownGlitchForwardRotation = _teethDownStartRotation * Quaternion.AngleAxis(-rotationAngle - glitchRotationAngle, normalizedAxis);
            Quaternion teethUpGlitchBackRotation = _teethUpStartRotation * Quaternion.AngleAxis(rotationAngle - glitchRotationAngle, normalizedAxis);
            Quaternion teethDownGlitchBackRotation = _teethDownStartRotation * Quaternion.AngleAxis(-rotationAngle + glitchRotationAngle, normalizedAxis);

            _clackingSequence = DOTween.Sequence()
                .AppendInterval(delay)
                .AppendCallback(StopIdleRotation)
                .Append(TeethUp.transform.DOLocalRotateQuaternion(teethUpTargetRotation, rotationDuration))
                .Join(TeethDown.transform.DOLocalRotateQuaternion(teethDownTargetRotation, rotationDuration))
                .Append(TeethUp.transform.DOLocalRotateQuaternion(teethUpGlitchForwardRotation, glitchRotationDuration))
                .Join(TeethDown.transform.DOLocalRotateQuaternion(teethDownGlitchForwardRotation, glitchRotationDuration))
                .Append(TeethUp.transform.DOLocalRotateQuaternion(teethUpGlitchBackRotation, glitchRotationDuration))
                .Join(TeethDown.transform.DOLocalRotateQuaternion(teethDownGlitchBackRotation, glitchRotationDuration))
                .Append(TeethUp.transform.DOLocalRotateQuaternion(teethUpTargetRotation, glitchRotationDuration))
                .Join(TeethDown.transform.DOLocalRotateQuaternion(teethDownTargetRotation, glitchRotationDuration))
                .Append(TeethUp.transform.DOLocalRotateQuaternion(_teethUpStartRotation, rotationDuration))
                .Join(TeethDown.transform.DOLocalRotateQuaternion(_teethDownStartRotation, rotationDuration))
                .OnComplete(PlayClackingLoop);
        }

        private void PlayIdleRotation()
        {
            _idleSequence?.Kill();

            Vector3 normalizedAxis = rotationAxis == Vector3.zero ? Vector3.forward : rotationAxis.normalized;
            Quaternion teethUpIdleRotation = _teethUpStartRotation * Quaternion.AngleAxis(idleRotationAngle, normalizedAxis);
            Quaternion teethDownIdleRotation = _teethDownStartRotation * Quaternion.AngleAxis(-idleRotationAngle, normalizedAxis);

            _idleSequence = DOTween.Sequence()
                .Append(TeethUp.transform.DOLocalRotateQuaternion(teethUpIdleRotation, idleRotationDuration))
                .Join(TeethDown.transform.DOLocalRotateQuaternion(teethDownIdleRotation, idleRotationDuration))
                .Append(TeethUp.transform.DOLocalRotateQuaternion(_teethUpStartRotation, idleRotationDuration))
                .Join(TeethDown.transform.DOLocalRotateQuaternion(_teethDownStartRotation, idleRotationDuration))
                .SetLoops(-1);
        }

        private void StopIdleRotation()
        {
            _idleSequence?.Kill();
            _idleSequence = null;

            TeethUp.transform.localRotation = _teethUpStartRotation;
            TeethDown.transform.localRotation = _teethDownStartRotation;
        }
    }
}
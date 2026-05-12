using DG.Tweening;
using UnityEngine;

namespace Combat.Misc
{
    public class RopeStrech : MonoBehaviour
    {
        [SerializeField] private float strechTime = 1f;
        [SerializeField] private float endScale = 1f;
        
        private float startScale;
        private void Start()
        {
            startScale = transform.localScale.y;
            StrechOut();
        }
        private void StrechOut()
        {
            Sequence fallSequence = DOTween.Sequence();
            fallSequence.Append(transform.DOScaleY(endScale, strechTime));
            fallSequence.Join(transform.DOScaleX(1/endScale, strechTime).OnComplete(StrechIn));
            fallSequence.Play();
        }

        private void StrechIn()
        {
            Sequence fallSequence = DOTween.Sequence();
            fallSequence.Append(transform.DOScaleY(startScale, strechTime));
            fallSequence.Join(transform.DOScaleX(startScale, strechTime));
            fallSequence.Play();
        }
    }
}
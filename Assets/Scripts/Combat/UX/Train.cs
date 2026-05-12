using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Combat.UX
{
    public class Train : MonoBehaviour
    {
        [SerializeField] private GameObject[] trainParts;
        [SerializeField] private float waveHeight = 0.5f;
        [SerializeField] private float moveDuration = 0.25f;
        [SerializeField] private float delayBetweenParts = 0.1f;
        [SerializeField] private float delayBetweenLoops = 0.5f;
        [SerializeField] private Ease ease = Ease.InOutSine;

        private Sequence waveSequence;

        private void OnEnable()
        {
            PlayWave();
        }

        private void OnDisable()
        {
            waveSequence?.Kill();
            waveSequence = null;
        }

        private void PlayWave()
        {
            waveSequence?.Kill();

            if (trainParts == null || trainParts.Length == 0)
                return;

            waveSequence = DOTween.Sequence();

            for (int i = 0; i < trainParts.Length; i++)
            {
                if (trainParts[i] == null)
                    continue;

                Transform trainPart = trainParts[i].transform;
                float startY = trainPart.localPosition.y;

                waveSequence.Insert(
                    i * delayBetweenParts,
                    trainPart.DOLocalMoveY(startY + waveHeight, moveDuration)
                        .SetLoops(2, LoopType.Yoyo)
                        .SetEase(ease));
            }

            waveSequence.AppendInterval(delayBetweenLoops);
            waveSequence.SetLoops(-1, LoopType.Restart);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using CoreLoop.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace DefaultNamespace.Cinematics
{
    public class CreditsCinematic : MonoBehaviour
    {
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly DefaultActions defaultActions;
        [SerializeField] private Image spriteRenderer1;
        [SerializeField] private Image spriteRenderer2;
        [SerializeField] private List<Sprite> frames;
        [SerializeField] private float frameDelay = 2f;
        
        private CancellationTokenSource _cts;
        
        private void OnEnable()
        {
            defaultActions.UI.SkipCinematic.performed += SkipCinematic;
        }

        private void OnDisable()
        {
            defaultActions.UI.SkipCinematic.performed -= SkipCinematic;
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private void Start()
        {
            _cts = new CancellationTokenSource();
            StartCinematic(_cts.Token).Forget();
        }

        private async UniTaskVoid StartCinematic(CancellationToken ct)
        {
            foreach (var frame in frames)
            {
                spriteRenderer1.sprite = frame;
                await UniTask.Delay(TimeSpan.FromSeconds(frameDelay), cancellationToken: ct);
            }

            spriteRenderer2.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(frameDelay), cancellationToken: ct);

            await SpriteMove(ct);
            
            OnAnimationComplete();
        }

        private async UniTask SpriteMove(CancellationToken ct)
        {
            await spriteRenderer2.transform.DOLocalMoveY(spriteRenderer2.transform.localPosition.y - 3240, 3f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(ct);
        }

        private void OnAnimationComplete()
        {
            sceneLoader.LoadCreditsScene();
        }

        private void SkipCinematic(InputAction.CallbackContext obj)
        {
            _cts?.Cancel();
            sceneLoader.LoadCreditsScene();
        }
    }
}
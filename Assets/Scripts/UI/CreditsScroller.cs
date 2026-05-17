using System;
using CoreLoop.Interfaces;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UI
{
    public class CreditsScroller : MonoBehaviour
    {
        [Inject] private ISceneLoader sceneLoader;
        [Inject] private readonly DefaultActions defaultActions;
        
        [SerializeField] private float duration = 10f;
        [SerializeField] private float topPoint;

        private void OnEnable()
        {
            defaultActions.UI.SkipCinematic.performed += Skip;
        }

        private void OnDisable()
        {
            defaultActions.UI.SkipCinematic.performed -= Skip;
        }
        private void Start()
        {
            transform.DOLocalMoveY(topPoint, duration).SetEase(Ease.Linear).OnComplete(() => {;
                sceneLoader.LoadMainMenu();
            });
        }

        private void Skip(InputAction.CallbackContext obj)
        {
            sceneLoader.LoadMainMenu();
        }
    }
}
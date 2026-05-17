using System.Collections.Generic;
using CoreLoop.Interfaces;
using DG.Tweening;
using Unity.VisualScripting.ReorderableList;
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
        
        private int currentFrameIndex = 0;
        
        private void OnEnable()
        {
            defaultActions.UI.SkipCinematic.performed += SkipCinematic;
        }

        private void OnDisable()
        {
            defaultActions.UI.SkipCinematic.performed -= SkipCinematic;
        }

        public void Clicked()
        {
            // please don't read this code, it's a mess and I don't want to look at it again
            if (currentFrameIndex > frames.Count + 1) return;
            
            if (currentFrameIndex == frames.Count + 1)
            {
                currentFrameIndex++;
                SpriteMove();   
                return;
            }
            
            if (currentFrameIndex == frames.Count)
            {
                spriteRenderer2.gameObject.SetActive(true);
                currentFrameIndex++;
            }
            else
            {
                spriteRenderer1.sprite = frames[currentFrameIndex++];
            }
        }

        private void SpriteMove()
        {
            
            spriteRenderer2.transform.DOLocalMoveY(spriteRenderer2.transform.localPosition.y - 3240, 3f).SetEase(Ease.Linear).OnComplete(OnAnimationComplete);
        }

        private void OnAnimationComplete()
        {
            sceneLoader.LoadCreditsScene();
        }

        private void SkipCinematic(InputAction.CallbackContext obj)
        {
            sceneLoader.LoadCreditsScene();
        }
    }
}
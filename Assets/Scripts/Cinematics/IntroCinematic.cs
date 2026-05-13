using System;
using CoreLoop.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace DefaultNamespace.Cinematics
{
    public class IntroCinematic : MonoBehaviour
    {
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly DefaultActions defaultActions;
        [SerializeField] private Animator animator;
        
        private bool hasCompleted;
        
        private void OnEnable()
        {
            defaultActions.UI.SkipCinematic.performed += SkipCinematic;
        }

        private void OnDisable()
        {
            defaultActions.UI.SkipCinematic.performed -= SkipCinematic;
        }

        private void Update()
        {
            if (hasCompleted) return;
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    
            // Check if animation has finished playing
            if (stateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
            {
                hasCompleted = true;
                OnAnimationComplete();
            }
        }

        private void OnAnimationComplete()
        {
            sceneLoader.LoadNextLevel();
        }

        private void SkipCinematic(InputAction.CallbackContext obj)
        {
            hasCompleted = true;
            sceneLoader.LoadNextLevel();
        }
    }
}
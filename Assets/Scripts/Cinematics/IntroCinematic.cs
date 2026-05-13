using CoreLoop.Interfaces;
using UnityEngine;
using Zenject;

namespace DefaultNamespace.Cinematics
{
    public class IntroCinematic : MonoBehaviour
    {
        [Inject] private readonly ISceneLoader sceneLoader;
        [SerializeField] private Animator animator;
        
        private bool hasCompleted;
        
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
    }
}
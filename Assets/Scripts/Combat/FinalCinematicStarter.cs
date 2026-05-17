using System;
using CoreLoop.Interfaces;
using Dialogue.Interfaces;
using UnityEngine;
using Zenject;

namespace Combat
{
    public class FinalCinematicStarter : MonoBehaviour
    {
        [Inject] private ISceneLoader sceneLoader;
        [Inject] private IDialogueManager dialogueManager;
        
        [SerializeField, Scene] private string finalCinematicScene;

        private void OnEnable()
        {
            dialogueManager.OnDialogueExited += StartFinalCinematic;
        }

        private void OnDisable()
        {
            dialogueManager.OnDialogueExited -= StartFinalCinematic;
        }

        private void StartFinalCinematic()
        {
            sceneLoader.LoadGivenLevel(finalCinematicScene);
        }
    }
}
using System;
using CoreLoop.Interfaces;
using CoreLoop.States;
using Dialogue;
using Interfaces;
using UnityEngine;
using Zenject;

public class DialogueStarter : MonoBehaviour
{
    [Inject] private readonly IGameStateMachine gameStateMachine;
    [Inject] private readonly DialogueState.Factory dialogueStateFactory;
    [Inject] private readonly ISceneLoader sceneLoader;
    [Inject(Id = "InitialDialogue")] private readonly DialogueSO initialDialogueSO;
    [Inject(Id = "CleanedRoomDialogue")] private readonly DialogueSO cleanedRoomDialogueSO;
    private void Start()
    {
        if (initialDialogueSO)
            gameStateMachine.ChangeState(dialogueStateFactory.Create(initialDialogueSO));
    }

    private void OnEnable()
    {
        sceneLoader.BattleEnded += StartCleanedRoomDialogue;
    }
    
    private void OnDisable()
    {
        sceneLoader.BattleEnded -= StartCleanedRoomDialogue;
    }

    public void StartCleanedRoomDialogue()
    {
        if (cleanedRoomDialogueSO)
            gameStateMachine.ChangeState(dialogueStateFactory.Create(cleanedRoomDialogueSO));
    }
}

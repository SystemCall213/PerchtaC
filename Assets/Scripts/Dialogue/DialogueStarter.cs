using CoreLoop.Interfaces;
using CoreLoop.States;
using Dialogue;
using UnityEngine;
using Zenject;

public class DialogueStarter : MonoBehaviour
{
    [Inject] private readonly IGameStateMachine gameStateMachine;
    [Inject] private readonly DialogueState.Factory dialogueStateFactory;
    [Inject] private readonly DialogueSO dialogueSo;
    private void Start()
    {
        gameStateMachine.ChangeState(dialogueStateFactory.Create(dialogueSo));
    }
}

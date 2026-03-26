using CoreLoop.Interfaces;
using Dialogue;
using Zenject;

namespace CoreLoop.States
{
    public class LoadLevel : State
    {
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly IGameStateMachine gameStateMachine;
        [Inject] private readonly DialogueState.Factory dialogueStateFactory;
        [Inject(Id = "InitialDialogue")] private readonly DialogueSO dialogue;
        public override void Enter()
        {
            sceneLoader.LoadNextLevel();
            gameStateMachine.ChangeState(dialogueStateFactory.Create(dialogue));
        }

        public override void Exit()
        {
            
        }

        public class Factory : PlaceholderFactory<LoadLevel> { }
    }
}
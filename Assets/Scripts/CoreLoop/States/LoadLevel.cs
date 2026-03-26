using CoreLoop.Interfaces;
using Dialogue;
using Zenject;

namespace CoreLoop.States
{
    public class LoadLevel : State
    {
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly GameStateMachine gameStateMachine;
        [Inject(Id = "InitialDialogue")] private readonly DialogueSO dialogue;
        public override void Enter()
        {
            sceneLoader.LoadNextLevel();
            gameStateMachine.ChangeState(new DialogueState(dialogue));
        }

        public override void Exit()
        {
            
        }
    }
}
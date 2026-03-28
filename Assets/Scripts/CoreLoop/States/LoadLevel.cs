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
        public override void Enter()
        {
            sceneLoader.LoadNextLevel();
        }

        public override void Exit()
        {
            
        }

        public class Factory : PlaceholderFactory<LoadLevel> { }
    }
}
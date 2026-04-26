using CoreLoop.Interfaces;
using Dialogue;
using Zenject;

namespace CoreLoop.States
{
    public class LoadNextLevel : State
    {
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly IGameStateMachine gameStateMachine;
        public override void Enter()
        {
            sceneLoader.LoadNextLevel();
        }

        public override void Exit()
        {
            
        }

        public class Factory : PlaceholderFactory<LoadNextLevel> { }
    }
}
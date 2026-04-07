using CoreLoop.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class LoadGivenLevel : State<string>
    {
        private readonly ISceneLoader sceneLoader;
        
        [Inject]
        public LoadGivenLevel(string levelName, ISceneLoader sceneLoader)
        {
            Payload = levelName;
            this.sceneLoader = sceneLoader;
        }

        public override void Enter()
        {
            sceneLoader.LoadGivenLevel(Payload);
        }

        public override void Exit()
        {
        }
        public class Factory : PlaceholderFactory<string, LoadGivenLevel> { }
    }
}
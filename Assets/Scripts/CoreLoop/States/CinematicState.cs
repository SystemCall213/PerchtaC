using CoreLoop.Interfaces;
using UI.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class CinematicState : State<string>
    {
        private readonly ISceneLoader sceneLoader;
        private readonly DefaultActions defaultActions;

        [Inject]
        public CinematicState(string payload, ISceneLoader sceneLoader, DefaultActions defaultActions)
        {
            Payload = payload;
            this.sceneLoader = sceneLoader;
            this.defaultActions = defaultActions;
        }

        public override void Enter()
        {
            sceneLoader.LoadCinematicScene(Payload);
            defaultActions.UI.CloseMenu.Disable();
            defaultActions.UI.SkipCinematic.Enable();
        }

        public override void Exit()
        {
            defaultActions.UI.CloseMenu.Enable();
            defaultActions.UI.SkipCinematic.Disable();
        }

        public class Factory : PlaceholderFactory<string, CinematicState> { }
    }
}
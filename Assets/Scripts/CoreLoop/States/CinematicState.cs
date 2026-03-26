using CoreLoop.Interfaces;
using UI.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class CinematicState : State<string>
    {
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly DefaultActions defaultActions;

        public CinematicState(string payload)
        {
            Payload = payload;
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
    }
}
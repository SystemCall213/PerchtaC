using CoreLoop.Interfaces;
using UI.Interfaces;
using Zenject;

namespace CoreLoop.States
{
    public class CinematicState : State<string>
    {
        public string Payload { get; set; }
        [Inject] private readonly ISceneLoader sceneLoader;
        [Inject] private readonly DefaultActions defaultActions;
        protected override void Enter(string payload)
        {
            
        }

        protected override void Exit(string payload)
        {
            throw new System.NotImplementedException();
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
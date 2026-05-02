using Audio;
using CoreLoop.Interfaces;
using CoreLoop.StatePayload;
using CoreLoop.States;
using Interfaces;
using Zenject;

namespace CoreLoop
{
    public class GameStateMachineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindFactory<CinematicState, CinematicState.Factory>();
            Container.BindFactory<MainMenuState, MainMenuState.Factory>();
            Container.BindFactory<CombatStatePayload, CombatState, CombatState.Factory>();
            Container.BindFactory<RoomStatePayload, RoomState, RoomState.Factory>();
            Container.BindFactory<DialogueSO, DialogueState, DialogueState.Factory>();
            
            Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle().NonLazy();
        }
    }
}
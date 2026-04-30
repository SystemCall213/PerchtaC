using CoreLoop.Interfaces;
using CoreLoop.States;
using Interfaces;
using Zenject;

namespace CoreLoop
{
    public class GameStateMachineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindFactory<string, CinematicState, CinematicState.Factory>();
            Container.BindFactory<LoadNextLevel, LoadNextLevel.Factory>();
            Container.BindFactory<MainMenuState, MainMenuState.Factory>();
            Container.BindFactory<string, CombatState, CombatState.Factory>();
            Container.BindFactory<string, LoadGivenLevel, LoadGivenLevel.Factory>();
            Container.BindFactory<RoomState, RoomState.Factory>();
            Container.BindFactory<DialogueSO, DialogueState, DialogueState.Factory>();
            
            Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
        }
    }
}
using CoreLoop;
using CoreLoop.Interfaces;
using CoreLoop.States;
using Dialogue;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField, Scene] private string[] levels;
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.Bind<DefaultActions>().AsSingle();
        Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle().WithArguments(levels);
        

        Container.BindFactory<string, CinematicState, CinematicState.Factory>();
        Container.BindFactory<DialogueSO, DialogueState, DialogueState.Factory>();
        Container.BindFactory<LoadNextLevel, LoadNextLevel.Factory>();
        Container.BindFactory<MainMenuState, MainMenuState.Factory>();
        Container.BindFactory<string, CombatState, CombatState.Factory>();
        Container.BindFactory<string, LoadGivenLevel, LoadGivenLevel.Factory>();
        Container.BindFactory<RoomState, RoomState.Factory>();
        
        Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
        Container.Bind<DialogueManager>().AsSingle();
    }
}

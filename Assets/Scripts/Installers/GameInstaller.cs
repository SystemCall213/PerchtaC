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
        Container.Bind<DefaultActions>().AsSingle();
        Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle().WithArguments(levels);
        Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();

        Container.BindFactory<string, CinematicState, CinematicState.Factory>();
        Container.BindFactory<DialogueSO, DialogueState, DialogueState.Factory>();
        Container.BindFactory<LoadLevel, LoadLevel.Factory>();
        Container.BindFactory<MainMenuState, MainMenuState.Factory>();
    }
}

using CoreLoop;
using CoreLoop.Interfaces;
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
    }
}

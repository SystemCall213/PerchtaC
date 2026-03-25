using CoreLoop;
using CoreLoop.Interfaces;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

public class GameInstaller : MonoInstaller
{
    [SerializeField, Scene] private string[] levels;
    public override void InstallBindings()
    {
        Container.Bind<DefaultActions>().AsSingle();
        Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle().WithArguments(levels);
        
    }
}

using Controls;
using CoreLoop;
using CoreLoop.Interfaces;
using Dialogue.Interfaces;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField, Scene] private string[] levels;
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        ControlsInstaller.Install(Container);
        Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle().WithArguments(levels);
        Container.Bind<IDialogueManager>().To<DialogueManager>().AsSingle();
    }
}

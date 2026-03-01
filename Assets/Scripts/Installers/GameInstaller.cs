using Installers;
using Installers.Interfaces;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Inject] private ISceneLoader _sceneLoader;
    public override void InstallBindings()
    {
    }
}

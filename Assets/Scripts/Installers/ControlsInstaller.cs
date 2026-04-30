using System.ComponentModel;
using Zenject;
namespace Controls
{
    public class ControlsInstaller : Installer<ControlsInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<DefaultActions>().AsSingle();
        }
    }
}
using System;
using UI;
using UI.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Installers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private SettingMenu settingMenu;
        public override void InstallBindings()
        {
            Container.Bind<IUIFacade>().To<UIFacade>().AsSingle();
            Container.Bind<SettingMenu>().FromInstance(settingMenu).AsSingle();
        }
    }
}
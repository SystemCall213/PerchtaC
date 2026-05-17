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
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private ShnapsMenu shnapsMenu;
        [SerializeField] private TutorialMenu tutorialMenu;
        public override void InstallBindings()
        {
            Container.Bind<IUIFacade>().To<UIFacade>().AsSingle();
            Container.Bind<SettingMenu>().FromComponentInNewPrefab(settingMenu).AsSingle().NonLazy();
            Container.Bind<PauseMenu>().FromComponentInNewPrefab(pauseMenu).AsSingle().NonLazy();
            Container.Bind<ShnapsMenu>().FromComponentInNewPrefab(shnapsMenu).AsSingle().NonLazy();
            Container.Bind<TutorialMenu>().FromComponentInNewPrefab(tutorialMenu).AsSingle().NonLazy();
        }
    }
}
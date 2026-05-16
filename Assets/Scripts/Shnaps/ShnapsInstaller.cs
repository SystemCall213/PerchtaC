using System;
using DefaultNamespace.Shnaps.Interfaces;
using UnityEngine;
using Zenject;

namespace DefaultNamespace.Shnaps
{
    public class ShnapsInstaller : MonoInstaller
    {
        [SerializeField] private ShnapsUI shnapsUI;
        public override void InstallBindings()
        {
            Container.Bind<IShnapsController>().To<ShnapsController>().AsSingle();
            Container.Bind<ShnapsUI>().FromComponentInNewPrefab(shnapsUI).AsSingle();
            Container.Bind<IShnapsFacade>().To<ShnapsFacade>().AsSingle();
            Container.BindInterfacesAndSelfTo<ShnapsAdapter>().AsSingle().NonLazy();
            
        }
    }
}
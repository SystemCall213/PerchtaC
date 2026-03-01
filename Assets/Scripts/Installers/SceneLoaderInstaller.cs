using System.Collections.Generic;
using Installers.Interfaces;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class SceneLoaderInstaller : MonoInstaller
    {
        [SerializeField] private List<SceneObject> scenesToLoad;
        public override void InstallBindings()
        {
            Container.Bind<ISceneLoader>().FromInstance(new SceneLoader(scenesToLoad)).AsSingle();
        }
    }
}
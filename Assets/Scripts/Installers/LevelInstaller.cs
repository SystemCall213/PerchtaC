using Dialogue;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private DialogueSO initialDialogue;
        
        public override void InstallBindings()
        {
            Container.Bind<DialogueSO>().FromInstance(initialDialogue).AsSingle();
        }
    }
}
using Interfaces;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private DialogueSO initialDialogue;
        [SerializeField] private DialogueSO cleanedRoomDialogue;
        
        public override void InstallBindings()
        {
            Container.Bind<DialogueSO>().WithId("InitialDialogue").FromInstance(initialDialogue);
            Container.Bind<DialogueSO>().WithId("CleanedRoomDialogue").FromInstance(cleanedRoomDialogue);
        }
    }
}
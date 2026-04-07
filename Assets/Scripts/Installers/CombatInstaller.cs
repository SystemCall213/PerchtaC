using Combat;
using Combat.Interfaces;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class CombatInstaller : MonoInstaller
    {
        [SerializeField] private GameObject combatArena;
        [SerializeField] private ScriptableObject combatScenario;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerMovement>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CombatArena>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameObject>().WithId("CombatArena").FromInstance(combatArena).AsSingle();
            
            if (combatScenario is ICombatScenario scenario)
            {
                Container.Bind<ICombatScenario>().FromInstance(scenario).AsSingle();
            }
            
            Container.BindInterfacesAndSelfTo<CombatController>().AsSingle().NonLazy()
                ;
        }
    }
}
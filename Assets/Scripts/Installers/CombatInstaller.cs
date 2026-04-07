using Combat;
using Combat.Interfaces;
using Combat.Misc;
using Glyph;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class CombatInstaller : MonoInstaller
    {
        [SerializeField] private GameObject combatArena;
        [SerializeField] private ScriptableObject combatScenario;
        [SerializeField] private GlyphSOInstaller glyphSOInstaller;
        
        public override void InstallBindings()
        {
            if (glyphSOInstaller != null)
            {
                Container.Inject(glyphSOInstaller);
                glyphSOInstaller.InstallBindings();
                Container.Bind<GlyphFacade>().AsSingle();
                Container.BindInterfacesAndSelfTo<GlyphCompletionTracker>().AsSingle();
            }

            Container.Bind<PlayerHealth>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PlayerMovement>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CombatArena>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameObject>().WithId("CombatArena").FromInstance(combatArena).AsSingle();
            if (combatScenario is ICombatScenario scenario)
            {
                Container.Bind<ICombatScenario>().FromInstance(scenario).AsSingle();
            }
            
            Container.BindInterfacesAndSelfTo<CombatController>().AsSingle().NonLazy();
        }
    }
}
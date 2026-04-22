using Combat;
using Combat.Arena;
using Combat.Interfaces;
using Combat.Misc;
using Glyph;
using Glyph.Glyph_HoldPoint;
using UnityEngine;
using Zenject;
using GlyphFacade = Glyph.GlyphFacade;

namespace Installers
{
    public enum GlyphSystemType
    {
        BlindPainting,
        HoldPoint
    }

    public class CombatInstaller : MonoInstaller
    {
        [SerializeField] private GameObject combatArena;
        [SerializeField] private ScriptableObject combatScenario;
        [SerializeField] private GlyphSystemType glyphSystemType;
        [SerializeField] private GlyphFollower glyphFollowerPrefab;
        [SerializeField] private GlyphLinesSOInstaller glyphLinesSOInstaller;
        [SerializeField] private GlyphSOInstaller glyphSOInstaller;
        
        
        public override void InstallBindings()
        {
            if (glyphSystemType == GlyphSystemType.BlindPainting && glyphSOInstaller != null)
            {
                Container.Inject(glyphSOInstaller);
                glyphSOInstaller.InstallBindings();
                Container.BindInterfacesAndSelfTo<GlyphFacade>().AsSingle();
                Container.BindInterfacesAndSelfTo<GlyphCompletionTracker>().AsSingle();
            }

            if (glyphSystemType == GlyphSystemType.HoldPoint)
            {
                Container.BindInterfacesAndSelfTo<Glyph.Glyph_HoldPoint.GlyphFacade>().AsSingle();
                Container.Bind<GlyphFollower>().FromInstance(glyphFollowerPrefab).AsTransient();
                Container.Inject(glyphLinesSOInstaller);
                glyphLinesSOInstaller.InstallBindings();
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
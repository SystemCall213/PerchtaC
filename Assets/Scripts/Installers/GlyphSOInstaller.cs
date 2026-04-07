using System.Collections.Generic;
using Glyph;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "Glyphs", menuName = "Installers/GlyphSOInstaller", order = 0)]
    public class GlyphSOInstaller : ScriptableObjectInstaller<GlyphSOInstaller>
    {
        [SerializeField] private List<GlyphSO> glyphs;

        public override void InstallBindings()
        {
            Container.BindInstance(glyphs).WithId("AvailableGlyphs").AsSingle();
        }
    }
}
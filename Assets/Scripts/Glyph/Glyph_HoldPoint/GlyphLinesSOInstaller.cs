using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Glyph.Glyph_HoldPoint
{
    [CreateAssetMenu(fileName = "GlyphLinesSOInstaller", menuName = "Glyph/Glyph_HoldPoint/GlyphLinesSOInstaller")]
    public class GlyphLinesSOInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private List<LineRenderer> lines;
        public override void InstallBindings()
        {
            Container.BindInstance(lines).WithId("GlyphLines").AsSingle();
        }
        
    }
}
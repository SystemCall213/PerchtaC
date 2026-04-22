using System;
using Glyph.Interfaces;

namespace Glyph.Glyph_HoldPoint
{
    public class GlyphFacade : IGlyphFacade
    {
        public event Action<int> OnGlyphPainted;
        
        public void TriggerGlyphPainted()
        {
            OnGlyphPainted?.Invoke(1);
        }
    }
}
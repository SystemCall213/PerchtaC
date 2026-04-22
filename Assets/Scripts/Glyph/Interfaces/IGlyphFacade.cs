using System;

namespace Glyph.Interfaces
{
    public interface IGlyphFacade
    {
        event Action<int> OnGlyphPainted;
        void TriggerGlyphPainted();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Glyph
{
    public class GlyphFacade
    {
        private readonly List<GlyphSO> _allGlyphs;
        private List<GlyphSO> _shuffledGlyphs;
        private int _currentIndex;

        public event Action<int> OnGlyphPainted;

        public GlyphFacade([Inject(Id = "AvailableGlyphs")] List<GlyphSO> glyphs)
        {
            _allGlyphs = glyphs;
            Reshuffle();
        }

        private void Reshuffle()
        {
            _shuffledGlyphs = _allGlyphs.OrderBy(_ => Guid.NewGuid()).ToList();
            _currentIndex = 0;
        }

        public GlyphSO GetNextGlyph()
        {
            if (_shuffledGlyphs == null || _shuffledGlyphs.Count == 0) return null;

            if (_currentIndex >= _shuffledGlyphs.Count)
            {
                Reshuffle();
            }

            return _shuffledGlyphs[_currentIndex++];
        }

        public void TriggerGlyphPainted() => OnGlyphPainted?.Invoke(1);
    }
}
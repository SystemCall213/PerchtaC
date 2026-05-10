using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Zenject;
using System.Linq;

namespace Combat
{
    public class SpawnerPositionSelector
    {
        [Inject(Id = "Spawn_Position")] private List<Transform> _spawners;

        private List<int> _availableIndices = new List<int>();
        private int _currentIndex;

        public Vector3 GetNextPosition()
        {
            if (_spawners == null || _spawners.Count == 0)
            {
                Debug.LogWarning("No spawners found with ID 'Spawn_Position'");
                return Vector3.zero;
            }

            if (_availableIndices.Count == 0 || _currentIndex >= _availableIndices.Count)
            {
                _availableIndices = Enumerable.Range(0, _spawners.Count).ToList().Shuffle();
                _currentIndex = 0;
            }

            int spawnerIndex = _availableIndices[_currentIndex++];
            return _spawners[spawnerIndex].position;
        }
    }
}
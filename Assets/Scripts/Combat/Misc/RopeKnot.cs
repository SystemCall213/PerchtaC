using System;
using Combat.Arena;
using UnityEngine;

namespace Combat.Misc
{
    public class RopeKnot : MonoBehaviour
    {
        [SerializeField] private float lifetime = 10f;
        [SerializeField] private GameObject rope;
        [SerializeField] private int radialDelta = 5;
        [SerializeField] private float delayBetweenSpawns = 0.5f;
        
        private int currentRadialIndex = 0;
        
    }
}
using System;
using Audio;
using CoreLoop.Interfaces;
using UnityEngine;

namespace CoreLoop.StatePayload
{
    [Serializable]
    public class RoomStatePayload : IStatePayload
    {
        [SerializeField] private MusicId musicId;
        public MusicId MusicId => musicId;
        
    }
}
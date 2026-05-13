using System;
using FMOD.Studio;
using FMODUnity;
using StudioStopMode = FMOD.Studio.STOP_MODE;

namespace Audio
{
    public enum MusicDestinationMarker
    {
        Room,
        Boss
    }

    public interface IMusicService
    {
        void Request(MusicId id);
        void RequestIfNotPlaying(MusicId id, MusicDestinationMarker destinationMarker);
        void SetIsInRoom(bool isInRoom);
    }

    public sealed class MusicService : IMusicService, IDisposable
    {
        private readonly MusicCatalog catalog;

        private EventInstance current;
        private MusicId? currentId;

        public MusicService(MusicCatalog catalog)
        {
            this.catalog = catalog;
        }

        public void Request(MusicId id)
        {
            Request(id, null);
        }

        public void RequestIfNotPlaying(MusicId id, MusicDestinationMarker destinationMarker)
        {
            if (IsPlaying()) return;

            Request(id, destinationMarker);
        }

        private void Request(MusicId id, MusicDestinationMarker? destinationMarker)
        {
            if (id == MusicId.None) return;

            if (currentId == id && IsPlaying()) return;

            if (!catalog.TryGet(id, out var reference)) return;

            if (current.isValid())
            {
                current.stop(StudioStopMode.ALLOWFADEOUT);
                current.release();
            }

            current = RuntimeManager.CreateInstance(reference);
            if (destinationMarker.HasValue)
            {
                JumpToDestinationMarker(destinationMarker.Value);
            }

            current.start();
            currentId = id;
        }

        public void SetIsInRoom(bool isInRoom)
        {
            if (!current.isValid()) return;
            current.setParameterByName("IsInRoom", isInRoom ? 1f : 0f);
        }

        private void JumpToDestinationMarker(MusicDestinationMarker destinationMarker)
        {
            switch (destinationMarker)
            {
                case MusicDestinationMarker.Room:
                    SetIsInRoom(true);
                    break;
                case MusicDestinationMarker.Boss:
                    SetIsInRoom(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(destinationMarker), destinationMarker, null);
            }
        }

        private bool IsPlaying()
        {
            if (!current.isValid()) return false;

            return current.getPlaybackState(out var state) == FMOD.RESULT.OK &&
                   (state == PLAYBACK_STATE.PLAYING ||
                    state == PLAYBACK_STATE.STARTING ||
                    state == PLAYBACK_STATE.SUSTAINING);
        }

        public void Dispose()
        {
            StopCurrent();
        }

        private void StopCurrent()
        {
            if (!current.isValid()) return;
            current.stop(StudioStopMode.ALLOWFADEOUT);
            current.release();
            current = default;
            currentId = null;
        }
    }
}

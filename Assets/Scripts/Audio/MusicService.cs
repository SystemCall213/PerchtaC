using System;
using FMOD.Studio;
using FMODUnity;
using StudioStopMode = FMOD.Studio.STOP_MODE;

namespace Audio
{
    public interface IMusicService
    {
        void Request(MusicId id);
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
            if (id == MusicId.None) return;

            if (currentId == id && current.isValid()) return;

            if (!catalog.TryGet(id, out var reference)) return;

            if (current.isValid())
            {
                current.stop(StudioStopMode.ALLOWFADEOUT);
                current.release();
            }

            current = RuntimeManager.CreateInstance(reference);
            current.start();
            currentId = id;
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

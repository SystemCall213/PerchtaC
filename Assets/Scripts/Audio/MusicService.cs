using System;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using Zenject;
using StudioStopMode = FMOD.Studio.STOP_MODE;

//#TODO clear music change logic
namespace Audio
{
    public interface IMusicService
    {
        MusicId? Current { get; }
        void Request(MusicId id);
        void StopAfterLoop();
        void StopImmediate();
    }

    public sealed class MusicService : IMusicService, ITickable, IDisposable
    {
        private readonly MusicCatalog catalog;

        private EventInstance current;
        private MusicId? currentId;
        private MusicId? pendingId;
        private bool stopPending;
        private bool callbackAttached;
        private int lastTimelinePos;

        private EVENT_CALLBACK callbackDelegate;

        public MusicId? Current => currentId;

        public MusicService(MusicCatalog catalog)
        {
            this.catalog = catalog;
            callbackDelegate = OnFmodCallback;
        }

        public void Request(MusicId id)
        {
            if (id == MusicId.None) return;
            if (currentId == null)
            {
                StartImmediate(id);
                return;
            }
            if (currentId == id) return;
            pendingId = id;
            stopPending = false;
        }

        public void StopAfterLoop()
        {
            if (!current.isValid()) return;
            pendingId = null;
            stopPending = true;
        }

        public void StopImmediate()
        {
            if (!current.isValid()) return;
            current.stop(StudioStopMode.IMMEDIATE);
            current.release();
            current = default;
            currentId = null;
            pendingId = null;
            stopPending = false;
            callbackAttached = false;
        }

        public void Tick()
        {
            if (!current.isValid()) return;
            if (current.getTimelinePosition(out int pos) == FMOD.RESULT.OK)
            {
                if (pos < lastTimelinePos)
                {
                    ApplyPendingSwitchOrStop();
                }
                lastTimelinePos = pos;
            }
        }

        public void Dispose()
        {
            StopImmediate();
        }

        private void StartImmediate(MusicId id)
        {
            if (!catalog.TryGet(id, out var reference)) return;

            if (current.isValid())
            {
                current.stop(StudioStopMode.IMMEDIATE);
                current.release();
            }

            current = RuntimeManager.CreateInstance(reference);
            current.start();
            currentId = id;
            pendingId = null;
            stopPending = false;
            lastTimelinePos = 0;
            AttachMarkerCallback();
        }

        private void AttachMarkerCallback()
        {
            if (!current.isValid() || callbackAttached) return;
            current.setCallback(callbackDelegate, EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
            callbackAttached = true;
        }

        private FMOD.RESULT OnFmodCallback(EVENT_CALLBACK_TYPE type, IntPtr eventInstance, IntPtr param)
        {
            if (type == EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
            {
                var props = Marshal.PtrToStructure<TIMELINE_MARKER_PROPERTIES>(param);
                var name = props.name;
                if (string.Equals(name, "LoopEnd", StringComparison.Ordinal))
                {
                    ApplyPendingSwitchOrStop();
                }
            }
            return FMOD.RESULT.OK;
        }

        private void ApplyPendingSwitchOrStop()
        {
            if (!current.isValid()) return;

            if (stopPending)
            {
                current.stop(StudioStopMode.ALLOWFADEOUT);
                current.release();
                current = default;
                currentId = null;
                stopPending = false;
                pendingId = null;
                callbackAttached = false;
                return;
            }

            if (pendingId is MusicId next)
            {
                if (catalog.TryGet(next, out var nextRef))
                {
                    var nextInst = RuntimeManager.CreateInstance(nextRef);
                    nextInst.start();

                    current.stop(StudioStopMode.ALLOWFADEOUT);
                    current.release();

                    current = nextInst;
                    currentId = next;
                    pendingId = null;
                    stopPending = false;
                    lastTimelinePos = 0;
                    callbackAttached = false;
                    AttachMarkerCallback();
                }
            }
        }
    }
}

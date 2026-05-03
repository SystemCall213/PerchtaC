---
sessionId: session-260502-013738-3a4z
isActive: false
---

# Requirements

### Overview & Goals
Simplify `Assets/Scripts/Audio/MusicService.cs` so music track requests reliably change the currently playing FMOD event, and remove unused/dead loop-transition logic.

### Scope
#### In Scope
- Make `IMusicService.Request(MusicId id)` switch tracks immediately when a different valid `MusicId` is requested.
- Keep the no-op behavior for `MusicId.None`, missing catalog entries, and repeated requests for the already-playing track.
- Ensure the current FMOD `EventInstance` is stopped and released before/while starting a replacement track to avoid leaked instances.
- Remove unused public API and implementation complexity from `MusicService.cs`, including delayed loop-stop/switch machinery that is not used by the game states.
- Preserve existing integration points:
  - `SoundInstaller` still binds `MusicService` through Zenject.
  - `MainMenuState`, `RoomState`, and `CombatState` continue using `musicService.Request(...)`.

#### Out of Scope
- Changing `MusicCatalog.asset` entries or FMOD event authoring.
- Adding new music-routing behavior in the currently empty `MusicRouter`.
- Changing room/combat state payload models or scene-state selection logic.

### Functional Requirements
- Entering main menu requests `MusicId.MainMenu` and plays `event:/Menu` from `MusicCatalog.asset`.
- Entering room/combat states requests the payload music id and replaces the currently playing track when it differs.
- Re-requesting the current track does not restart it unnecessarily.
- Missing catalog entries fail safely without stopping the currently playing valid track.
- Disposing `MusicService` stops and releases the active FMOD instance.

# Technical Design

### Current Implementation
- `MusicService.cs` defines `IMusicService` and `MusicService`.
- `Request(MusicId id)` currently starts immediately only when no track is active; otherwise it stores `pendingId` and waits for either:
  - an FMOD timeline marker named `LoopEnd`, via `EVENT_CALLBACK_TYPE.TIMELINE_MARKER`; or
  - a timeline-position wrap detected in `ITickable.Tick()`.
- If the FMOD music events do not emit the expected marker or timeline wrap, requested tracks remain pending and never switch.
- Project usage shows only `Request(...)` is consumed externally:
  - `MainMenuState.Enter()` requests `MusicId.MainMenu`.
  - `RoomState.Enter()` requests `Payload.MusicId` when not `MusicId.None`.
  - `CombatState.Enter()` requests `combatMusic`.
- `StopAfterLoop()` and `StopImmediate()` are not called from project code; `StopImmediate()` is only used internally by `Dispose()`.
- `MusicCatalog.cs` is a simple `ScriptableObject` dictionary from `MusicId` to `FMODUnity.EventReference`, matching the style of `SoundCatalog`/`SoundService`.
- `SoundInstaller.cs` binds `MusicCatalog` and `MusicService` via `Container.BindInterfacesAndSelfTo<MusicService>().AsSingle().NonLazy();`.

### Key Decisions
- **Immediate switching instead of pending loop-bound switching.** This directly fixes the current “tracks are not changing” behavior and removes dependence on FMOD marker authoring.
- **Minimal public API.** Keep only the externally used music request contract on `IMusicService`; make stop/release behavior an internal implementation detail used by `Dispose()`.
- **No Zenject ticking.** Remove `ITickable` from `MusicService` because there is no longer per-frame pending-switch state to poll.
- **Fail-safe catalog lookup.** Resolve the next track before stopping the current one, so invalid/missing music IDs do not silence existing music.

### Proposed Changes
#### `Assets/Scripts/Audio/MusicService.cs`
- Simplify `IMusicService` to the externally used method:
  ```csharp
  public interface IMusicService
  {
      void Request(MusicId id);
  }
  ```
- Simplify `MusicService` to implement `IMusicService` and `IDisposable` only.
- Keep only these fields:
  - `MusicCatalog catalog`
  - `EventInstance current`
  - `MusicId? currentId`
- Implement `Request(MusicId id)` as:
  - return for `MusicId.None`;
  - return if `currentId == id` and the current instance is valid;
  - `catalog.TryGet(id, out reference)` before stopping the current track;
  - stop/release the current instance;
  - create/start the new instance;
  - set `currentId = id`.
- Replace public stop methods with a private helper, e.g. `StopCurrent()`:
  - validate `current.isValid()`;
  - stop with `StudioStopMode.ALLOWFADEOUT`;
  - release;
  - clear `current` and `currentId`.
- Remove redundant fields and imports:
  - `System.Runtime.InteropServices`
  - `FMOD`
  - callback delegate and timeline-marker logic
  - `pendingId`, `stopPending`, `callbackAttached`, `lastTimelinePos`
  - `Tick()`, `StopAfterLoop()`, public `StopImmediate()`, `StartImmediate()`, `AttachMarkerCallback()`, `OnFmodCallback()`, `ApplyPendingSwitchOrStop()`.

### File Impact
- Modify only `Assets/Scripts/Audio/MusicService.cs` unless compilation reveals a remaining stale reference.
- No changes expected in:
  - `Assets/Scripts/Audio/MusicCatalog.cs`
  - `Assets/Scripts/Audio/SoundInstaller.cs`
  - `Assets/Scripts/GameStateMachine/States/MainMenuState.cs`
  - `Assets/Scripts/GameStateMachine/States/RoomState.cs`
  - `Assets/Scripts/GameStateMachine/States/CombatState.cs`

### Risks
- If future gameplay relies on loop-bound transitions, immediate switching changes that behavior. Current project search found no callers for `StopAfterLoop()` or `StopImmediate()`, and all active game-state usage requests direct track changes.
- If a requested `MusicId` is not configured in `MusicCatalog.asset` (currently only MainMenu, Room1, and Boss1 are present), the service will intentionally keep the current track playing.

# Testing

### Validation Approach
- Use a compile/build check to catch stale interface or Zenject binding issues after removing `ITickable` and unused public methods.
- Validate through code-level behavior because this is a Unity/FMΟD service with runtime audio effects.

### Key Scenarios
- Request `MusicId.MainMenu` from no current music: creates and starts the menu event.
- Request a different configured id, such as `MusicId.Room1` or `MusicId.Boos1`: stops/releases the previous event and starts the requested event immediately.
- Request the same id again: no restart occurs.
- Request `MusicId.None`: no change occurs.
- Request a catalog-missing id, such as `Room2` if not configured: current music is not stopped.
- Dispose service: active event is stopped/released without leaving pending state.

# Delivery Steps

### ✓ Step 1: Replace delayed switching with immediate track replacement
`MusicService.Request` directly starts a different configured track when requested.

- Update `MusicService` so it no longer queues `pendingId` for loop-bound switching.
- Resolve the requested `MusicId` through `MusicCatalog.TryGet` before stopping the active track.
- Stop and release the existing `EventInstance` only when a replacement track is valid.
- Create, start, and remember the new `EventInstance` and `currentId` immediately.

### ✓ Step 2: Remove unused music service API and loop callback machinery
`MusicService.cs` contains only the state and methods needed for direct music requests and disposal.

- Remove `ITickable` from `MusicService` and delete the `Tick()` timeline-wrap polling logic.
- Remove FMOD timeline-marker callback setup and `LoopEnd` handling.
- Remove unused pending/stop fields and callback-related imports.
- Reduce `IMusicService` to the externally used `Request(MusicId id)` contract.
- Convert public stop behavior into a private release helper used by replacement and `Dispose()`.

### ✓ Step 3: Verify integration with existing game states and bindings
Existing Zenject bindings and game-state music requests compile against the simplified service.

- Check `SoundInstaller` still binds `MusicService` through `BindInterfacesAndSelfTo<MusicService>()` without requiring `ITickable`.
- Confirm `MainMenuState`, `RoomState`, and `CombatState` compile with the reduced `IMusicService` interface.
- Run an available compile/build validation to catch stale references to removed methods or properties.
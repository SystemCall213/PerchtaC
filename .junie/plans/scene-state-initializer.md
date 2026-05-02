---
sessionId: session-260501-195914-dn53
isActive: false
---

# Requirements

### Overview & Goals
Complete the scene-placed state initializer by building on the existing `Assets/Scripts/GameStateMachine/StateSwichInitiator.cs`. The component should enter the enum-selected game state when the scene starts, and the Inspector should show the correct inline `IStatePayload` fields for states that require payload data.

### In Scope
- Rework the existing `StateSwichInitiator` instead of introducing a parallel scene-start component.
- Replace the hardcoded factory-injection/switch boilerplate with a reflection-backed registry so adding a new selectable state needs minimal initializer changes.
- Add a custom class attribute that links a `SceneStateType` enum value to a state class.
- Infer the required payload type from `State<TPayload>` so the editor can create and display the correct `[SerializeReference] IStatePayload` object.
- Scope reflection to the game-state assembly, following the pattern in `Assets/Scripts/Combat/Arena/RadialPositionSelectors/RadialPositionSelectorType.cs`.
- Add a custom editor drawer similar to `RadialPositionSelectorSwitcher.cs` that swaps/clears the payload when the selected enum value changes.
- Update existing selectable states and `CombatStatePayload` so the current `MainMenu`, `Cinematic`, `Combat`, and `Room` cases work.

### Out of Scope
- Reworking scene loading, async transitions, or `SceneLoader` behavior.
- Removing the existing Zenject `PlaceholderFactory` bindings; they are still used by other code such as `DialogueStarter` and `DialogueManager`.
- Renaming `StateSwichInitiator` unless scene/prefab references are intentionally migrated.
- Inline editing of `ScriptableObject` payloads through `[SerializeReference]`; this plan targets serializable non-`UnityEngine.Object` payload classes such as `CombatStatePayload`.

# Technical Design

### Current Implementation
- `StateSwichInitiator.cs` already has the right runtime location and lifecycle: it is a `MonoBehaviour`, injects `IGameStateMachine`, and calls `LoadState()` from `Start()`.
- The current version also contains most of the intended behavior, but it is not scalable:
  - it defines a nested `StateEnum`,
  - injects one factory per state (`MainMenuState.Factory`, `CinematicState.Factory`, `CombatState.Factory`, `RoomState.Factory`),
  - stores a raw `[SerializeReference] IStatePayload payload`,
  - uses a `switch` to create each state manually.
- `CombatState` is the only current inline payload state: `CombatState : State<CombatStatePayload>`, and its constructor receives `CombatStatePayload` plus Zenject dependencies (`IMusicService`, `DefaultActions`).
- `CombatStatePayload` currently exposes `MusicId` as an auto-property, which Unity will not reliably draw as inline serialized managed-reference data.
- `GameStateMachineInstaller.cs` binds existing Zenject factories; those bindings can remain for other systems, but the scene initializer does not need to inject every factory if it can instantiate registered state classes through Zenject.
- The radial selector reference (`RadialPositionSelectorType.cs` and `RadialPositionSelectorSwitcher.cs`) already demonstrates the desired pattern: enum + attribute + serializable wrapper + `[SerializeReference]` + assembly-scoped reflection + custom drawer.

### Key Decisions
- Keep `StateSwichInitiator` as the component name for compatibility, but rewrite the internals for readability and scalability. A later typo-fix rename can be a separate migration if needed.
- Put the enum-linking attribute on state classes, not payload classes. This lets one registry answer both runtime questions: “which state type should be created?” and editor questions: “does this state need a payload, and which payload type is it?”
- Infer payload type by walking the state inheritance chain and finding `State<TPayload>`. For example, `[SceneState(SceneStateType.Combat)] CombatState : State<CombatStatePayload>` maps `Combat` to both `CombatState` and `CombatStatePayload`.
- Instantiate states with Zenject `DiContainer.Instantiate(...)` from the initializer. This removes the factory-field and `switch` boilerplate while still resolving constructor/field dependencies such as `IMusicService`, `DefaultActions`, and `IUIFacade`.
- Preserve the existing `IGameStateMachine` API. Payload states can still be passed through the generic `ChangeState<TPayload>` path via a small reflection helper, or safely through `ChangeState(State)` after construction if the created state already has its payload assigned.
- Treat `None` as a no-op and clear/hide payload for any state with no inferred payload type.

### Proposed Runtime Model
Add a reusable selection/registry model under `Assets/Scripts/GameStateMachine/`:

```csharp
public enum SceneStateType
{
    None = 0,
    MainMenu,
    Cinematic,
    Combat,
    Room
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class SceneStateAttribute : Attribute
{
    public SceneStateType Type { get; }
    public SceneStateAttribute(SceneStateType type) => Type = type;
}

[Serializable]
public struct SceneStateSelection
{
    public SceneStateType stateType;
    [SerializeReference] public IStatePayload payload;

    public void ValidateAndInitialize();
    public static bool PayloadMatchesState(IStatePayload payload, SceneStateType stateType);
    public static IStatePayload CreatePayload(SceneStateType stateType);
}
```

The registry mirrors `RadialPositionSelectorData.CreateSelector(...)`, but scans only the state assembly:

```csharp
_stateTypes = typeof(State).Assembly.GetTypes()
    .Where(t => typeof(State).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
    .Select(t => new { StateType = t, Attr = t.GetCustomAttribute<SceneStateAttribute>() })
    .Where(x => x.Attr != null)
    .ToDictionary(x => x.Attr.Type, x => BuildDescriptor(x.StateType));
```

`BuildDescriptor(...)` will infer an optional payload type from the `State<TPayload>` base class.

### Proposed Component Behavior
`StateSwichInitiator` will keep the existing `Start() -> LoadState()` flow, but its fields become generic:

```csharp
[Inject] private IGameStateMachine gameStateMachine;
[Inject] private DiContainer container;
[SerializeField] private SceneStateSelection initialState;
```

At runtime:
- `None` returns immediately.
- `initialState.ValidateAndInitialize()` ensures the payload is present only when the selected state requires one.
- The registry resolves the selected enum to a state descriptor.
- The initializer creates the state through `container.Instantiate(descriptor.StateType, extraArgs)` where `extraArgs` contains the payload only for `State<TPayload>` states.
- The initializer passes the created state to `gameStateMachine.ChangeState(...)`, using the payload-aware overload when practical to preserve the current duplicate-state comparison behavior.
- Missing attributes, duplicate enum mappings, or missing required payload instances produce clear `Debug.LogError`/`Debug.LogWarning` messages instead of null-reference crashes.

### Existing State Updates
Existing selectable states get one attribute each:

```csharp
[SceneState(SceneStateType.MainMenu)]
public class MainMenuState : State { ... }

[SceneState(SceneStateType.Cinematic)]
public class CinematicState : State { ... }

[SceneState(SceneStateType.Combat)]
public class CombatState : State<CombatStatePayload> { ... }

[SceneState(SceneStateType.Room)]
public class RoomState : State { ... }
```

`CombatStatePayload` becomes Unity-serializable while keeping the current runtime contract:

```csharp
[Serializable]
public class CombatStatePayload : IStatePayload
{
    [SerializeField] private MusicId musicId;
    public MusicId MusicId => musicId;
}
```

### Editor Design
Create a `CustomPropertyDrawer` for `SceneStateSelection` under `Assets/Scripts/GameStateMachine/Editor/`, using the radial selector drawer as the template:

- Draw the `stateType` enum first.
- On enum change, call `SceneStateSelection.CreatePayload(selectedState)` and assign the result to `payloadProp.managedReferenceValue`.
- If the selected state has no payload type, set the managed reference to `null` and draw only the enum.
- If the existing payload object no longer matches the selected state, replace it with the inferred payload type.
- Draw the payload using `EditorGUI.PropertyField(..., includeChildren: true)` so public/serialized fields such as `CombatStatePayload.MusicId` are visible.
- Implement `GetPropertyHeight` with `EditorGUI.GetPropertyHeight(payloadProp, true)` so nested payload fields do not overlap.

### Files Affected
- `Assets/Scripts/GameStateMachine/StateSwichInitiator.cs`
  - Keep the class, remove per-state factory fields and the manual `switch`.
  - Store one `SceneStateSelection` and create selected states through the registry + Zenject container.
- New runtime file(s) under `Assets/Scripts/GameStateMachine/`, for example `SceneStateSelection.cs` / `SceneStateRegistry.cs`
  - Define `SceneStateType`, `SceneStateAttribute`, descriptors, registry lookup, payload inference, and payload creation/matching helpers.
- `Assets/Scripts/GameStateMachine/States/MainMenuState.cs`
- `Assets/Scripts/GameStateMachine/States/CinematicState.cs`
- `Assets/Scripts/GameStateMachine/States/CombatState.cs`
- `Assets/Scripts/GameStateMachine/States/RoomState.cs`
  - Add `[SceneState(...)]` attributes.
- `Assets/Scripts/GameStateMachine/StatePayload/CombatStatePayload.cs`
  - Add `[Serializable]`, serialized `MusicId` data, and a read-only property for `CombatState`.
- New editor file under `Assets/Scripts/GameStateMachine/Editor/`, for example `SceneStateSelectionDrawer.cs`
  - Implement the dynamic inspector behavior.
- `Assets/Scripts/GameStateMachine/GameStateMachineInstaller.cs`
  - Expected to remain unchanged because the initializer uses `DiContainer.Instantiate(...)` and existing factories are still needed elsewhere.

### Risks & Mitigations
- **Direct `DiContainer` use can become a service-locator smell:** keep it isolated inside this generic scene-start adapter; ordinary gameplay code should continue using explicit dependencies/factories.
- **Payload constructor requirements:** inline `[SerializeReference]` payloads must have a public parameterless constructor; log an error if `Activator.CreateInstance` fails.
- **Unity serialization limitations:** use fields or `[SerializeField]` backing fields, not auto-properties, for payload data shown in the Inspector.
- **Duplicate enum attributes:** detect duplicate `SceneStateAttribute` values while building the registry and log a clear error.
- **`ScriptableObject` payloads:** do not create them with `[SerializeReference]`; add a separate object-reference payload path later if `DialogueState` needs scene-initializer support.

# Testing

### Validation Approach
Validation will focus on Unity inspector behavior and Play Mode startup from a scene that contains `StateSwichInitiator`.

### Key Scenarios
- Select `Room`; Inspector shows no payload and Play Mode enters `RoomState`.
- Select `MainMenu` or `Cinematic`; Inspector shows no payload and the corresponding attributed state is instantiated through Zenject.
- Select `Combat`; Inspector creates a `CombatStatePayload` managed reference and shows its `MusicId` field.
- Change from `Combat` to a no-payload state; payload is cleared/hidden.
- Change back to `Combat`; a matching payload object is created again and its value is passed to `CombatState`.

### Edge Cases
- `None` selected: component does nothing.
- A state enum has no attributed state class: initializer logs a clear error and does not change state.
- A selected state requires a payload but payload creation fails: initializer logs a clear error and avoids a null-reference exception.
- Payload object type no longer matches the selected enum after script changes: drawer replaces it on the next inspector update/change.

# Delivery Steps

### ✓ Step 1: Add the reflection-backed scene state model
The project has a reusable registry that maps enum values to state descriptors and optional payload types.

- Add `SceneStateType` with the current initializer targets: `None`, `MainMenu`, `Cinematic`, `Combat`, and `Room`.
- Add `SceneStateAttribute` for annotating state classes with their enum value.
- Add `SceneStateSelection` with `stateType` and `[SerializeReference] IStatePayload payload`.
- Add a registry/descriptor helper that scans `typeof(State).Assembly.GetTypes()` for attributed `State` classes.
- Infer payload type by detecting `State<TPayload>` in the state inheritance chain.
- Add helper methods to create, clear, and validate payloads for the selected state.

### ✓ Step 2: Rewrite `StateSwichInitiator` around the registry
The existing scene-start component enters any registered state without per-state factory fields or a manual switch.

- Keep the `StateSwichInitiator` component and its `Start() -> LoadState()` behavior.
- Replace the nested enum and raw payload fields with one `SceneStateSelection` field.
- Inject only the generic services needed by the adapter: `IGameStateMachine` and Zenject `DiContainer`.
- Resolve the selected state descriptor at runtime and instantiate the state with `DiContainer.Instantiate(...)`.
- Pass payloads as Zenject extra arguments for `State<TPayload>` states such as `CombatState`.
- Call the appropriate `IGameStateMachine.ChangeState(...)` overload and log clear errors for invalid selections.

### ✓ Step 3: Register existing states and serialize the combat payload
All currently selectable states participate in the registry, and combat payload data is editable in the Inspector.

- Add `[SceneState(SceneStateType.MainMenu)]` to `MainMenuState`.
- Add `[SceneState(SceneStateType.Cinematic)]` to `CinematicState`.
- Add `[SceneState(SceneStateType.Combat)]` to `CombatState`.
- Add `[SceneState(SceneStateType.Room)]` to `RoomState`.
- Convert `CombatStatePayload` to a `[Serializable]` payload with serialized `MusicId` data.
- Keep `CombatState` compatible by preserving access through `payload.MusicId`.

### ✓ Step 4: Add the dynamic inspector drawer and safeguards
The Inspector shows only the payload fields required by the selected scene state.

- Add a `CustomPropertyDrawer` for `SceneStateSelection` in a GameStateMachine `Editor` folder.
- Draw the enum first and update `payload.managedReferenceValue` when the enum changes.
- Create a matching payload object for `Combat` and clear payload for no-payload states.
- Draw payload children with `EditorGUI.PropertyField(..., true)` and calculate dynamic property height.
- Validate the main editor/runtime scenarios: no-payload states hide payload, `Combat` shows `MusicId`, `None` is a no-op, and Play Mode enters the selected attributed state.
---
sessionId: session-260512-143250-z63s
isActive: false
---

# Requirements

### Overview & Goals
Update the HoldPoint glyph flow so each spawned glyph line uses a runtime-generated `512x512` mask sprite and the `GlyphFollower` progressively paints that mask in white/full-alpha as it travels. This should reveal the masked `LineRenderer` gradually instead of showing the whole glyph immediately.

### Scope
#### In Scope
- Generate a new transparent runtime `Texture2D`/`Sprite` for the `SpriteMask` attached to each spawned glyph line GameObject.
- Assign the generated sprite to the line GameObject’s `SpriteMask` during follower/line initialization.
- Paint white full-alpha pixels into that sprite as `GlyphFollower` moves along the line.
- Preserve the existing HoldPoint behavior in `GlyphFollower.cs`: follow line points, move only when the player is close, move backwards outside the reset radius, and trigger `GlyphFacade.TriggerGlyphPainted()` at completion.
- Destroy runtime-created texture/sprite resources when the `GlyphFollower` is destroyed.

#### Out of Scope
- Reworking the separate BlindPainting glyph system (`Assets/Scripts/Glyph/Glyph_BlindPainting/*`), which already uses `RenderTexture` masks and shader painting.
- Changing glyph line prefab geometry or scene installer configuration beyond what is required to support the runtime mask sprite.
- Adding new gameplay rules for completion/progress thresholds.

### Functional Requirements
- A spawned HoldPoint glyph line starts hidden except for any already-painted mask pixels.
- The generated mask uses a `512x512` texture initialized fully transparent.
- The line GameObject’s `SpriteMask.sprite` receives the generated sprite before painting begins.
- As `GlyphFollower` advances, it maps its current world/local position onto mask texture coordinates and paints a configurable circular area with `Color.white` and alpha `1`.
- The existing `LineRenderer.maskInteraction` setup, such as `VisibleInsideMask` on `Assets/Prefab/Glyph/SquareLine.prefab`, is respected so the white mask region reveals the visible line.
- Runtime mask resources are cleaned up when the follower completes and destroys itself, and also if the follower is destroyed early.

# Technical Design

### Current Implementation
- HoldPoint glyphs are driven by `Assets/Scripts/Glyph/Glyph_HoldPoint/GlyphProgressTracker.cs`:
  - `Start()` calls `UpdateLineRenderer(0)` and subscribes to `glyphFacade.OnGlyphPainted`.
  - `SpawnLineRenderer()` instantiates the next injected `LineRenderer` prefab under the tracker transform.
  - `SpawnFollower()` instantiates `Assets/Prefab/Glyph/GlyphFollower.prefab` via Zenject and calls `GlyphFollower.Initialize(lineRenderer, player, this, onComplete)`.
- `Assets/Scripts/Glyph/Glyph_HoldPoint/GlyphFollower.cs` currently:
  - Stores sampled line points in world space.
  - Moves forward/backward based on player distance/reset radius.
  - Calls `onReachedEnd` and `Destroy(gameObject)` when the path completes.
- `Assets/Prefab/Glyph/SquareLine.prefab` already has:
  - A visible white `LineRenderer` on the root GameObject with `m_MaskInteraction: 2` (`VisibleInsideMask`).
  - A `SpriteMask` component on the same root GameObject with `m_Sprite: {fileID: 0}`.
  - A gray unmasked child `LineRenderer` used as the background guide.
- The BlindPainting system (`GlyphRenderer.cs`, `PlayerPainting.cs`, `BrushShader.shader`) provides useful reference for runtime mask creation/painting concepts, but it uses `RenderTexture` + material shader masking, not Unity `SpriteMask` sprites.

### Key Decisions
- Implement the new mask lifecycle in the HoldPoint glyph path, not in BlindPainting, because the requested behavior targets line renderers and `GlyphFollower`.
- Keep `GlyphProgressTracker` responsible for spawning the line and follower, while `GlyphFollower` owns the runtime mask resources because the user explicitly wants the sprite destroyed with the follower.
- Use a CPU-editable `Texture2D` with `TextureFormat.RGBA32`, converted to a `Sprite` via `Sprite.Create`, because `SpriteMask` requires a `Sprite` and the follower needs to paint white/full-alpha pixels into it over time.
- Derive world/local-to-texture mapping from the spawned line’s local bounds/points so the mask aligns with the glyph line GameObject where the `SpriteMask` is attached.

### Proposed Changes
- Extend `GlyphFollower` to cache the spawned line’s `SpriteMask`, generated `Texture2D`, and generated `Sprite`.
- During `GlyphFollower.Initialize(...)`:
  - Resolve `SpriteMask` from `lineRenderer.GetComponent<SpriteMask>()` on the same GameObject.
  - Create a `512x512` transparent texture.
  - Compute the mask’s local coverage rect from the line’s local points and line width, padded enough for the brush radius.
  - Create a sprite whose pixels-per-unit makes the sprite cover that local rect.
  - Assign the generated sprite to `SpriteMask.sprite`.
  - Position/scale considerations stay on the existing line GameObject; mapping accounts for local-space bounds.
- Add serialized brush settings to `GlyphFollower`:
  - `maskResolution = 512`
  - `paintRadius` in world/local units or texture pixels
  - optional `paintInterval`/dirty-apply throttling if needed for performance
- Add a private paint method in `GlyphFollower`, called after movement updates:
  - Convert `transform.position` into the line’s local space using `lineRenderer.transform.InverseTransformPoint`.
  - Convert local `x/y` into normalized UV coordinates using the computed mask rect.
  - Paint a circular region into the texture with `Color32(255,255,255,255)`.
  - Call `Texture2D.Apply(false)` after each paint or after batched paint updates.
- Add cleanup in `GlyphFollower.OnDestroy()`:
  - Clear the `SpriteMask.sprite` reference if it still points at the generated sprite.
  - `Destroy(generatedSprite)` and `Destroy(generatedTexture)`.

### Data / API Shape
Expected additions inside `GlyphFollower.cs`:

```csharp
[SerializeField] private int maskResolution = 512;
[SerializeField] private float paintRadius = 0.2f;

private SpriteMask spriteMask;
private Texture2D maskTexture;
private Sprite maskSprite;
private Rect localMaskRect;

private void InitializeMask(LineRenderer line);
private void PaintAtFollowerPosition();
private void PaintCircle(int centerX, int centerY, int radiusPixels);
private void OnDestroy();
```

### Affected Files
- `Assets/Scripts/Glyph/Glyph_HoldPoint/GlyphFollower.cs`
  - Main implementation location for mask generation, painting, and cleanup.
- `Assets/Prefab/Glyph/GlyphFollower.prefab`
  - May need serialized values for `maskResolution`/`paintRadius` after fields are added.
- `Assets/Prefab/Glyph/SquareLine.prefab` and any other line glyph prefabs referenced by `GlyphLinesSOInstaller`
  - Should already have `SpriteMask` on the same GameObject and `LineRenderer.maskInteraction = VisibleInsideMask`; implementation should tolerate missing `SpriteMask` with a warning/no-op rather than crashing.

### Risks
- `SpriteMask` sizing must match the line’s local bounds; otherwise painting can reveal offset or scaled regions. The implementation will compute bounds from `LineRenderer` positions rather than assuming a fixed square.
- Per-frame `Texture2D.Apply()` on a `512x512` texture can be costly. If needed, the implementation can skip applying when no pixels changed and throttle/batch applies.
- Destroying the line renderer in `GlyphProgressTracker.SpawnLineRenderer()` before an old follower is destroyed could leave old runtime resources if multiple followers overlap. `GlyphFollower.OnDestroy()` will own cleanup regardless of completion path.

# Testing

### Validation Approach
Validation will focus on Unity runtime behavior and compile safety for the HoldPoint glyph system.

### Key Scenarios
- Start a HoldPoint combat scene such as `Boss1`, `Boss2`, or `Boss3`, where `glyphFollowerPrefab` is assigned and HoldPoint glyphs are configured.
- Confirm the spawned glyph line’s `SpriteMask.sprite` is assigned at runtime and has a `512x512` texture.
- Confirm the visible white line is initially hidden by the transparent mask while the gray guide child remains visible.
- Move the player near the follower and verify the glyph line is revealed progressively along the follower path.
- Let the follower reach the end and verify the existing completion behavior still triggers the next glyph.

### Edge Cases
- A line prefab without a `SpriteMask` should not crash the game; it should skip mask setup/painting safely.
- A line with zero points should retain the existing early-return behavior in `GlyphProgressTracker.SpawnFollower()`.
- Destroying/replacing the active line or follower early should clean up generated runtime texture/sprite resources.

### Test Changes
- No automated Unity test files are required unless the project already has a suitable playmode test harness for glyph rendering. At minimum, run a compile/build check after implementation.

# Delivery Steps

### ✓ Step 1: Add runtime mask generation to GlyphFollower initialization
`GlyphFollower` creates and assigns a transparent 512x512 sprite mask for the spawned line when initialized.

- Update `Assets/Scripts/Glyph/Glyph_HoldPoint/GlyphFollower.cs` to cache the line’s same-GameObject `SpriteMask`.
- Add serialized settings for mask resolution and paint radius, defaulting the resolution to `512`.
- Create a transparent `Texture2D` and `Sprite` during `Initialize(...)`.
- Compute local mask coverage from the initialized `LineRenderer` points and width so the sprite aligns with the glyph line.
- Assign the generated sprite to `SpriteMask.sprite` and handle missing masks with a safe no-op/warning.

### ✓ Step 2: Implement follower-driven white-alpha painting
`GlyphFollower` paints the generated mask sprite as it moves along the glyph path.

- Add a painting method that converts follower world position to the line renderer’s local space.
- Map local position into the generated mask texture’s pixel coordinates using the computed mask rect.
- Paint a circular brush area with white full-alpha pixels.
- Apply texture updates only when pixels changed to avoid unnecessary per-frame work.
- Invoke painting during forward/backward movement so the reveal follows the follower path.

### ✓ Step 3: Clean up generated mask resources with the follower
Runtime-created mask sprite and texture are destroyed when the follower is destroyed.

- Add `OnDestroy()` cleanup in `GlyphFollower.cs`.
- Clear `SpriteMask.sprite` if it references the generated sprite.
- Destroy the generated `Sprite` and `Texture2D` safely.
- Ensure cleanup runs for normal completion (`Destroy(gameObject)` at path end) and early destruction.

### ✓ Step 4: Validate HoldPoint glyph reveal behavior
The HoldPoint glyph scene flow still works while masked line renderers reveal progressively.

- Confirm `GlyphProgressTracker` still spawns a line and follower and triggers `glyphFacade.TriggerGlyphPainted()` on completion.
- Verify prefabs like `Assets/Prefab/Glyph/SquareLine.prefab` use `LineRenderer.maskInteraction = VisibleInsideMask` and a same-object `SpriteMask`.
- Run a compile/build check or Unity play validation.
- Inspect runtime behavior for initial hidden state, progressive reveal, completion transition, and resource cleanup.
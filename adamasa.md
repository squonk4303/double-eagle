# Individual Component

## Requirements

- A video showing off the code that is tightly integrated with the game engine that is difficult to see from the text of the programming.
- An individual discussion (`username.md` or as a PDF in Inspera) covering:
  - A link to, and discussion of, code you consider good
  - A link to, and discussion of, code you consider bad
  - A personal reflection about the key concepts you learnt during the course

---

## Evaluation Rubric
Everything at default values works for me, but I guess it sort of depends a bit on how the two others want to evaluate the group based ones.

---
## Introduction

I was responsible for the UI-related aspects of the game.

(When a file extension isn't specified it means both .cs script and .tscn scene)


### Files Exclusively My Contribution

- DeathPopup (all)
- GameStateManager (all)
- GunRecoil (all)
- Health (all)
- HPBar (all)
- MainMenu (all)
- OptionsPanel (all)
- PauseMenu (all)
- Tracer (all) - deprecated

### Partial Contributions

- ShootingGallery (some: UI elements)
- Marksman (some: gun and recoil)

### 3D Modeling

- Desert Eagle gun modelled in Blender
- `gun.tscn` (all)

---

## Good Code

### 1. GameStateManager: State Machine Architecture Pattern (`GameStateManager.cs:49-74`)

**Implementation:**
```csharp
public bool CanTransitionTo(GameState newState)
{
    return (State, newState) switch
    {
        (GameState.MENU, GameState.PLAYING) => true,
        (GameState.PLAYING, GameState.PAUSED) => true,
        (GameState.PLAYING, GameState.LOST) => true,
        (GameState.PLAYING, GameState.WON) => true,
        (GameState.PAUSED, GameState.PLAYING) => true,
        (GameState.PAUSED, GameState.MENU) => true,
        (GameState.LOST, GameState.MENU) => true,
        (GameState.LOST, GameState.PLAYING) => true,
        (GameState.WON, GameState.MENU) => true,
        (GameState.WON, GameState.PLAYING) => true,
        _ => false
    };
}
```

**Why It's Good:**
- **Finite state machine implementation** with explicit validation preventing invalid transitions
- **Pattern demonstrated:** State Machine Pattern with validation
- **C# switch expressions** provide clean, readable state transition rules
- **Uses signals** to communicate between nodes (idiomatic Godot approach)
- **Prevents bugs:** Can't accidentally transition from PLAYING directly to MENU, must go through PAUSED first

**Alternatives Considered:**
- Simple boolean flags (isPaused, isDead) - rejected because they allow invalid combinations
- Event-based system without states - rejected because harder to reason about game flow

**Concrete Benefits:**
Before implementing the FSM, I had significant trouble managing distinctly different behaviors when transitioning between playing and paused states.
The state machine made these transitions explicit and prevented invalid state combinations.

### 2. GameStateManager: Centralized Input Mode Management (`GameStateManager.cs:114-121`)

**Problem:**
Initially, multiple scripts were calling `Input.MouseMode = ...` directly, causing race conditions where menus would fight over cursor visibility.
It was also quite apparent this would be hard to maintain with a growing codebase.

**Solution:**
```csharp
private void _UpdateInputMode()
{
    Input.MouseMode = State switch
    {
        GameState.PLAYING => Input.MouseModeEnum.Captured,
        _ => Input.MouseModeEnum.Visible
    };
}
```

**Why It's Good:**
- **Single Responsibility Principle:** One class owns mouse mode management
- **Centralized game flow** eliminates scattered mouse mode changes throughout codebase
- Called only on state changes, preventing redundant Input.MouseMode assignments
- **Original motivation** for implementing the state machine architecture

**Result:**
Eliminated bugs where cursor would disappear in menus or stay visible during gameplay. 
This mouse cursor issue was actually the main catalyst for implementing the entire state machine architecture; what started as a simple bug fix evolved into a fundamental architectural improvement.

### 3. Health (`Health.cs:22-38`)

**Implementation:**
```csharp
public float CurrentHealth
{
    get => currentHealth;
    private set
    {
        currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        EmitSignal(SignalName.OnHealthChanged, currentHealth);

        if (currentHealth == 0)
        {
            EmitSignal(SignalName.OnDied);
        }
    }
}
```

**Why It's Good:**
- **Event-driven architecture:** Uses Godot's signal system for observer pattern
- **Encapsulation:** Private setter prevents external code from bypassing validation
- **Automatic clamping** in setter ensures health never goes below 0 or above max
- **Decoupled design:** Health has no knowledge of who listens to its signals (HPBar, DeathPopup, etc.)
- **Side effects in property:** Automatically emits signals when health changes

**Benefits:**
- Adding new UI elements (e.g., DeathPopup) required no changes to Health.cs; just connect to existing signals
- Impossible to create invalid health states (negative health, health > max)

### 4. OptionsPanel (`OptionsPanel.cs:61-74`)

**Why It's Good:**
- **User settings persist** through `ConfigFile` (idiomatic Godot approach)
- **Separation of concerns:** Distinct `LoadSettings()` and `SaveSettings()` operations
- **Default value handling:** If config file doesn't exist or lacks a value, uses sensible defaults
- **User experience consideration:** Settings survive game restarts

**Implementation Pattern:**
```csharp
private void SaveSettings()
{
    var config = new ConfigFile();
    config.SetValue("audio", "master_volume", masterVolumeSlider.Value);
    config.SetValue("controls", "sensitivity", sensitivitySlider.Value);
    config.Save("user://settings.cfg");
}
```

---

## Bad Code

### 1. DeathPopup: Hacky Popup Prevention (`DeathPopup.cs:22-30`)

**Current Implementation:**
```csharp
private void OnVisibilityChanged()
{
    // Probably a better way to do this...
    if (!Visible)
    {
        CallDeferred(MethodName.Show);
    }
}
```

**Why It's Bad:**
- Fighting against Godot's popup behavior instead of configuring it properly
- Uses `CallDeferred` as a workaround rather than addressing root cause
- Creates technical debt and makes code harder to understand
- Even I acknowledged this with the comment "Probably a better way to do this..."
- **Code smell:** When you're fighting the framework, that indicates you're not using it correctly

**What I'd Do Differently:**
Set proper popup configuration in `_Ready()`:
```csharp
public override void _Ready()
{
    Unresizable = true;
    PopupWindow = false;  // Prevent closing on outside click
    // Or use Panel instead of Popup if we don't need popup-specific behavior
}
```

**Lesson Learned:**
When fighting the framework with workarounds, step back and read the documentation. There's usually a proper configuration option. 
This taught me that clever hacks often indicate missing understanding of the API.

### 2. Hardcoded Scene Paths Throughout Codebase

**Examples:**
- `"res://scenes/main_menu.tscn"` (`DeathPopup.cs:58`, `PauseMenu.cs:101`)
- `"res://Scenes/shooting_gallery.tscn"` (`MainMenu.cs:29`)
- `"res://Scenes/options_panel.tscn"` (`MainMenu.cs:34`, `PauseMenu.cs:67`)

**Why It's Bad:**
- **Typo-prone:** Notice inconsistent capitalization ("scenes" vs "Scenes"); no compile-time checking catches this
- **Hard to refactor:** If scene files move or get renamed, must find/replace all string references
- **Brittle:** A single typo causes runtime crash that could have been caught at compile time
- **No IDE support:** Can't use "Find All References" or automatic refactoring tools

**What I'd Do Differently:**
Create a static `ScenePaths` class with constants:
```csharp
public static class ScenePaths
{
    public const string MainMenu = "res://scenes/main_menu.tscn";
    public const string ShootingGallery = "res://scenes/shooting_gallery.tscn";
    public const string OptionsPanel = "res://scenes/options_panel.tscn";
}

// Usage:
GetTree().ChangeSceneToFile(ScenePaths.MainMenu);
```

Or use exported variables for scenes that might change:
```csharp
[Export] public PackedScene MainMenuScene;
```

**Lesson Learned:**
Magic strings are technical debt. Centralize them into constants for maintainability and type safety.

### 3. PauseMenu: Tight Coupling to Scene Structure (`PauseMenu.cs:73`)

**Current Implementation:**
```csharp
var marksman = GetParent().GetNode<Marksman>("Marksman");
```

**Why It's Bad:**
- **Assumes specific parent-child relationship:** PauseMenu must be child of node that contains Marksman sibling
- **Fragile:** If scene structure changes (e.g., PauseMenu moves to UI container), this breaks
- **No null check:** Game crashes with NullReferenceException if Marksman doesn't exist in expected location
- **Hard to test:** Can't test PauseMenu in isolation without full scene hierarchy
- **Tight coupling:** PauseMenu knows too much about scene structure

**What I'd Do Differently:**
**Option 1: Use Node Groups**
```csharp
// In Marksman.cs _Ready():
AddToGroup("player");

// In PauseMenu.cs:
var marksman = GetTree().GetNodesInGroup("player").FirstOrDefault() as Marksman;
if (marksman != null)
{
    marksman.SetSensitivity(sensitivity);
}
```

**Option 2: Use Signals (even better)**
```csharp
// In OptionsPanel.cs:
[Signal] public delegate void SensitivityChangedEventHandler(float newSensitivity);

// In Marksman.cs:
OptionsPanel.SensitivityChanged += SetSensitivity;
```

**Option 3: Export Variable**
```csharp
[Export] public Marksman Player;  // Assign in Godot editor
```

**Lesson Learned:**
Navigating node hierarchy with `GetParent().GetNode()` creates fragile code. Use dependency injection (exported variables), signals, or node groups for loose coupling.

### 4. Commented Out Debug Code

**Examples:**
Various `// GD.Print(...)` statements scattered throughout codebase

**Why It's Bad:**
- **Bloats the codebase** with inactive code that serves no purpose
- **Confuses readers:** Is this code meant to be here? Should I uncomment it?
- **Should use version control as history:** If you want to preserve old debug code, that's what git is for
- **No semantic meaning:** Can't tell if commented code is debugging, deprecated, or TODO

**What I'd Do Differently:**
**Option 1: Remove entirely**
```csharp
// Just delete it; git preserves history if you need it back
```

**Option 2: Use debug flags for intentional debug code**
```csharp
#if DEBUG
    GD.Print($"State transition: {oldState} → {newState}");
#endif
```

**Option 3: Use proper logging with levels**
```csharp
if (OS.IsDebugBuild())
{
    GD.Print($"[DEBUG] State changed to {newState}");
}
```

**Lesson Learned:**
Dead code should be deleted, not commented out. Trust your version control system. If you need conditional debugging, use preprocessor directives or debug flags.

---

## Key Concepts Learned

### Event-Driven Programming with Signals

Before this project, I primarily used direct method calls for communication between components. Implementing the Health→HPBar connection taught me the value of decoupling.

**Aha Moment:**
When I wanted to add DeathPopup that also needed to know about death, I could just connect to the existing signal; no changes to Health.cs needed.
This really demonstrates the simple strength of signals in Godot.

**Technical Learning:**
- Signal declaration with `[Signal]` attribute in C#
- Emitting signals with `EmitSignal(SignalName.OnHealthChanged, value)`
- Connecting signals both in code and via Godot editor
- Observer pattern implementation in game engine context

**Challenge Overcome:**
Initially tried connecting signals in `_Ready()` which caused null reference errors.
Learned to use `_EnterTree()` or connect signals in the Godot editor's signal connection UI for scene-based connections.

**Trade-off:**
Signals are harder to trace through code (no "Find All References" works across signal connections), but the flexibility and decoupling are worth it for UI components.

### State Machine for Game Flow

**Why I Implemented This:**
We had bugs where mouse cursor would behave incorrectly because multiple scripts were changing Input.MouseMode.
I realized we would need a better system, and I discovered centralized game state management.

**Technical Learning:**
- Finite state machine architecture with explicit states (enum-based)
- State validation via `CanTransitionTo()` method prevents invalid transitions
- Centralized state management eliminates race conditions
- Applied to menu systems, pause behavior, and game-over flows

**Pattern Recognition:**
This is the same pattern used in professional game engines (Unreal's GameMode, Unity's SceneManager).
Understanding when to use a fully fledged FSM vs. simpler boolean flags.

**Real-World Application:**
Prevented bug where player could pause while in death screen, which would soft-lock the game.

### Singleton Pattern

**Implementation:**
```csharp
public static GameStateManager Instance { get; private set; }

public override void _EnterTree()
{
    if (Instance != null)
    {
        QueueFree();
        return;
    }
    Instance = this;
}
```

**Why I Used It:**
Game state needs to be accessible from anywhere (menus, gameplay, UI).
Singleton provides global access point without passing references through multiple layers.

**Trade-offs Learned:**
- **Pro:** Convenience - any script can access `GameStateManager.Instance.State`
- **Con:** Creates global state, making code harder to test in isolation
- **Con:** Tight coupling - many classes depend on GameStateManager existing
- **Con:** Initialization order issues if accessed before `_EnterTree()` runs

**Lesson:**
Singletons are acceptable for truly global concerns (game state, input management) but shouldn't be overused.
For a small game this works; larger projects should use dependency injection or service locators.

### UI/UX Design in Game Engines

**Key Challenges Discovered:**
Game UI differs from traditional applications:
- Must handle pause state (some UI processes while game is frozen)
- Mouse capture management (FPS controls vs menu interaction)
- Dynamic loading/unloading (memory management for scenes)
- Integration with 3D world (UI overlays on gameplay)

**ProcessMode.Always Deep Dive:**
```csharp
ProcessMode = ProcessModeEnum.Always;  // PauseMenu continues while game paused
```
Without this, pause menu buttons wouldn't respond to clicks because the entire scene tree pauses. 
This taught me that game engines treat UI as part of the scene graph, unlike web frameworks where UI and logic are separate.

**Mouse Mode Management:**
Took me hours to figure out why I couldn't click menu buttons; Input.MouseMode was still set to Captured. 
Learned that mouse mode is a global state and thus must be managed carefully.

**Dynamic UI Loading:**
Learned to instance scenes at runtime for options panel to avoid loading it unnecessarily:
```csharp
var optionsScene = GD.Load<PackedScene>("res://Scenes/options_panel.tscn");
var optionsPanel = optionsScene.Instantiate<OptionsPanel>();
AddChild(optionsPanel);
```

### Godot Engine-Specific Idioms

**Scene Instancing:**
Coming from Unity, Godot's scene instancing felt different. Everything is a scene, even UI components. 
I learned to think in terms of scene composition rather than prefabs.

**Node Tree Navigation:**
- `GetNode<T>()` for typed node access
- `GetParent()` for traversing up the tree
- Learned the hard way this creates tight coupling (see Bad Code section)

**Input Handling:**
Godot's input action system (`IsActionPressed("pause")`) is more flexible than checking raw keycodes.
Allows remapping controls without code changes.

**Settings Persistence:**
`ConfigFile` API for user settings:
```csharp
config.Save("user://settings.cfg");  // user:// maps to OS-specific app data folder
```
Learned that `user://` is cross-platform path that Godot automatically resolves.

**Process Modes:**
- `ProcessModeEnum.Always` - processes even when paused
- `ProcessModeEnum.Pausable` - stops when game paused
- `ProcessModeEnum.WhenPaused` - only processes when paused

### C# Properties and Encapsulation

**Before This Project:**
I typically used public fields for simplicity.

**What I Learned:**
Properties allow validation and side effects while maintaining clean syntax:
```csharp
public float CurrentHealth
{
    get => currentHealth;
    private set  // Private setter prevents external modification
    {
        currentHealth = Mathf.Clamp(value, 0, MaxHealth);  // Validation
        EmitSignal(SignalName.OnHealthChanged, currentHealth);  // Side effect
    }
}
```

**Key Takeaways:**
- **Data hiding:** Private setters prevent invalid state
- **Validation in setters:** Impossible to set health to negative value
- **Side effects:** Property changes can trigger events automatically
- **Encapsulation:** Internal field `currentHealth` vs public property `CurrentHealth`

**When Not to Use:**
If a property has expensive computation or side effects (like emitting signals), could use an explicit method instead to make the cost obvious.

---

## Performance and Architecture Considerations

### Singleton Trade-offs
**Decision:** Used singleton for GameStateManager
**Reasoning:** Game state is truly global - needed from menus, gameplay, UI
**Scale Consideration:** Works for small game, but larger projects should use dependency injection or service locators to avoid tight coupling and improve testability

### Signal Performance
**Learning:** Signals use reflection in Godot, making them slower than direct method calls

**When to Use Signals:**
- UI updates (60fps is the bottleneck, not signal overhead)
- Infrequent events (deaths, level completion, state changes)
- Decoupling is more important than microseconds

**When NOT to Use Signals:**
- Performance-critical gameplay loops (e.g., bullet physics at 1000+ entities)
- Every frame updates (use direct method calls or polling instead)

### Process Mode Efficiency
Setting PauseMenu to `ProcessMode.Always` means it processes even when paused.
This is efficient for our use case (only 2-3 UI nodes), but taught me to be mindful of which nodes continue processing during pause.
Don't set entire scene trees to Always; only the UI that needs it.

### Memory Management
**Dynamic Scene Loading:**
Options panel is loaded on-demand rather than existing in scene tree:
- **Pro:** Saves memory when not in use
- **Pro:** Faster initial level load
- **Con:** Small delay when first opened (scene instantiation cost)
- **Decision:** Worth it for infrequently accessed UI

**Cleanup:**
Learned to call `QueueFree()` when closing dynamic UI to prevent memory leaks.

---

## Integration and Collaboration

### Working with Group Members' Code

**Challenge:**
My UI needed to interact with group member's ShootingGallery and Marksman classes without creating circular dependencies.

**Solutions Used:**
1. **Signals for one-way communication:** Health→HPBar (I emit, they listen)
2. **GameStateManager as mediator:** PauseMenu→GameStateManager→Marksman (breaks direct dependency)
3. **Exported variables in ShootingGallery.tscn:** Allowed customizing UI placement without code changes

**Lesson:**
Clear interfaces (public methods, signals, exported variables) enable parallel development. 
We could work on different systems simultaneously without blocking each other.
This was sort of crucial as we worked a lot in parallel.

### Integration Points

**My Code Consumed:**
- `Marksman.SetSensitivity(float)` - from PauseMenu options
- `ShootingGallery` exported UI positions - for placing HPBar and menus

**My Code Provided:**
- `Health` signals - for any system that needs to know about damage/death
- `GameStateManager.Instance.State` - for any system that needs current game state
- `GameStateManager.OnStateChanged` signal - for reacting to state transitions

**Communication Pattern:**
We agreed on general C# best practices that we would follow throughout the development.
This contract allowed us to work independently and integrate smoothly

---

## Debugging Strategies Learned

### State Machine Debugging
Added debug logging to trace state transitions:
```csharp
public void ChangeState(GameState newState)
{
    GD.Print($"State changed: {oldState} -> {newState}");;  // Helped identify invalid transitions
    // ...
}
```

**Discovery:**
Found that when starting the game and clicking play wouldn't capture the mouse instantly, like it would all the other times: after clicking retry, resume, or even play from main menu if you had already played a round.
The debugging showed that I had forgotten to change the default scene from ShootingGallery (playing) to MainMenu (menu) after I had made MENU the default state in code.
Thus, when I clicked play for the first time after launching the game, the function to capture mouse wouldn't be called.
This was of course a very simple fix, but would likely take me way longer if it wasn't for the debug log explicitly showing the internal behaviour.

### UI Debugging Techniques

**Scene Tree Inspection:**
Used `get_tree().get_root().print_tree_pretty()` to debug scene structure when PauseMenu couldn't find Marksman node. Discovered the hierarchy wasn't what I assumed.

**Remote Scene Tree:**
Godot's remote inspector (while game running) showed which nodes were actually in the tree vs what I expected. Critical for debugging dynamic scene loading.

**Signal Connection Verification:**
When HPBar wasn't updating, checked Godot's Node -> Signals tab to verify connections existed.

### Settings Persistence Debugging
Manually deleted `user://settings.cfg` repeatedly to test default value handling. Tedious but caught a critical bug where sensitivity defaulted to 0 instead of 1.0, making the game unplayable.

**Lesson:**
Test edge cases (missing config file, corrupted values, first-time user experience) explicitly; don't just test the 'happy' path.

### Common Bugs Encountered

1. **Null Reference Exceptions from GetNode():**
   - **Cause:** Node didn't exist in expected hierarchy
   - **Fix:** Add null checks or use exported variables
   - **Prevention:** Minimize `GetNode()` usage; prefer signals

2. **Mouse Cursor Issues:**
   - **Cause:** Multiple scripts setting Input.MouseMode
   - **Fix:** Centralized in GameStateManager
   - **Lesson:** Global state needs single owner

3. **Signals Not Firing:**
   - **Cause:** Connected signal before node entered tree
   - **Fix:** Connect in `_EnterTree()` or use editor connections
   - **Lesson:** Order of initialization matters

---

## Video Demonstration

Video link: https://www.youtube.com/watch?v=6IH7KloS0SY

The video will showcase the following engine-integrated features (~3-4 minutes total):

### 1. Signal Connections (~90 seconds)
**What to Show:**
- Open `Health` in Godot editor
- Navigate to Health node's signals panel, show `OnDied` and `OnHealthChanged` signals
- Open `HPBar.cs`, show connection code or editor connection
- Play the game, take damage to demonstrate:
  - Health decreasing → HPBar updating in real-time
  - Death triggering → DeathPopup appearing
- **Why this matters:** Demonstrates event-driven architecture that's not visible in code alone

### 2. Mouse Mode Switching (~60 seconds)
**What to Show:**
- Start game in main menu (cursor visible and clickable)
- Click "Start Game" → cursor disappears (captured mode for FPS controls)
- Demonstrate looking around with captured mouse
- Press ESC to pause → cursor reappears (visible mode for menu interaction)
- Click resume → cursor captured again
- Die → cursor visible for death popup
- Return to menu → cursor visible
- **Why this matters:** Shows centralized Input.MouseMode management based on game state

### 3. Settings Persistence (~80 seconds)
**What to Show:**
- Play game, open options
- Change sensitivity slider to an unusual value (e.g., 2.5)
- Show file explorer with `user://` directory location
- Show `settings.cfg` file created with new value
- Close and restart game completely
- Open options, show sensitivity loaded correctly at 2.5
- **Why this matters:** Godot's ConfigFile system and user data persistence

### 4. Dynamic Scene Loading (~70 seconds)
**What to Show:**
- Open Godot's Remote Scene Tree tab (while game running)
- Show OptionsPanel not in scene tree initially
- Click options button in main menu or pause menu
- Show OptionsPanel appear in remote scene tree as child of current scene
- Close options, show node removed from tree (cleanup via `QueueFree()`)
- **Why this matters:** Runtime scene instancing and memory management

### 5. State Machine Transitions (~90 seconds)
**What to Show:**
- Add temporary debug prints to `GameStateManager.ChangeState()`:
  ```csharp
  GD.Print($"State changed: {oldState} -> {newState}");
  ```
- Play through complete state flow:
  - MENU → PLAYING (click start)
  - PLAYING → PAUSED (press ESC)
  - PAUSED → PLAYING (resume)
  - PLAYING → LOST (die)
  - LOST → MENU (return to menu)
- Show console output displaying state transitions
- **Why this matters:** FSM validation logic in action

---

## Conclusion

This project taught me fundamental game development patterns (state machines, event-driven programming, singleton pattern) and Godot-specific techniques (signals, ProcessMode, scene instancing).

**Key Takeaway:**
The most valuable lesson was learning when to strive for perfect code vs accepting pragmatic solutions. 
The DeathPopup hack is bad code, but shipping a working game was more important than refactoring everything perfectly.

**Future Improvements:**
If I were to refactor this codebase, I would:
1. Replace hardcoded scene paths with constants
2. Fix the DeathPopup workaround with proper configuration
3. Reduce tight coupling in PauseMenu with node groups or signals
4. Add proper logging system instead of GD.Print statements
5. More consistent documentation and commenting; debugging and deprecated implementations can be accessed through git history

**What I'm Most Proud Of:**
The GameStateManager architecture. It prevented numerous bugs and made the game flow more predictable and maintainable.

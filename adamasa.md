# Individual Component

## Requirements

- A video showing off the code that is tightly integrated with the game engine that is difficult to see from the text of the programming.
- An individual discussion (`username.md` or as a PDF in Inspera) covering:
  - A link to, and discussion of, code you consider good
  - A link to, and discussion of, code you consider bad
  - A personal reflection about the key concepts you learnt during the course

---

## Introduction

I was responsible for the UI-related aspects of the game.

### Files Exclusively My Contribution

- DeathPopup (all)
- GameStateManager (all)
- Health (all)
- HPBar (all)
- MainMenu (all)
- OptionsPanel (all)
- PauseMenu (all)

### Partial Contributions

- ShootingGallery (some: UI elements)
- Marksman (some: gun and recoil)

### 3D Modeling

- Desert Eagle gun modelled in Blender
- `gun.tscn` (all)

---

## Good Code

### 1. GameStateManager: State Machine Architecture Pattern

- Finite state machine implementation
- Validation checks throughout
- State transitions via `CanTransitionTo()` to prevent invalid transitions
- Uses signals to communicate between nodes (idiomatic Godot)

### 2. GameStateManager: Centralized Input Mode Management

- Game flow is centralized through this file
- Centralized input mode management prevents bugs with mouse capture
- **Pros:** Eliminates bugs related to scattered mouse mode changes (the original motivation for implementing the state machine)

### 3. Health

- Uses signals in proper Godot fashion
- Clamping logic inside the setter
- Decoupled design—Health has no knowledge of who listens to its signals

### 4. OptionsPanel

- User settings persist through `ConfigFile` (idiomatic Godot)
- Separation of concerns: distinct Load and Save operations
- Default value handling

---

## Bad Code

### 1. DeathPopup

- Likely suboptimal way of preventing the popup from disappearing when clicking outside of it (by immediately showing it when visibility is lost)
- Could probably use flags instead
- Creates technical debt since it calls `CallDeferred` instead of preventing the issue altogether

### 2. Hardcoded Paths

- Generally present around the codebase
- Easy to introduce typos
- Hard to refactor when renaming or removing parent nodes
- No compile-time checking
- **Better approach:** Use constants or export variables

### 3. PauseMenu: Tight Coupling

- Tight coupling to scene structure:
  ```csharp
  var marksman = GetParent().GetNode<Marksman>("Marksman");
  ```
- Assumes a specific parent structure
- If the scene structure changes, this will likely break
- No null check—game will crash if Marksman isn't in the scene
- **Better approach:** Node groups, signals, or dependency injection

### 4. Commented Out Debug Code

- Bloats the documentation
- Should be removed entirely or replaced with debug flags
- Use version control as history instead

---

## Key Concepts Learned

### Event-Driven Programming with Signals

- Used extensively in Health, OptionsPanel, and GameStateManager
- Enables low coupling, observer pattern, and reactive programming
- Key elements: Signal declaration with `[Signal]` and connecting via `EmitSignal()`

### State Machine for Game Flow

- Finite state machine architecture
- State validation, transitions, and handling of invalid states
- Applied to game flow control and menu systems

### Singleton Pattern

- `GameStateManager.Instance`
- Provides global state access with singleton lifecycle in Godot
- Trade-off: Convenience vs. tight coupling

### General UI/UX Design in Game Engines

- `ProcessMode.Always` for pause menus
- Mouse mode management (captured vs. visible)
- Dynamic UI loading and cleanup
- Game-specific UI challenges

### Godot Engine Specifics (Idioms)

- Scene instancing
- Node tree navigation
- Input handling
- Settings persistence
- `ProcessMode` for paused scenes

### C# Properties and Encapsulation

- Example: `Health.CurrentHealth` with private setter and validation (lines 22–38)
- Takeaways: Data hiding, validation in setters, side effects in properties

---

## Video Demonstration

The video will showcase:

1. **Signal connections:** Health to HPBar communication
2. **ProcessMode.Always:** How pause menu works while game is paused
3. **State transitions:** State machine in action (with debug prints)
4. **Dynamic scene loading:** Options panel instantiation
5. **Persistence:** Changing sensitivity settings and reloading them
6. **Mouse mode switching:** Cursor capture in PLAYING state vs. visible in menus

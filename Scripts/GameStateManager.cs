using Godot;
using System;

public enum GameState
{
    PLAYING,
    PAUSED,
    LOST,
    WON,
    MENU
}

public partial class GameStateManager : Node
{
    // Singleton instance
    public static GameStateManager Instance { get; private set; }

    // Signal for state changes
    [Signal]
    public delegate void StateChangedEventHandler(GameState oldState, GameState newState);

    private GameState _currentState = GameState.PLAYING;

    public GameState CurrentState => _currentState;

    public override void _EnterTree()
    {
        // Set up singleton
        if (Instance != null)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    public override void _Ready()
    {
        // Ensure this node persists across scene changes
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public bool CanTransitionTo(GameState newState)
    {
        // Define valid state transitions
        switch (_currentState)
        {
            case GameState.PLAYING:
                // From PLAYING, can go to PAUSED or LOST
                return newState == GameState.PAUSED || newState == GameState.LOST || newState == GameState.WON;

            case GameState.PAUSED:
                // From PAUSED, can only go back to PLAYING or to MENU
                return newState == GameState.PLAYING || newState == GameState.MENU;

            case GameState.LOST:
            case GameState.WON:
                // From LOST/WON, can only restart (PLAYING) or go to MENU
                return newState == GameState.PLAYING || newState == GameState.MENU;

            case GameState.MENU:
                // From MENU, can go to PLAYING
                return newState == GameState.PLAYING;

            default:
                return false;
        }
    }

    public void ChangeState(GameState newState)
    {
        if (_currentState == newState)
        {
            // GD.Print($"Already in state {newState}");
            return;
        }

        if (!CanTransitionTo(newState))
        {
            // GD.PrintErr($"Invalid state transition from {_currentState} to {newState}");
            return;
        }

        GameState oldState = _currentState;
        _currentState = newState;

        // GD.Print($"State changed: {oldState} -> {newState}");

        // Handle pause state based on game state
        UpdatePauseState();

        // Handle mouse mode based on game state
        UpdateMouseMode();

        // Emit signal for listeners
        EmitSignal(SignalName.StateChanged, (int)oldState, (int)newState);
    }

    private void UpdatePauseState()
    {
        // Game should be paused in PAUSED, LOST, and WON states
        // MENU and PLAYING states should not be paused (MENU needs buttons to work)
        GetTree().Paused = _currentState == GameState.PAUSED ||
                          _currentState == GameState.LOST ||
                          _currentState == GameState.WON;
    }

    private void UpdateMouseMode()
    {
        // Show mouse cursor in all states except PLAYING
        if (_currentState == GameState.PLAYING)
            Input.MouseMode = Input.MouseModeEnum.Captured;
        else
            Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public bool IsPlaying() => _currentState == GameState.PLAYING;
    public bool IsPaused() => _currentState == GameState.PAUSED;
    public bool IsLost() => _currentState == GameState.LOST;
    public bool IsWon() => _currentState == GameState.WON;
    public bool IsInMenu() => _currentState == GameState.MENU;
}

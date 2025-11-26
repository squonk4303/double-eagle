using Godot;
using System;

public partial class DeathPopup : PopupPanel
{
    public override void _Ready()
    {
        // Hide the popup initially
        Hide();

        // Connect button signals
        GetNode<Button>("VBoxContainer/RetryButton").Pressed += OnRetryPressed;
        GetNode<Button>("VBoxContainer/MainMenuButton").Pressed += OnMainMenuPressed;

        // Interactable when paused
        ProcessMode = ProcessModeEnum.Always;

        // Prevent popup from closing when clicking outside
        PopupHide += OnPopupHideAttempt;
    }

    private void OnPopupHideAttempt()
    {
        // If in LOST state and popup tries to hide (from clicking outside),
        // immediately show it again. Probably a better way to do this...
        if (GameStateManager.Instance?.IsLost() == true)
        {
            CallDeferred(MethodName.Show);
        }
    }

    public void ShowDeathMenu()
    {
        var stateManager = GameStateManager.Instance;
        if (stateManager != null)
            stateManager.ChangeState(GameState.LOST);

        PopupCentered();
    }

    private void OnRetryPressed()
    {
        Hide();
        var stateManager = GameStateManager.Instance;
        if (stateManager != null)
            stateManager.ChangeState(GameState.PLAYING);

        GetTree().ReloadCurrentScene();
    }

    private void OnMainMenuPressed()
    {
        Hide();
        var stateManager = GameStateManager.Instance;
        if (stateManager != null)
            stateManager.ChangeState(GameState.MENU);

        GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
    }
}
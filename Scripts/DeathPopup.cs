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

        // Must use buttons
        Exclusive = true;
    }

    public void ShowDeathMenu()
    {
        PopupCentered();
        // Cursor mode
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().Paused = true;
    }

    private void OnRetryPressed()
    {
        Hide();
        GetTree().Paused = false;
        GetTree().ReloadCurrentScene();
    }

    private void OnMainMenuPressed()
    {
        Hide();
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn"); 
    }
}
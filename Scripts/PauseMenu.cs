using Godot;
using System;

public partial class PauseMenu : Control
{
    public override void _Ready()
    {
        GD.Print("PauseMenu ready");

        GetNode<Button>("VBoxContainer/ResumeButton").Pressed += OnResumePressed;
        GetNode<Button>("VBoxContainer/OptionsButton").Pressed += OnOptionsPressed;
        GetNode<Button>("VBoxContainer/MainMenuButton").Pressed += OnMainMenuPressed;

        Visible = false; // Use property instead
        ProcessMode = ProcessModeEnum.Always; // Ensure it processes when paused
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ESCAPE")) // Temporary keybind for testing (U)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        // Toggle mouse visibility and pause state
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Visible = !Visible;
        GetTree().Paused = !GetTree().Paused;
    }

    private void OnResumePressed()
    {
        TogglePause();
    }

    private void OnOptionsPressed()
    {
        // Open options
    }

    private void OnMainMenuPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
    }
}

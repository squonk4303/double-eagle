using Godot;
using System;

public partial class PauseMenu : Control
{
    // Exported nodes for easy access and customization
    [Export] private VBoxContainer pauseButtons;
    [Export] private Control optionsContainer;

    public override void _Ready()
    {
        // Connect button signals
        GetNode<Button>("PauseButtons/ResumeButton").Pressed += OnResumePressed;
        GetNode<Button>("PauseButtons/OptionsButton").Pressed += OnOptionsPressed;
        GetNode<Button>("PauseButtons/MainMenuButton").Pressed += OnMainMenuPressed;

        Visible = false;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ESCAPE"))
            TogglePause();
    }

    public void TogglePause()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Visible = !Visible;
        GetTree().Paused = !GetTree().Paused;
        pauseButtons.Visible = true;

        if (optionsContainer != null && optionsContainer.GetChildCount() > 0)
        {
            foreach (Node child in optionsContainer.GetChildren())
                child.QueueFree();
        }
    }

    private void OnResumePressed() => TogglePause();

    private void OnOptionsPressed()
    {
        // Load and instance optios_panel scene
        var optionsScene = GD.Load<PackedScene>("res://Scenes/options_panel.tscn");
        var options = optionsScene.Instantiate<OptionsPanel>();
        optionsContainer.AddChild(options);
        pauseButtons.Visible = false;

        // Connect sensitivity signal to Marksman
        var marksman = GetParent().GetNode<Marksman>("Marksman");
        options.SensitivityChanged += marksman.OnSensitivityChanged;
    }

    private void OnMainMenuPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
    }
}

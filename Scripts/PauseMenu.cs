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
        // Ensure menu proecesses even when game is paused
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ESCAPE"))
            TogglePause();
    }

    public void TogglePause()
    {
        // Toggle (in)visiblity and (un)pause
        Visible = !Visible;
        bool isPaused = !GetTree().Paused;
        GetTree().Paused = isPaused;
        if (isPaused)
        {
            // Pausing - show cursor
            Input.MouseMode = Input.MouseModeEnum.Visible;
            pauseButtons.Visible = true;
        }
        else
        {
            // Unpausing - capture mouse
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        // Clear options panel if it exists
        if (optionsContainer != null && optionsContainer.GetChildCount() > 0)
        {
            foreach (Node child in optionsContainer.GetChildren())
                child.QueueFree();
        }
    }

    private void OnResumePressed() => TogglePause();

    private void OnOptionsPressed()
    {
        GD.Print("Options pressed in PauseMenu");
        // Load and instance optios_panel scene
        var optionsScene = GD.Load<PackedScene>("res://Scenes/options_panel.tscn");
        var options = optionsScene.Instantiate<OptionsPanel>();
        optionsContainer.AddChild(options);
        pauseButtons.Visible = false;

        // Connect sensitivity signal to Marksman
        var marksman = GetParent().GetNode<Marksman>("Marksman");
        options.SensitivityChanged += marksman.OnSensitivityChanged;

        options.BackPressed += OnOptionsBack;
    }

    private void OnOptionsBack()
    {
        GD.Print("Back pressed in OptionsPanel from PauseMenu");
        pauseButtons.Visible = true;
    }

    private void OnMainMenuPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
    }
}

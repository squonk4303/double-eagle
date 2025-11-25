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
        GetNode<Button>("PauseButtons/RetryButton").Pressed += OnRetryPressed;
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
        var stateManager = GameStateManager.Instance;
        if (stateManager == null)
        {
            GD.PrintErr("GameStateManager instance not found!");
            return;
        }

        // Only allow toggling pause from PLAYING or PAUSED states
        if (stateManager.IsPlaying())
        {
            // Transition to PAUSED
            stateManager.ChangeState(GameState.PAUSED);
            Visible = true;
            pauseButtons.Visible = true;
        }
        else if (stateManager.IsPaused())
        {
            // Transition back to PLAYING
            stateManager.ChangeState(GameState.PLAYING);
            Visible = false;

            // Clear options panel if it exists
            if (optionsContainer != null && optionsContainer.GetChildCount() > 0)
            {
                foreach (Node child in optionsContainer.GetChildren())
                    child.QueueFree();
            }
        }
        // If in LOST or WON state, ignore pause input
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

        // Connect back-signal
        options.BackPressed += OnOptionsBack;
    }

    private void OnOptionsBack()
    {
        GD.Print("Back pressed in OptionsPanel from PauseMenu");
        pauseButtons.Visible = true;
    }

    private void OnRetryPressed()
    {
        var stateManager = GameStateManager.Instance;
        if (stateManager != null)
            stateManager.ChangeState(GameState.PLAYING);

        GetTree().ReloadCurrentScene();
    }

    private void OnMainMenuPressed()
    {
        var stateManager = GameStateManager.Instance;
        if (stateManager != null)
            stateManager.ChangeState(GameState.MENU);

        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
    }
}

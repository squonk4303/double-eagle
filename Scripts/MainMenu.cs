using Godot;
using System;

public partial class MainMenu : Control
{
    // Exported nodes for easy access and customization
    [Export] private VBoxContainer mainButtons;
    [Export] private Control optionsContainer;

    public override void _Ready()
    {
        GetNode<Button>("MarginContainer/MainButtons/PlayButton").Pressed += OnPlayPressed;
        GetNode<Button>("MarginContainer/MainButtons/OptionsButton").Pressed += OnOptionsPressed;
        GetNode<Button>("MarginContainer/MainButtons/QuitButton").Pressed += OnQuitPressed;

        // Set initial state to MENU when main menu loads
        var stateManager = GameStateManager.Instance;
        if (stateManager != null)
            stateManager.ChangeState(GameState.MENU);
    }

    private void OnPlayPressed()
    {
        GD.Print("Play pressed");
        var stateManager = GameStateManager.Instance;
        if (stateManager != null)
            stateManager.ChangeState(GameState.PLAYING);

        GetTree().ChangeSceneToFile("res://Scenes/shooting_gallery.tscn");
    }

    private void OnOptionsPressed()
    {
        var optionsScene = GD.Load<PackedScene>("res://Scenes/options_panel.tscn");
        var options = optionsScene.Instantiate<OptionsPanel>();
        optionsContainer.AddChild(options);
        mainButtons.Visible = false;

        // No need to connect Marksman in main menu since it doesn't exist here; no need for real time updates

        // Connect back-signal
        options.BackPressed += OnOptionsBack;

    }

    private void OnOptionsBack()
    {
        GD.Print("Back pressed in OptionsPanel from MainMenu");
        mainButtons.Visible = true;

        // Clear the options container
        if (optionsContainer != null && optionsContainer.GetChildCount() > 0)
        {
            foreach (Node child in optionsContainer.GetChildren())
                child.QueueFree();
        }
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}

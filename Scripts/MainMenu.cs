using Godot;
using System;

public partial class MainMenu : Control
{
    // Exported nodes for easy access and customization
    [Export] private VBoxContainer mainButtons;
    [Export] private Control optionsContainer;

    public override void _Ready()
    {
        GetNode<Button>("MainButtons/PlayButton").Pressed += OnPlayPressed;
        GetNode<Button>("MainButtons/OptionsButton").Pressed += OnOptionsPressed;
        GetNode<Button>("MainButtons/QuitButton").Pressed += OnQuitPressed;
    }

    private void OnPlayPressed()
    {
        GD.Print("Play pressed");
        GetTree().ChangeSceneToFile("res://Scenes/shooting_gallery.tscn");
    }

    private void OnOptionsPressed()
    {
        var optionsScene = GD.Load<PackedScene>("res://Scenes/options_panel.tscn");
        var options = optionsScene.Instantiate<OptionsPanel>();
        optionsContainer.AddChild(options);
        mainButtons.Visible = false;

        var marksman = GetParent().GetNode<Marksman>("Marksman");
        options.SensitivityChanged += marksman.OnSensitivityChanged;

        // Connect back signal
        options.BackPressed += OnOptionsBack;
    }

    private void OnOptionsBack()
    {
        GD.Print("Back pressed in OptionsPanel from MainMenu");
        mainButtons.Visible = true;
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}

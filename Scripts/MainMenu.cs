using Godot;
using System;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        GetNode<Button>("VBoxContainer/PlayButton").Pressed += OnPlayPressed;
        GetNode<Button>("VBoxContainer/OptionsButton").Pressed += OnOptionsPressed;
        GetNode<Button>("VBoxContainer/QuitButton").Pressed += OnQuitPressed;
    }

    private void OnPlayPressed()
    {
        GD.Print("Play pressed");
        GetTree().ChangeSceneToFile("res://Scenes/shooting_gallery.tscn");
    }

    private void OnOptionsPressed()
    {
        // Open options menu
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}

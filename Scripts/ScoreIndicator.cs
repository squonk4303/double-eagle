using Godot;
using System;

public partial class ScoreIndicator : Node2D
{
    private Label _label;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");

        // Play animation
        var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("float-up");
        // Kill when animation finishes
        animationPlayer.AnimationFinished += (StringName _) => QueueFree();
    }

    public void SetText(string txt)
    {
        if (_label != null)
        {
            _label.Text = txt;
        }
    }
}

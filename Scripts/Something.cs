using Godot;
using System;


public partial class Something : Node3D
{
    [Export]
    public PackedScene ball_scene;

    [Export]
    public PackedScene bullet_scene;

    public override void _Ready()
    {
        var timer = GetNode<Timer>("BallTimer");
        // Connects signal to function
        timer.Timeout += OnBallTimerTimeout;
    }

    private void OnBallTimerTimeout()
    {
        GD.Print("Signal activated");
        var ball = ball_scene.Instantiate();
        // TODO: Switch back to random once ball is C#
        // var z_pos = rand.Next(5);
        var spawnLocation = new Vector3(-5, 0, 5);
        // ball.initialize(spawnLocation);
    }
}

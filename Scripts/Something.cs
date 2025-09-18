using Godot;
using System;


public partial class Something : Node3D
{
    public override void _Ready()
    {
        var timer = GetNode<Timer>("BallTimer");
        // Connects signal to function
        timer.Timeout += OnBallTimerTimeout;
    }

    /// Spawn a ball with velocity
    private void OnBallTimerTimeout()
    {
        PackedScene scene = GD.Load<PackedScene>("res://Scenes/ball.tscn");
        Ball ball = scene.Instantiate() as Ball;
        AddChild(ball);

        // TODO: Switch back to random once ball is C#
        // var z_pos = rand.Next(5);
        var spawnLocation = new Vector3(-5, 0, 5);

        ball.SetUp(spawnLocation);
    }
}

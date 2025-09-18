using Godot;
using System;


public partial class Something : Node3D
{
    public override void _Ready()
    {
        var timer = GetNode<Timer>("BallTimer");
        var marksman = GetNode<Marksman>("Marksman");
        // Connects signal to function
        timer.Timeout += OnBallTimerTimeout;
        marksman.FireGun00 += OnFireGun00;
    }

    /// Spawn a ball with velocity
    private void OnBallTimerTimeout()
    {
        PackedScene scene = GD.Load<PackedScene>("res://Scenes/ball.tscn");
        Ball ball = scene.Instantiate() as Ball;
        AddChild(ball);

        // Randomizes the z-position for a bit o variety
        // TODO: Would prefer to use C#'s Random class.
        var z_pos = GD.Randi() % 5;
        var spawnLocation = new Vector3(-5.0f, 0.0f, z_pos);

        ball.SetUp(spawnLocation);
    }

    private void OnFireGun00(Vector3 position, Vector3 rotation)
    {
        GD.Print("Firing @ ", position, rotation);
        PackedScene scene = GD.Load<PackedScene>("res://Scenes/bullet.tscn");
        Bullet bullet = scene.Instantiate() as Bullet;
        bullet.Initialize(position, rotation);
        AddChild(bullet);
    }
}

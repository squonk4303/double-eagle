using Godot;
using System;


public partial class ShootingGallery : Node3D
{
    private const string PATH_BALL = "res://Scenes/ball.tscn";
    private const string PATH_BULLET = "res://Scenes/bullet.tscn";

    public override void _Ready()
    {
        // Connect signals from periodic spawn-timer
        // and marksman firing weapon
        Timer timer = GetNode<Timer>("BallTimer");
        timer.Timeout += OnBallTimerTimeout;
        Marksman marksman = GetNode<Marksman>("Marksman");
        marksman.FireGun00 += OnFireGun00;
    }

    /// Spawn a ball at a certain location
    private void OnBallTimerTimeout()
    {
        // Add ball to node tree
        PackedScene scene = GD.Load<PackedScene>(PATH_BALL);
        Ball ball = scene.Instantiate() as Ball;
        AddChild(ball);

        // Randomizes the z-position for a bit o variety
        // TODO: Would prefer to use C#'s Random class.
        var z_pos = GD.Randi() % 5;
        var spawnLocation = new Vector3(-5.0f, 0.0f, z_pos);

        // Set ball position and add some force
        ball.Initialize(spawnLocation);
    }

    /// Spawn a bullet wherever is demanded
    private void OnFireGun00(Vector3 position, Vector3 rotation)
    {
        // Loads, instantiates, and spawns bullet
        PackedScene scene = GD.Load<PackedScene>(PATH_BULLET);
        Bullet bullet = scene.Instantiate() as Bullet;
        bullet.Initialize(position, rotation);
        AddChild(bullet);
    }
}

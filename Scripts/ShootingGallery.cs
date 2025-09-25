using Godot;
using System;


public partial class ShootingGallery : Node3D
{
    private const string PATH_BALL = "res://Scenes/ball.tscn";
    private const string PATH_BULLET = "res://Scenes/bullet.tscn";
    private const string PATH_LASER = "res://Scenes/laser.tscn";

    private readonly string[] PATH_LOCATIONS = new string[]
    {
        "Spawns/Path0/Stretch",
        "Spawns/Path1/Stretch",
    };

    public override void _Ready()
    {
        // Connect signals from periodic spawn-timer
        // and marksman firing weapon
        Timer timer = GetNode<Timer>("BallTimer");
        timer.Timeout += OnBallTimerTimeout;
        Marksman marksman = GetNode<Marksman>("Marksman");
        marksman.GunFire00 += OnGunFire00;
        marksman.GunFireRay += OnGunFireRay;
    }

    /// Spawn a ball at a random location
    private void OnBallTimerTimeout()
    {
        PackedScene ballScene = GD.Load<PackedScene>(PATH_BALL);
        var ball = ballScene.Instantiate() as Ball;
        var path = GetNode<PathFollow3D>(PATH_LOCATIONS[0]);

        // Picks a random spot on the path to spawn.
        // Randomizes the z-position a bit too.
        // TODO: Would prefer to use C#'s Random class (perhaps for seed control?).
        var zJiggle = GD.Randf() * 4.0f + 1.0f;
        path.ProgressRatio = GD.Randf();
        // Randomly flip the Y-coordinate
        float flipper = (GD.Randi() % 2 - 0.5f) * 2.0f;
        var spawn = new Vector3(path.Position.X * flipper, path.Position.Y, zJiggle);
        var target = new Vector3(0, 7.0f, 0);
        ball.Initialize(spawn, target);
        AddChild(ball);
    }

    /// Spawn a bullet wherever is demanded
    private void OnGunFire00(Vector3 position, Vector3 rotation)
    {
        // Loads, instantiates, and spawns bullet
        PackedScene scene = GD.Load<PackedScene>(PATH_BULLET);
        Bullet bullet = scene.Instantiate() as Bullet;
        bullet.Initialize(position, rotation);
        AddChild(bullet);
    }

    private void OnGunFireRay(Vector3 position, Vector3 rotation)
    {
        var scene = GD.Load<PackedScene>(PATH_LASER);
        Node3D ray = scene.Instantiate() as Node3D;
        ray.Position = position;
        ray.Rotation = rotation;

        AddChild(ray);

        // PackedScene scene = GD.Load<PackedScene>(PATH_BULLET);
        // Bullet bullet = scene.Instantiate() as Bullet;
        // bullet.Initialize(position, rotation);
        // AddChild(bullet);
    }
}

using Godot;
using System;


public partial class ShootingGallery : Node3D
{
    private const string PATH_BULLET = "res://Scenes/bullet.tscn";
    private const string PATH_LASER = "res://Scenes/laser.tscn";
    private const string PATH_BGM = "res://Audio/621216__nlux__yp-plague-drone-loop-06.wav";

    private readonly string[] PATH_BALLS = new string[]
    {
        "res://Scenes/small_ball.tscn",
        "res://Scenes/watermelon.tscn",
    };

    private readonly string[] PATH_LOCATIONS = new string[]
    {
        "Spawns/Path0/Stretch",
        "Spawns/Path1/Stretch",
    };

    private AudioStreamPlayer _audioPlayer;

    public override void _Ready()
    {
        // Connect signals from periodic spawn-timer
        // and marksman firing weapon
        Timer timer = GetNode<Timer>("BallTimer");
        timer.Timeout += OnBallTimerTimeout;
        Marksman marksman = GetNode<Marksman>("Marksman");
        marksman.GunFire00 += OnGunFire00;
        marksman.GunFireRay += OnGunFireRay;

        // Set music to play through non-positional audio player
        _audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        _audioPlayer.Stream = GD.Load<AudioStream>(PATH_BGM);
        // Restart playback on track finish.
        _audioPlayer.Finished += () => _audioPlayer.Play();
        _audioPlayer.VolumeDb = -100.0f;  // TODO: Inaudible until useful.
        _audioPlayer.Play();
    }

    private PackedScene GetRandomBall()
    {
        // Load random ball asset
        // ...It's uniform randomness, which is not quite right
        PackedScene ballScene = GD.Load<PackedScene>(
            PATH_BALLS[GD.Randi() % PATH_BALLS.Length]
        );

        return ballScene;
    }

    private Vector3 GetSpawnPosition()
    {
        var path = GetNode<PathFollow3D>(PATH_LOCATIONS[0]);

        // Picks a random spot on the path to spawn.
        // TODO: Would prefer to use C#'s Random class (perhaps for seed control?).
        path.ProgressRatio = GD.Randf();
        // Randomly flip the Y-coordinate
        float flipper = (GD.Randi() % 2 - 0.5f) * 2.0f;
        return new Vector3(path.Position.X * flipper, path.Position.Y, 0.0f);
    }

    /// Spawn a ball at a random location
    private void OnBallTimerTimeout()
    {
        var ball = GetRandomBall().Instantiate() as Ball;
        Vector3 spawn = GetSpawnPosition();
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
    }
}

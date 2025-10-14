using Godot;
using System;
using System.Collections.Generic;  // For List


public partial class ShootingGallery : Node3D
{
    private const string PATH_BULLET = "res://Scenes/bullet.tscn";
    private const string PATH_LASER = "res://Scenes/laser.tscn";
    private const string PATH_BGM = "res://Audio/621216__nlux__yp-plague-drone-loop-06.wav";

    private const double MAX_HP = 100.0d;

    private readonly string[] PATH_BALLS = new string[]
    {
        "res://Scenes/balloon.tscn",
        "res://Scenes/small_ball.tscn",
        "res://Scenes/watermelon.tscn",
    };

    private readonly string[] PATH_LOCATIONS = new string[]
    {
        "Spawns/Path0/Stretch",
        "Spawns/Path1/Stretch",
    };

    // A queue (shuffled elsewhere) which determines rate of balls spawning
    private string[] _ballQueueTemplate = new string[]
    {
        "res://Scenes/balloon.tscn",
        "res://Scenes/balloon.tscn",
        "res://Scenes/watermelon.tscn",
        "res://Scenes/small_ball.tscn",
        "res://Scenes/small_ball.tscn",
        "res://Scenes/small_ball.tscn",
        "res://Scenes/small_ball.tscn",
        "res://Scenes/small_ball.tscn",
        "res://Scenes/small_ball.tscn",
        "res://Scenes/small_ball.tscn",
    };

    private List<string> _ballQueue = new List<string>();
    private AudioStreamPlayer _audioPlayer;

    private Health _health;
    private HPBar _hpBar;
    private Label _healthLabel;

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

        // Connect misc. signals
        var falloutZone = GetNode<Fallout>("FalloutZone");
        falloutZone.BodyFellOut += OnBodyFellOut;

        // Get health component
        _health = GetNode<Health>("Health");

        // Connect health signals
        _health.HealthChanged += OnHealthChanged;
        _health.Died += OnPlayerDied;

        // Get HUD elements
        _hpBar = GetNode<HPBar>("HeadsUpDisplay/HPBar");

        // Initialize HP-HUD
        _healthLabel = GetNode<Label>("HeadsUpDisplay/Health");

    }

    /// Loads balls in from a randomized queue
    private PackedScene LoadRandomBall()
    {
        PackedScene ballScene;

        // When queue is empty
        if (_ballQueue.Count == 0)
        {
            // Reload queue with a shuffled template
            Random.Shared.Shuffle(_ballQueueTemplate);
            _ballQueue = new List<string>(_ballQueueTemplate);

            // Load and return one with uniform randomness
            // For a bit o' spice
            ballScene = GD.Load<PackedScene>(
                PATH_BALLS[GD.Randi() % PATH_BALLS.Length]
            );
            return ballScene;
        }
        else
        {
            // Load one item and remove it from the queue
            ballScene = GD.Load<PackedScene>(_ballQueue[0]);
            _ballQueue.RemoveAt(0);
        }

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

    /** Old health management code, now in Health.cs
    private void AddHealth(double value)
    {
        // Update health value
        _health = Math.Clamp(_health + value, -MAX_HP, MAX_HP);

        // Respond to certain health values
        if (_health > 0)
        {
            _healthLabel.Text = $"HP: {_health}";
        }
        else
        {
            // TODO: Implement game over mechanics
            //       This right now doesn't even lock HP
            _healthLabel.Text = "Game over, man...";
            // Cool game-over effects
            Engine.TimeScale = 0.2;
            var light = GetNode<DirectionalLight3D>("DirectionalLight3D");
            light.Rotation = new Vector3(0, 1.0f, 0);
            light.LightColor = new Color("darkred");
        }
    }
    */

    /// Spawn a ball at a random location
    private void OnBallTimerTimeout()
    {
        var ball = LoadRandomBall().Instantiate();
        Vector3 spawn = GetSpawnPosition();
        var target = new Vector3(0, 7.0f, 0);
        ball.Call("Initialize", spawn, target);
        AddChild(ball);
    }

    /// Spawn a bullet wherever is demanded
    private void OnGunFire00(Vector3 position, Vector3 rotation)
    {
        // Cost health for shootin
        _health.TakeDamage(1.0f);

        // Loads, instantiates, and spawns bullet
        PackedScene scene = GD.Load<PackedScene>(PATH_BULLET);
        Bullet bullet = scene.Instantiate() as Bullet;
        bullet.Initialize(position, rotation);
        // Connect Bullet's signal to ShootingGallery's slot
        bullet.Connect("BulletReport", Callable.From(OnBulletReport));

        AddChild(bullet);
    }

    private void OnGunFireRay(Vector3 position, Vector3 rotation)
    {
        // Loads, instantiates, and spawns laser
        var scene = GD.Load<PackedScene>(PATH_LASER);
        Node3D laser = scene.Instantiate() as Node3D;
        laser.Position = position;
        laser.Rotation = rotation;
        // Connect Laser's signal to ShootingGallery's slot
        laser.Connect(
            "LaserReport",
            // Make a callable From this lambda
            Callable.From(
                (Godot.Collections.Array<CollisionObject3D> targets) => OnLaserReport(targets)
            )
        );

        AddChild(laser);
    }

    private void OnBulletReport()
    {
        // Refund the health lost by shooting in the first place
        _health.Heal(1.0f);
    }

    private void OnLaserReport(Godot.Collections.Array<CollisionObject3D> targets)
    {
        // Add/remove health based on how many targets hit with laser
        // TODO: Bug where hitting a bullet will count positively here
        //       And the world boundary... which is infinitely wide....
        // $$ f(x) = 5x^{1.5} - 4 $$
        // Hits: f(0) = -4; f(1) = 1; f(2) = 10.14; f(3) = 21.98;
        var f = (int x) => (float)Math.Round(5 * Math.Pow(x, 1.5) - 4);
        _health.Heal(f(targets.Count));
    }

    private void OnBodyFellOut(Node3D body)
    {
        // Remove health when balls fall out of reach
        if (body is Watermelon)
        {
            _health.TakeDamage(3.0f);
        }
        else if (body is Balloon)
        {
            _health.TakeDamage(2.0f);
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        // Update all UI elements
        _hpBar.UpdateHealth(current, max);
        _healthLabel.Text = $"HP: {current}";
    }

    private void OnPlayerDied()
    {
        _healthLabel.Text = "Game over, man...";
        Engine.TimeScale = 0.2;
        var light = GetNode<DirectionalLight3D>("DirectionalLight3D");
        light.Rotation = new Vector3(0, 1.0f, 0);
        light.LightColor = new Color("darkred");
    }
}

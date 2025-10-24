using Godot;
using System;
using System.Collections.Generic;  // For List and Dictionary
using System.Linq;  // for [].Count


public partial class ShootingGallery : Node3D
{
    [Export] private CanvasLayer headsUpDisplay;
    [Export] private PauseMenu pauseMenu;
   
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

    // A queue for ball types to spawn in
    private List<string> _spawnQueue = new List<string>();

    private AudioStreamPlayer _audioPlayer;

    private Health _health;
    private HPBar _hpBar;
    private Label _healthLabel;
    private int _score = 0;
    public bool IsDead = false;

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

        // Connect to the PauseMenu's visibility changes
        if (pauseMenu != null)
        {
            pauseMenu.VisibilityChanged += OnPauseMenuVisibilityChanged;
        }
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

    /// Create a transient popup over the center of the screen
    private void ScorePopup(string txt)
    {
        var hud = GetNode<CanvasLayer>("HeadsUpDisplay");
        PackedScene scene = GD.Load<PackedScene>("res://Scenes/score_indicator.tscn");
        var popup = scene.Instantiate<ScoreIndicator>();
        // Add as child early to call _Ready &c
        hud.AddChild(popup);

        // Set position based on center of screen
        Vector2 position = GetViewport().GetVisibleRect().Size * 0.5f;
        position += new Vector2(45.0f, 10.0f);

        popup.SetText(txt);
        popup.Position = position;
    }

    private void AddScore(float n)
    {
        if (!IsDead)
        {
            var label = GetNode<Label>("HeadsUpDisplay/ScoreDisplay");
            _score += (int)Math.Round(n);
            string txt;
            txt = _score.ToString("Score: 00000");
            label.Text = txt;
        }
    }

    /// Load next ball in queue
    private PackedScene LoadNextBall()
    {
        string path;
        // Get next in queue for balls
        // If there is none, get a smallball
        if (_spawnQueue.Count > 0)
        {
            path = _spawnQueue[0];
            _spawnQueue.RemoveAt(0);
        }
        else
        {
            path = "res://Scenes/small_ball.tscn";
        }
        var scene = GD.Load<PackedScene>(path);
        return scene;
    }

    /// Spawn a ball at a random location
    private void OnBallTimerTimeout()
    {
        // Load a ball from the randomized queue
        PackedScene ballScene = LoadNextBall();
        var ball = ballScene.Instantiate();
        Vector3 spawn = GetSpawnPosition();
        // target here is a bit over the middle of the backboard
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

    /// Add/remove health based on how many valid targets hit with laser
    private void OnLaserReport(Godot.Collections.Array<CollisionObject3D> targets)
    {
        // $$ f(x) = 5x^{1.5} - 4 $$
        // f(0) = -4; f(1) = 1; f(2) = 10.14; f(3) = 21.98;
        var f = (int x) => (float)Math.Round(5 * Math.Pow(x, 1.5) - 4);
        // Count targets which inherit Ball
        int count = targets.Count(e => e is Ball);
        _health.Heal(f(count));
        if (f(count) > 0.0f)
        {
            AddScore(f(count) * 10.0f);
        }

        // Notify player of score
        ScorePopup($"{f(count)} POINTS");

        if (count == 1)
        {
            _spawnQueue.Add("res://Scenes/watermelon.tscn");
        }
        else if (count >= 2)
        {
            _spawnQueue.Add("res://Scenes/balloon.tscn");
        }
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
        IsDead = true;
        _healthLabel.Text = "Game over, man...";
        Engine.TimeScale = 0.2;
        var light = GetNode<DirectionalLight3D>("DirectionalLight3D");
        light.Rotation = new Vector3(0, 1.0f, 0);
        light.LightColor = new Color("darkred");
    }

    private void OnPauseMenuVisibilityChanged()
    {
        // Turn HUD off when pause menu is visible, or vice versa
        headsUpDisplay.Visible = !pauseMenu.Visible;
    }
}

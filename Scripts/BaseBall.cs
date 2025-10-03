using Godot;
using System;

public partial class Ball : RigidBody3D
{
    private const float COLLISION_FORCE = 1200.0f;
    private const float ENTRY_FORCE = 60.0f;
    private const string BULLET_HIT_SFX = "res://Audio/582265__rocketpancake__justa-slap-smack.wav";
    private const string LASER_HIT_SFX = "res://Audio/789793__quatricise__pop-4.wav";

    private AnimationPlayer _animation;
    private AudioStreamPlayer3D _audioPlayer;
    private AudioStream _bulletHitSfx;
    private AudioStream _laserHitSfx;
    private int _timesHit = 0;

    public override void _Ready()
    {
        _audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
        _animation = GetNode<AnimationPlayer>("AnimationPlayer");
        _animation.SpeedScale = 4;

        // Load sfx resources
        _bulletHitSfx = GD.Load<AudioStream>(BULLET_HIT_SFX);
        _laserHitSfx = GD.Load<AudioStream>(LASER_HIT_SFX);
    }

    /// Prepare for getting spawned in
    public void Initialize(Vector3 spawn, Vector3 target)
    {
        Position = spawn;

        // --- Get force with which to spawn ---
        Vector3 difference = target - spawn;
        // TODO: All this does is get a higher angle. There should be a way
        // to get the same result in a more controlled way.
        difference.Z = 0.0f;
        difference *= 0.5f;
        difference.Y += 3.0f;
        ApplyForce(difference * (ENTRY_FORCE + GD.Randf() * 40.0f));

        // --- Make up an angle to spawn with ---
        Rotation = difference.Normalized();
    }

    /// How to respond when hit by a bullet
    public void bulletHit(Vector3 b_pos)
    {
        // Get direction from center of bullet to center of self
        Vector3 direction = b_pos.DirectionTo(GlobalPosition);
        // Suppress force applied in z-direction
        // NOTE: This denormalizes the vector
        direction.Z = 0.0f;
        ApplyForce(direction * COLLISION_FORCE);

        // Select and playe sfx
        _audioPlayer.Stream = _bulletHitSfx;
        // Randomize pitch [0.5, 2.0]
        _audioPlayer.PitchScale = GD.Randf() * 1.5f + 0.5f;
        _audioPlayer.VolumeDb = 00.0f;
        _audioPlayer.Play();
    }

    public void LaserHit()
    {
        if (_timesHit <= 0)
        {
            _animation.Play("turn_red");

            // Select and playe sfx
            _audioPlayer.Stream = _laserHitSfx;
            // Randomize pitch [0.5, 2.0]
            _audioPlayer.PitchScale = GD.Randf() * 1.5f + 0.5f;
            _audioPlayer.VolumeDb = 70.0f;
            _audioPlayer.Play();
        }
        _timesHit += 1;
    }

    public override void _Process(double delta)
    {
        // Despawn once low enough
        if (Position.Y <= -400.0f)
        {
            QueueFree();
        }
    }
}

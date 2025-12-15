using Godot;
using System;

public partial class Ball : RigidBody3D
{
    private const string BULLET_HIT_SFX = "res://Audio/582265__rocketpancake__justa-slap-smack.wav";
    private const string LASER_HIT_SFX = "res://Audio/789793__quatricise__pop-4.wav";

    private AnimationPlayer _animation;
    private AudioStreamPlayer3D _audioPlayer;
    private AudioStream _bulletHitSfx;
    private AudioStream _laserHitSfx;
    private int _timesHit = 0;

    [Export] public float AnimationSpeedScale = 4;
    [Export] public float CollisionForce = 1200.0f;
    [Export] public float EntryForceFactor = 60.0f;
    [Export] public float EntryForceConstant = 40.0f;
    [Export] public float PitchFactor = 1.5f;
    [Export] public float PitchConstant = 0.5f;

    // Slow effect settings
    [Export] public float LaserSlowDurationSec = 2.0f;
    [Export] public float LaserSlowVelocityMultiplier = 0.25f;
    [Export] public float LaserSlowExtraDamp = 6.0f;

    private uint _slowToken = 0;
    private float _baseLinearDamp;
    private float _baseAngularDamp;

    public override void _Ready()
    {
        _audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
        _animation = GetNode<AnimationPlayer>("AnimationPlayer");
        _animation.SpeedScale = AnimationSpeedScale;

        _bulletHitSfx = GD.Load<AudioStream>(BULLET_HIT_SFX);
        _laserHitSfx = GD.Load<AudioStream>(LASER_HIT_SFX);

        // Capture original physics values once
        _baseLinearDamp = LinearDamp;
        _baseAngularDamp = AngularDamp;
    }

    public void bulletHit(Vector3 b_pos)
    {
        Vector3 direction = b_pos.DirectionTo(GlobalPosition);
        direction.Z = 0.0f;
        ApplyForce(direction * CollisionForce);

        _audioPlayer.Stream = _bulletHitSfx;
        _audioPlayer.PitchScale = PitchConstant + GD.Randf() * PitchFactor;
        _audioPlayer.VolumeDb = 0.0f;
        _audioPlayer.Play();
    }

    public void LaserHit()
    {
        if (_timesHit <= 0)
        {
            _animation.Play("turn_red");

            _audioPlayer.Stream = _laserHitSfx;
            _audioPlayer.PitchScale = PitchConstant + GD.Randf() * PitchFactor;
            _audioPlayer.VolumeDb = 70.0f;
            _audioPlayer.Play();

            CollisionLayer = 0x0000;
        }

        _timesHit += 1;

        ApplyLaserSlow();
    }

    private async void ApplyLaserSlow()
    {
        _slowToken++;
        uint myToken = _slowToken;

        LinearVelocity *= LaserSlowVelocityMultiplier;
        AngularVelocity *= LaserSlowVelocityMultiplier;

        LinearDamp = _baseLinearDamp + LaserSlowExtraDamp;
        AngularDamp = _baseAngularDamp + LaserSlowExtraDamp;

        await ToSignal(GetTree().CreateTimer(LaserSlowDurationSec),
                       SceneTreeTimer.SignalName.Timeout);

        if (myToken != _slowToken) return;

        LinearDamp = _baseLinearDamp;
        AngularDamp = _baseAngularDamp;
    }

    public override void _Process(double delta)
    {
        if (Position.Y <= -400.0f)
            QueueFree();
    }

    public virtual void Initialize(Vector3 spawn, Vector3 target)
    {
        Position = spawn;
        Vector3 difference = target - spawn;
        difference.Z = 0.0f;
        difference *= 0.5f;
        difference.Y += 3.0f;

        ApplyForce(difference * (EntryForceFactor + GD.Randf() * EntryForceConstant));
        Rotation = difference.Normalized();
    }
}

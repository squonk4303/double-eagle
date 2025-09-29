using Godot;
using System;

public partial class Ball : RigidBody3D
{
    private const float COLLISION_FORCE = 1200.0f;
    private const float ENTRY_FORCE = 60.0f;

    private AnimationPlayer _animation;
    private int _timesHit = 0;

    public override void _Ready()
    {
        _animation = GetNode<AnimationPlayer>("AnimationPlayer");
        _animation.SpeedScale = 4;
    }

    /// Prepare for getting spawned in
    public void Initialize(Vector3 spawn, Vector3 target)
    {
        Vector3 difference = target - spawn;
        // TODO: All this does is get a higher angle. There should be a way
        // to get the same result in a more controlled way.
        difference.Z = 0.0f;
        difference *= 0.5f;
        difference.Y += 3.0f;
        Position = spawn;
        ApplyForce(difference * (ENTRY_FORCE + GD.Randf() * 40.0f));
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
    }

    public void LaserHit()
    {
        if (_timesHit <= 0)
        {
            _animation.Play("turn_red");
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

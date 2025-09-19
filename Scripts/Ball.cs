using Godot;
using System;

public partial class Ball : RigidBody3D
{
    private const float COLLISION_FORCE = 1200.0f;
    private const float ENTRY_FORCE = 60.0f;

    /// Prepare for getting spawned in
    public void Initialize(Vector3 spawn, Vector3 target)
    {
        GD.Print(DateTime.Now.Second);
        Position = spawn;
        Vector3 difference = target - spawn;
        difference.Z = 0.0f;
        difference *= 0.5f;
        difference.Y += 3.0f;
        ApplyForce(difference * (ENTRY_FORCE + GD.Randf() * 40.0f));
        GD.Print(difference);
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

    /// What to do when leaving the player's FOV
    public void _on_visible_on_screen_notifier_3d_screen_exited()
    {
        // TODO: Will despawn anytime player looks away, which is unintended
        QueueFree();
    }
}

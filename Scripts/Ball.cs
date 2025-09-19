using Godot;
using System;

public partial class Ball : RigidBody3D
{
    public void SetUp(Vector3 startPosition)
    {
        this.Position = startPosition;
        ApplyForce(new Vector3(1.0f, 1.0f, 0.0f) * 350);
    }

    public void bulletHit(Vector3 b_pos)
    {
        // Get direction from center of bullet to center of self
        Vector3 direction = b_pos.DirectionTo(GlobalPosition);
        // Suppress force applied in z-direction
        // NOTE: This denormalizes the vector
        direction.Z = 0.0f;
        ApplyForce(direction * 1200);
    }

    public void _on_visible_on_screen_notifier_3d_screen_exited()
    {
        // TODO: Will despawn anytime player looks away, which is unintended
        QueueFree();
    }
}

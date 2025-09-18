using Godot;
using System;

public partial class Ball : RigidBody3D
{
    public void SetUp(Vector3 startPosition)
    {
        this.Position = startPosition;
        ApplyForce(new Vector3(1, 1, 0) * 350);
    }

    public void bullet_hit(Vector3 b_pos)
    {
        // Get direction from center of bullet to center of self
        Vector3 direction = b_pos.DirectionTo(GlobalPosition);
        // Suppress force applied in z-direction
        // NOTE: This denormalizes the vector
        direction.Z = 0.0F;
        ApplyForce(direction * 1200);
    }

    public void _on_visible_on_screen_notifier_3d_screen_exited()
    {
        // TODO: Will despawn anytime player looks away, which is unintended
        GD.Print("freeing ", this);
        QueueFree();
    }
}

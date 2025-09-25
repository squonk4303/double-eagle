using Godot;
using System;

public partial class Laser : Node3D
{
    public override void _Ready()
    {
        // Play fade-out animation
        var animation = GetNode<AnimationPlayer>("AnimationPlayer");
        animation.Play("fade");

        // TODO: Will have to see if there's anything that needs doing to
        // free this object like the bullet.
    }

    public override void _PhysicsProcess(double delta)
    {
        // Check for raycast collision
        var ray = GetNode<RayCast3D>("RayCast3D");
        ray.CollideWithAreas = true;
        ray.Enabled = true;

        if (ray.IsColliding())
        {
            var collider = ray.GetCollider();
            GD.Print("+++ Did collide");
            GD.Print("Collided with ", collider);
            ray.GetCollisionNormal();
        }
        else
        {
            // GD.Print("... Did not collide");
        }
    }
}

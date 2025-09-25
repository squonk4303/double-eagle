using Godot;
using System;

public partial class Laser : Node3D
{
    private AnimationPlayer _animation;

    public override void _Ready()
    {
        _animation = GetNode<AnimationPlayer>("AnimationPlayer");

        // Connect signal to free object when animation finishes
        AnimationMixer.AnimationFinishedEventHandler AnimationFinishedAction;
        AnimationFinishedAction = (StringName animName) => OnAnimationFinished(animName);
        _animation.AnimationFinished += AnimationFinishedAction;

        // Play fade-out animation
        _animation.Play("fade-out");
        _animation.SpeedScale = 4;
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
            GD.Print("... Did not collide", DateTime.Now);
        }
    }

    private void OnAnimationFinished(String animName)
    {
        GD.Print("Kill " + animName);
        QueueFree();
    }
}

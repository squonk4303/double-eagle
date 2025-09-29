using Godot;
using System;

public partial class Laser : Node3D
{
    private const int MAX_COLLISIONS = 2;

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
        CheckPenetrations();
    }

    /// Check for ray collisions, allowing for multiple, through penetration
    private void CheckPenetrations()
    {
        // Retrieve the Raycast object and do some alterations
        var ray = GetNode<RayCast3D>("RayCast3D");
        ray.CollideWithAreas = true;
        ray.Enabled = true;

        // Track objects the laser will ignore due to it penetrating through them.
        var exceptions = new Godot.Collections.Array<CollisionObject3D>();

        // Do a new raycast collision check after each penetration left
        Boolean keepChecking = true;
        int collisionsLeft = MAX_COLLISIONS;

        while (collisionsLeft > 0 && keepChecking)
        {
            // Ignore collision with objects through which have already been penetrated
            foreach (var e in exceptions)
            {
                ray.AddException(e);
            }

            // Update raycast before end of physics frame.
            // This ensures the multiple collisions are enacted
            // without the need for multiple active frames.
            ray.ForceRaycastUpdate();

            if (ray.IsColliding())
            {
                var collider = ray.GetCollider() as CollisionObject3D;
                // Prompt collision subject for their reaction
                if (collider.HasMethod("LaserHit"))
                {
                    collider.Call("LaserHit");
                }
                // Ignore collision subject in subsequent collision checks
                exceptions.Add(collider);
            }
            else
            {
                // Stop checking for collisions after hitting a wall or null
                keepChecking = false;
            }

            collisionsLeft -= 1;
        }

        // Remove Raycast from scene so as to only check collisions for the first frame
        ray.QueueFree();
    }

    private void OnAnimationFinished(String animName)
    {
        QueueFree();
    }
}

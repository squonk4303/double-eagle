using Godot;
using System;

public partial class Laser : Node3D
{
    private const int MAX_PENETRATIONS = 4;

    private AnimationPlayer _animation;
    private Boolean _doneRaycast = false;

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
        // Check raycast collision only on first frame

        // Check for raycast collision
        var ray = GetNode<RayCast3D>("RayCast3D");
        ray.CollideWithAreas = true;
        ray.Enabled = true;

        // Objects the laser will ignore due to it penetrating through them.
        var exceptions = new Godot.Collections.Array<CollisionObject3D>();

        Boolean keepChecking = true;
        int penetrationsLeft = MAX_PENETRATIONS;

        while (penetrationsLeft >= 0 && keepChecking)
        {
            // Don't check again for objects which have been penetrated through
            foreach (var e in exceptions)
            {
                ray.AddException(e);
            }

            ray.ForceRaycastUpdate();

            GD.Print("Loopin " + penetrationsLeft);
            if (ray.IsColliding())
            {
                var collider = ray.GetCollider() as CollisionObject3D;
                GD.Print("Collided with ", collider);
                if (collider.HasMethod("LaserHit"))
                {
                    collider.Call("LaserHit");
                }
                exceptions.Add(collider);
                // Add collider to list of exceptions
                // And add list of exceptions to ray exceptions
            }
            else
            {
                // Stop checking for hits after hitting a wall or null
                keepChecking = false;
            }

            penetrationsLeft -= 1;
        }
        // ---------------------------------------------------------------------------

        // Remove Raycast from scene so as to only check collisions for the first frame
        ray.QueueFree();
    }

    private void OnAnimationFinished(String animName)
    {
        QueueFree();
    }
}

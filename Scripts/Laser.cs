using Godot;
using System;

public partial class Laser : Node3D
{
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
        if (!_doneRaycast)
        {
            // ---------------------------------------------------------------------------
            // Check for raycast collision
            var ray = GetNode<RayCast3D>("RayCast3D");
            ray.ForceRaycastUpdate();
            ray.CollideWithAreas = true;
            ray.Enabled = true;

            Boolean going = true;
            int counter = 0;

            while (counter <= 5 && going)
            {
                GD.Print("Loopin " + counter);
                if (ray.IsColliding())
                {
                    var collider = ray.GetCollider() as CollisionObject3D;
                    GD.Print("Collided with ", collider);
                    if (collider.HasMethod("LaserHit"))
                    {
                        collider.Call("LaserHit");
                    }
                    ray.AddException(collider);
                    // Add collider to list of exceptions
                    // And add list of exceptions to ray exceptions
                }
                else
                {
                    going = false;
                }

                counter += 1;
            }
            // ---------------------------------------------------------------------------
            _doneRaycast = true;
        }
    }

    private void OnAnimationFinished(String animName)
    {
        QueueFree();
    }
}

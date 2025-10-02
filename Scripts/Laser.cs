using Godot;
using System;
using System.Collections.Generic;  // For List

public partial class Laser : Node3D
{
    private const int MAX_COLLISIONS = 2;

    private AnimationPlayer _animation;
    private AudioStreamPlayer3D _audioStream;
    private RayCast3D _ray;

    // Conditions to be fulfilled before freeing the Laser object.
    private List<string> _freeConditions = new List<string>{"animation", "sfx"};

    public override void _Ready()
    {
        // Retrieve the Raycast object and do some alterations
        _ray = GetNode<RayCast3D>("RayCast3D");
        _ray.CollideWithAreas = true;
        _ray.Enabled = true;

        _animation = GetNode<AnimationPlayer>("AnimationPlayer");
        _audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");

        // Connect signal to free object when animation finishes
        AnimationMixer.AnimationFinishedEventHandler AnimationFinishedAction;
        AnimationFinishedAction = (StringName animName) => OnAnimationFinished(animName);
        _animation.AnimationFinished += AnimationFinishedAction;

        // And for audio
        _audioPlayer.Finished += OnAudioFinished;

        // Play fade-out animation and sound effect
        _animation.Play("fade-out");
        _animation.SpeedScale = 4;
        _audioPlayer.Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Check raycast collisions for first frame
        if (_ray.Enabled)
        {
            CheckPenetrations(_ray);
        }
    }

    /// Check for _ray collisions, allowing for multiple, through penetration
    private void CheckPenetrations(RayCast3D _ray)
    {
        // Track objects the laser will ignore due to it penetrating through them.
        var exceptions = new Godot.Collections.Array<CollisionObject3D>();

        // Do a new raycast collision check after each penetration left
        int collisionsLeft = MAX_COLLISIONS;

        while (collisionsLeft > 0)
        {
            // Ignore collision with objects through which have already been penetrated
            foreach (var e in exceptions)
            {
                _ray.AddException(e);
            }

            // Update raycast before end of physics frame.
            // This ensures the multiple collisions are enacted
            // without the need for multiple active frames.
            _ray.ForceRaycastUpdate();

            if (_ray.IsColliding())
            {
                var collider = _ray.GetCollider() as CollisionObject3D;
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
                // Stop checking for collisions after hitting a wall or hitting null
                collisionsLeft = 0;
            }

            collisionsLeft -= 1;
        }

        // Disable Raycast so as to only check collisions for the first frame
        _ray.Enabled = false;
    }

    private void OnAnimationFinished(String animName)
    {
        _freeConditions.Remove("animation");
        // If list is empty, free laser.
        if(_freeConditions.Count == 0)
        {
            QueueFree();
        }
    }

    private void OnAudioFinished()
    {
        _freeConditions.Remove("sfx");
        // If list is empty, free laser.
        if(_freeConditions.Count == 0)
        {
            QueueFree();
        }
    }
}

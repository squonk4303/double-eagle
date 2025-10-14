using Godot;
using System;

public partial class Bullet : Node3D
{
    private const float BULLET_SPEED = 140.0f;
    private const float KILL_TIMER = 4.0f;

    private bool _hitSomethingYet = false;

    public override void _Ready()
    {
        // Call method Collided when something enters collision space
        Area3D area = GetNode<Area3D>("Area3D");
        area.BodyEntered += Collided;

        // Die
        Perish();
    }

    /// Get spawned in at specified position and angle
    public void Initialize(Vector3 startPosition, Vector3 startRotation)
    {
        Position = startPosition;
        Position += new Vector3(0, -0.55f, 0);
        Rotation = startRotation;
    }

    /// Start a timer to remove bullet after a period of time
    private async void Perish()
    {
        // Start timer after which bullet despawns
        await ToSignal(
            GetTree().CreateTimer(KILL_TIMER), SceneTreeTimer.SignalName.Timeout
        );
        QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        // https://docs.godotengine.org/en/stable/classes/class_node3d.html#class-node3d-property-global-transform
        // Vector3 forwardDir = GlobalPosition.normalized();
        // var forward_dir = global_transform.basis.z.normalized()
        // TODO: Temporary hard-coded direction
        Vector3 forwardDir = new Vector3(0.0f, 0.0f, -1.0f);
        forwardDir = new Vector3(0.0f, 0.0f, -1.0f);
        Translate(forwardDir * BULLET_SPEED * (float)delta);
    }

    /// Prompt others to respond when colliding into them
    private void Collided(Node3D body)
    {
        if (!_hitSomethingYet && body.HasMethod("bulletHit"))
        {
            body.Call("bulletHit", GlobalPosition);
        }

        _hitSomethingYet = true;
        QueueFree();
    }
}

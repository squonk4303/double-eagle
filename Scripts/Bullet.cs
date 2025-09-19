using Godot;
using System;

public partial class Bullet : Node3D
{
    const float BULLET_SPEED = 140.0f;
    const float KILL_TIMER = 4.0f;

    bool hitSomethingYet = false;

    public override void _Ready()
    {
        Area3D area = GetNode<Area3D>("Area3D");
        area.BodyEntered += Collided;
        GD.Print("Something");

        Perish();
    }

    public void Initialize(Vector3 startPosition, Vector3 startRotation)
    {
        Position = startPosition;
        Rotation = startRotation;
    }

    /// Start a timer to remove bullet after a period of time
    private async void Perish()
    {
        // Start timer after which bullet despawns
        await ToSignal(GetTree().CreateTimer(KILL_TIMER), SceneTreeTimer.SignalName.Timeout);
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

    private void Collided(Node body)
    {
        GD.Print("entered something");
        // if !hit_something_yet and body.has_method("bullet_hit"):
        //         body.bullet_hit(position)

        // hit_something_yet = true
        // queue_free()
    }
}

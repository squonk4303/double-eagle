using Godot;
using System;

public partial class Balloon : Ball
{
    public override void _Ready()
    {
        // Mass = 0.2f;
        GravityScale = 0.5f;
        base._Ready();
        PitchConstant = 1.2f;
        PitchFactor = 0.3f;
    }

    public override void Initialize(Vector3 spawn, Vector3 target)
    {
        spawn.Z = 2.0f;
        base.Initialize(spawn, target);
    }
}

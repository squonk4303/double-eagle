using Godot;
using System;

public partial class SmallBall : Ball
{
    public override void _Ready()
    {
        base._Ready();
        PitchConstant = 1.8f;
        PitchFactor = 0.2f;
    }

    public override void Initialize(Vector3 spawn, Vector3 target)
    {
        spawn.Z = 3.0f;
        base.Initialize(spawn, target);
    }
}

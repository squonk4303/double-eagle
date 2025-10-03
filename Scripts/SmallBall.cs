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
}

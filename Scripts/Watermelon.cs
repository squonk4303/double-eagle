using Godot;
using System;

public partial class Watermelon : Ball
{
    public override void _Ready()
    {
        AnimationSpeedScale = 1.5f;
        base._Ready();
        PitchConstant = 0.8f;
        PitchFactor = 0.1f;
    }
}

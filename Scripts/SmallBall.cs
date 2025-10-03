using Godot;
using System;

public partial class SmallBall : Ball
{
    public override void _Ready()
    {
        base._Ready();
        GD.Print("I'm a smallball ", this);
    }
}

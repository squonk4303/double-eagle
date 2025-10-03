using Godot;
using System;

public partial class Watermelon : Ball
{
    public override void _Ready()
    {
        base._Ready();
        GD.Print("I'm a watermelon ", this);
    }
}

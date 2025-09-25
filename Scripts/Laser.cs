using Godot;
using System;

public partial class Laser : Node3D
{
    public override void _Ready()
    {
        var animation = GetNode<AnimationPlayer>("AnimationPlayer");
        animation.Play("fade");
    }
}

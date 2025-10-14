using Godot;
using System;

public partial class Fallout : Area3D
{
    [Signal]
    public delegate void BodyFellOutEventHandler(Node3D body);

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public void OnBodyEntered(Node3D body)
    {
        EmitSignal(SignalName.BodyFellOut, body);
    }
}

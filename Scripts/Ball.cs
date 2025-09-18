using Godot;
using System;

public partial class Ball : RigidBody3D
{
    public void SetUp(Vector3 startPosition)
    {
        this.Position = startPosition;
        ApplyForce(new Vector3(1, 1, 0) * 350);
    }
}

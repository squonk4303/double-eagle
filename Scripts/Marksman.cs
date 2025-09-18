using Godot;
using System;

public partial class Marksman : Node3D
 {
    private Node3D pivot;
    private Camera3D camera;

    float mouseSensitivity = 0.02f;

    const double MAX_PITCH = Mathf.Pi * 0.5f;
    const double MIN_PITCH = Mathf.Pi * 0.5f;

    Vector3 toRotate;

    // signal gun_0_fired(b_position: Vector3, b_rotation: Vector3)

    public override void _Ready()
    {
        pivot = GetNode<Node3D>("Pivot");
        camera = GetNode<Camera3D>("Pivot/Camera3D");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (
            @event is InputEventMouseMotion mouseMotion &&
            Input.MouseMode == Input.MouseModeEnum.Captured
        ) {
            // Set distances to rotate camera
            // Continued in _Process(...)
            Vector2 mouseStretch = -1.0f * mouseMotion.Relative * mouseSensitivity;
            toRotate.X = mouseStretch.X;
            toRotate.Y = mouseStretch.Y;
        }
    }

    public override void _Process(double delta)
    {
        // TODO: *SHOULD* mouse movement be tied to process delta?
        // Find this out.

        // If mouse is captured, move to center of screen each frame

        // Rotate Pivot along y-axis
        // And Camera along its x-axis
        // TODO: Is order significant?
        pivot.Rotate(Vector3.Up, toRotate.Y * (float)delta);
        camera.Rotate(Vector3.Right, toRotate.X * (float)delta);
        // camera.rotation.x = clamp(camera.rotation.x, MIN_PITCH, MAX_PITCH)

        // TODO: Make this not suck
        Vector3 camRot = camera.Rotation;
        camRot.X = (float)Mathf.Clamp(camera.Rotation.X, MIN_PITCH, MAX_PITCH);
        camera.Rotation = camRot;

        // Reset rotation vector
        toRotate = new Vector3(0, 0, 0);
    }
}

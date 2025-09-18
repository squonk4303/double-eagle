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

    [Signal]
    public delegate void FireGun00EventHandler(Vector3 position, Vector3 rotation);
    // signal gun_0_fired(b_position: Vector3, b_rotation: Vector3)

    public override void _Ready()
    {
        // Capture mouse once player instantiates
        Input.MouseMode = Input.MouseModeEnum.Captured;

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
            // TODO: Evaluate Relative vs. ScreenRelative
            Vector2 mouseStretch = -1.0f * mouseMotion.Relative * mouseSensitivity;
            toRotate.X = mouseStretch.X;
            toRotate.Y = mouseStretch.Y;
        }
        if (@event.IsActionPressed("primary_fire"))
        {
            // Tweak position before emitting
            Vector3 offset = new Vector3(1.0f, -1.0f, -1.0f) * 0.1f;
            Vector3 bulletPosition = camera.GlobalPosition + offset;

            // Emit signal to spawn a bullet in parent scene
            // Gun00Fired.emit(bulletPosition, camera.GlobalRotation);
            EmitSignal(SignalName.FireGun00, bulletPosition, GlobalRotation);
        }

        // Escape mouse capture with Esc key
        if (
            @event is InputEventKey keyEvent &&
            keyEvent.Pressed &&
            keyEvent.Keycode == Key.Escape
        ) {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            // Capture mouse on left-click
            if (
                mouseButtonEvent.ButtonIndex == MouseButton.Left &&
                mouseButtonEvent.Pressed
            ) {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
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
        pivot.Rotate(Vector3.Up, toRotate.X * (float)delta);
        camera.Rotate(Vector3.Right, toRotate.Y * (float)delta);
        // camera.rotation.x = clamp(camera.rotation.x, MIN_PITCH, MAX_PITCH)

        // TODO: Make this not suck
        // Vector3 camRot = camera.Rotation;
        // camRot.X = (float)Mathf.Clamp(camera.Rotation.X, MIN_PITCH, MAX_PITCH);
        // camera.Rotation = camRot;

        // Reset rotation vector
        toRotate = new Vector3(0, 0, 0);
    }
}

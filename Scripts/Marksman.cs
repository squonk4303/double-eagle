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
            Vector3 offset = new Vector3(1, -1, -1) * 0.01f;
            Vector3 bulletPosition = camera.GlobalPosition + offset;

            // Emit signal to spawn a bullet in parent scene
            // Gun00Fired.emit(bulletPosition, camera.GlobalRotation);
            EmitSignal(SignalName.FireGun00, bulletPosition, GlobalRotation);
        }

        // if event.is_action_pressed("primary_fire"):
        //     # Tweak position before emitting
        //     var offset = Vector3(1, -1, -1) * 0.01
        //     var bullet_position = camera.global_position + offset
        //     # Emit signal to spawn a bullet in parent scene
        //     gun_0_fired.emit(bullet_position, camera.global_rotation)

        // # Hard-coded input events
        // if event is InputEventKey:
        //     # Yield control of mouse when "esc" is pressed
        //     if event.pressed and event.keycode == KEY_ESCAPE:
        //         Input.mouse_mode = Input.MOUSE_MODE_VISIBLE

        // if event is InputEventMouseButton:
        //     # Recapture mouse upon clicking inside window
        //     if event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
        //         Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
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
        // Vector3 camRot = camera.Rotation;
        // camRot.X = (float)Mathf.Clamp(camera.Rotation.X, MIN_PITCH, MAX_PITCH);
        // camera.Rotation = camRot;

        // Reset rotation vector
        toRotate = new Vector3(0, 0, 0);
    }
}

using Godot;
using System;

public partial class Marksman : Node3D
{
    // Where to limit pitch
    private const double MAX_PITCH = Mathf.Pi * 0.5f;
    private const double MIN_PITCH = Mathf.Pi * 0.5f;
    private const string GUN_FIRE_SFX = "res://Audio/gun_fire.wav";

    // Get child nodes for revolutionary actions (Completed in _Ready)
    private Node3D _pivot;
    private Camera3D _camera;
    private AudioStreamPlayer3D _audioStream;

    private Vector3 ToRotate;
    private float MouseSensitivity = 0.02f;

    // Declare signal for firing weapon
    [Signal]
    public delegate void GunFire00EventHandler(Vector3 position, Vector3 rotation);

    [Signal]
    public delegate void GunFireRayEventHandler(Vector3 position, Vector3 rotation);

    public override void _Ready()
    {
        _pivot = GetNode<Node3D>("Pivot");
        _camera = GetNode<Camera3D>("Pivot/Camera3D");
        _audioStream = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
    }

    /// Handle marksman-related input
    public override void _UnhandledInput(InputEvent @event)
    {
        if (
            @event is InputEventMouseMotion mouseMotion &&
            Input.MouseMode == Input.MouseModeEnum.Captured
        ) {
            // Set distances to rotate camera
            // Continued in _Process(...)
            // TODO: Evaluate Relative vs. ScreenRelative
            Vector2 mouseStretch = -1.0f * mouseMotion.Relative * MouseSensitivity;
            ToRotate.X = mouseStretch.X;
            ToRotate.Y = mouseStretch.Y;
        }

        if (@event.IsActionPressed("primary_fire"))
        {
            // Tweak position before emitting
            Vector3 offset = new Vector3(1.0f, -1.0f, -1.0f) * 0.1f;
            Vector3 bulletPosition = _camera.GlobalPosition + offset;

            // Instantiate and play sfx
            _audioStream.Stream = GD.Load<AudioStream>(GUN_FIRE_SFX);
            _audioStream.Play();

            // Emit signal to spawn a bullet in parent scene
            // Gun00Fired.emit(bulletPosition, _camera.GlobalRotation);
            EmitSignal(SignalName.GunFire00, bulletPosition, _camera.GlobalRotation);
        }

        if (@event.IsActionPressed("secondary_fire"))
        {
            Vector3 offset = new Vector3(1.0f, -1.0f, -1.0f) * 0.1f;
            Vector3 bulletPosition = _camera.GlobalPosition + offset;
            EmitSignal(SignalName.GunFireRay, bulletPosition, _camera.GlobalRotation);
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
        _pivot.Rotate(Vector3.Up, ToRotate.X * (float)delta);
        _camera.Rotate(Vector3.Right, ToRotate.Y * (float)delta);
        // _camera.rotation.x = clamp(_camera.rotation.x, MIN_PITCH, MAX_PITCH)

        // TODO: Make this not suck
        // Vector3 camRot = _camera.Rotation;
        // camRot.X = (float)Mathf.Clamp(_camera.Rotation.X, MIN_PITCH, MAX_PITCH);
        // _camera.Rotation = camRot;

        // Reset rotation vector
        ToRotate = new Vector3(0, 0, 0);
    }
}

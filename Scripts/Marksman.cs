using Godot;
using System;

public partial class Marksman : CharacterBody3D
{
    private const string GUNFIRE_SFX = "res://Audio/gun_fire.wav";

    // Where to limit pitch
    private const double MAX_PITCH = Mathf.Pi * 0.5f;
    private const double MIN_PITCH = Mathf.Pi * 0.5f;

    // Get child nodes for revolutionary actions (Completed in _Ready)
    private Node3D _pivot;
    private Camera3D _camera;
    private Vector3 _feetPosition;

    private Vector3 _toRotate;
    private AudioStream _gunfireSfx;
    private AudioStreamPlayer3D _audioPlayer;

    private PauseMenu _pauseMenu;

    private float _mouseSensitivity = 0.5f;
    // Sensitivity factor to tweak overall sensitivity
    private float _sensitivityFactor = 0.1f;

    // Comment out this since the sensitivity is loaded from config
    //[Export] public float MouseSensitivity = 0.02f;
    [Export] public float LeanSpeed = 6.0f;
    [Export] public float LeanLength = 8.0f;
    [Export] public float NoclipSpeed = 5.0f;
    [Export] public bool NoclipMode = false;

    // Declare signal for firing weapon
    [Signal]
    public delegate void GunFire00EventHandler(Vector3 position, Vector3 rotation);

    [Signal]
    public delegate void GunFireRayEventHandler(Vector3 position, Vector3 rotation);

    public override void _EnterTree()
    {
        base._EnterTree();
        _gunfireSfx = GD.Load<AudioStream>(GUNFIRE_SFX);
    }

    public override void _Ready()
    {
        // Set characterbody physics to disregard floors
        MotionMode = MotionModeEnum.Floating;

        // Set initial position
        _feetPosition = GlobalPosition;

        // Retrieve child nodes
        _pivot = GetNode<Node3D>("Pivot");
        _camera = GetNode<Camera3D>("Pivot/Camera3D");
        _audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
        _pauseMenu = GetNode<PauseMenu>("PauseMenu");

        // Load sensitivity from config
        var config = new ConfigFile();
        if (config.Load("user://settings.cfg") == Error.Ok)
            _mouseSensitivity = (float)config.GetValue("controls", "sensitivity", 0.5f);
    }

    // Called when sensitivity is changed in options menu
    public void OnSensitivityChanged(float value)
    {
        _mouseSensitivity = value;
    }

    /// Handle marksman-related input callbacks
    public override void _UnhandledInput(InputEvent @event)
    {
        // Check for mouse movement
        if (
            @event is InputEventMouseMotion mouseMotion &&
            Input.MouseMode == Input.MouseModeEnum.Captured
        )
        {
            // Set distances to rotate camera
            // Continued in _Process(...)
            // TODO: Evaluate Relative vs. ScreenRelative
            Vector2 mouseStretch = -1.0f * mouseMotion.Relative * _mouseSensitivity * _sensitivityFactor;
            _toRotate.X = mouseStretch.X;
            _toRotate.Y = mouseStretch.Y;
        }

        // Check for mouse buttons
        if (@event.IsActionPressed("primary_fire"))
        {
            // Tweak position before emitting
            Vector3 offset = new Vector3(1.0f, -1.0f, -1.0f) * 0.1f;
            Vector3 bulletPosition = _camera.GlobalPosition + offset;

            // Instantiate and play sfx
            _audioPlayer.Stream = _gunfireSfx;
            _audioPlayer.Play();

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

        // Check for keyboard events
        if (@event is InputEventKey keyEvent)
        {
            // --- Escape mouse capture with Esc key ---
            if (keyEvent.Keycode == Key.Escape)
            {
                _pauseMenu.TogglePause();
            }
        }

        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            // Capture mouse on left-click
            if (
                mouseButtonEvent.ButtonIndex == MouseButton.Left &&
                mouseButtonEvent.Pressed
            )
            {
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
        _pivot.Rotate(Vector3.Up, _toRotate.X * (float)delta);
        _camera.Rotate(Vector3.Right, _toRotate.Y * (float)delta);
        // _camera.rotation.x = clamp(_camera.rotation.x, MIN_PITCH, MAX_PITCH)

        // TODO: Make this not suck
        // Vector3 camRot = _camera.Rotation;
        // camRot.X = (float)Mathf.Clamp(_camera.Rotation.X, MIN_PITCH, MAX_PITCH);
        // _camera.Rotation = camRot;

        // Reset rotation vector
        _toRotate = new Vector3(0, 0, 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        // --- Leaning Movement ---

        if (!NoclipMode)
        {
            Vector3 chase;
            Vector3 run;
            Vector3 lean = Vector3.Zero;

            if (Input.IsActionPressed("move_left"))
            {
                lean -= _camera.GlobalTransform.Basis.X;
            }
            if (Input.IsActionPressed("move_right"))
            {
                lean += _camera.GlobalTransform.Basis.X;
            }
            if (Input.IsActionPressed("move_back"))
            {
                lean -= _camera.GlobalTransform.Basis.Y;
            }
            if (Input.IsActionPressed("move_forward"))
            {
                lean += _camera.GlobalTransform.Basis.Y;
            }

            if (lean != Vector3.Zero)
            {
                lean = lean.Normalized();
            }

            // Modify head position with the specifications from input
            // NOTE that lean is Vector3.Zero when user gives no input
            chase = _feetPosition + lean * LeanLength;
            run = chase - GlobalPosition;
            MoveAndCollide(run * LeanSpeed * (float)delta);
        }


        // --- Noclip Movement ---

        // Initialize direction vector
        Vector3 direction = Vector3.Zero;

        if (NoclipMode)
        {
            // Move in the direction you are facing
            // by getting the camera's directional vectors
            if (Input.IsActionPressed("move_forward"))
                direction -= _camera.GlobalTransform.Basis.Z;
            if (Input.IsActionPressed("move_back"))
                direction += _camera.GlobalTransform.Basis.Z;
            if (Input.IsActionPressed("move_left"))
                direction -= _camera.GlobalTransform.Basis.X;
            if (Input.IsActionPressed("move_right"))
                direction += _camera.GlobalTransform.Basis.X;
            if (Input.IsActionPressed("move_up"))
                direction += _camera.GlobalTransform.Basis.Y;
            if (Input.IsActionPressed("move_down"))
                direction -= _camera.GlobalTransform.Basis.Y;
        }

        // If there is any movement, normalize direction and move marksman
        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            // Update marksman global position
            GlobalPosition += direction * NoclipSpeed * (float)delta;
        }
    }
}

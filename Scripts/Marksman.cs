using Godot;
using System;

public partial class Marksman : CharacterBody3D
{
    private const string GUNFIRE_SFX = "res://Audio/gun_fire.wav";

    // Where to limit camera rotations
    private const float TO_RADIANS = Mathf.Pi * 0.005555555555555556f;
    private const float MAX_PITCH = 30.0f * TO_RADIANS;
    private const float MIN_PITCH = -30.0f * TO_RADIANS;
    private const float MAX_YAW = 45.0f * TO_RADIANS;
    private const float MIN_YAW = -45.0f * TO_RADIANS;

    private Camera3D _camera;
    private Vector3 _feetPosition;

    private Vector2 _accumulatedRotation;
    private AudioStream _gunfireSfx;
    private AudioStreamPlayer3D _audioPlayer;

    [Export] public float MouseSensitivity = 0.00045f;
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
        _camera = GetNode<Camera3D>("Camera3D");
        _audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
    }

    /// Handle marksman-related input callbacks
    public override void _UnhandledInput(InputEvent @event)
    {
        // --- Handle mouse movement ---
        if (
            @event is InputEventMouseMotion mouseMotion &&
            Input.MouseMode == Input.MouseModeEnum.Captured
        )
        {
            // Accumulate mouse travel intro a Vector2
            // TODO: Evaluate Relative vs. ScreenRelative
            _accumulatedRotation += -1.0f * mouseMotion.Relative * MouseSensitivity;

            // Reset rotation
            Transform3D transform = Transform;
            transform.Basis = Basis.Identity;
            Transform = transform;

            // Clamp rotations
            _accumulatedRotation.X = Mathf.Clamp(_accumulatedRotation.X, MIN_YAW, MAX_YAW);
            _accumulatedRotation.Y = Mathf.Clamp(_accumulatedRotation.Y, MIN_PITCH, MAX_PITCH);

            RotateObjectLocal(Vector3.Up, _accumulatedRotation.X);
            RotateObjectLocal(Vector3.Right, _accumulatedRotation.Y);
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
                Input.MouseMode = Input.MouseModeEnum.Visible;
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

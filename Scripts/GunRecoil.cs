using Godot;
using System;

public partial class GunRecoil : Node3D
{
    // Recoil configuration
    [ExportGroup("Recoil Settings")]
    [Export] public Vector3 PositionRecoil { get; set; } = new(0, 0, 0.05f);    // How far gun moves back (Z is backward)
    [Export] public Vector3 RotationRecoil { get; set; } = new(-10, 0, 0);      // Rotation in degrees (X is tilt up)
    [Export] public Vector3 PositionVariance { get; set; } = new(0.01f, 0.01f, 0.01f);  // Random offset per shot
    [Export] public Vector3 RotationVariance { get; set; } = new(2, 3, 1);              // Random rotation per shot

    [ExportGroup("Recovery Settings")]
    [Export] public float RecoverySpeed { get; set; } = 10.0f;          // How fast gun returns to default
    [Export] public float RotationRecoverySpeed { get; set; } = 12.0f;  // Separate recovery for rotation

    [ExportGroup("Accumulation Settings")]
    [Export] public bool EnableAccumulation { get; set; } = true;   // Whether recoil stacks
    [Export] public float MaxAccumulation { get; set; } = 3.0f;     // Max multiplier for accumulated recoil
    [Export] public float AccumulationDecay { get; set; } = 2.0f;   // How fast accumulation decreases

    // Internal state
    private Vector3 _defaultPosition;
    private Vector3 _defaultRotation;
    private Vector3 _currentPositionOffset;
    private Vector3 _currentRotationOffset;
    private float _accumulationLevel = 0.0f;  // Current accumulation (0 to MaxAccumulation)

    public override void _Ready()
    {
        _defaultPosition = Position;
        _defaultRotation = RotationDegrees;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        // Smoothly recover position
        _currentPositionOffset = _currentPositionOffset.Lerp(Vector3.Zero, RecoverySpeed * dt);
        Position = _defaultPosition + _currentPositionOffset;

        // Smoothly recover rotation
        _currentRotationOffset = _currentRotationOffset.Lerp(Vector3.Zero, RotationRecoverySpeed * dt);
        RotationDegrees = _defaultRotation + _currentRotationOffset;

        // Decay accumulation when not shooting
        if (EnableAccumulation)
        {
            _accumulationLevel = Mathf.Max(0, _accumulationLevel - AccumulationDecay * dt);
        }
    }

    public void ApplyRecoil()
    {
        // Calculate accumulation multiplier
        float accumMult = 1.0f;
        if (EnableAccumulation)
        {
            accumMult = 1.0f + _accumulationLevel;
            _accumulationLevel = Mathf.Min(MaxAccumulation, _accumulationLevel + 0.3f);
        }

        // Add random variance
        Vector3 posVariance = new(
            (float)GD.RandRange(-PositionVariance.X, PositionVariance.X),
            (float)GD.RandRange(-PositionVariance.Y, PositionVariance.Y),
            (float)GD.RandRange(-PositionVariance.Z, PositionVariance.Z)
        );

        Vector3 rotVariance = new(
            (float)GD.RandRange(-RotationVariance.X, RotationVariance.X),
            (float)GD.RandRange(-RotationVariance.Y, RotationVariance.Y),
            (float)GD.RandRange(-RotationVariance.Z, RotationVariance.Z)
        );

        // Apply recoil with accumulation
        _currentPositionOffset += (PositionRecoil + posVariance) * accumMult;
        _currentRotationOffset += (RotationRecoil + rotVariance) * accumMult;
    }

    // Optional: Reset accumulation manually
    public void ResetAccumulation()
    {
        _accumulationLevel = 0.0f;
    }
}
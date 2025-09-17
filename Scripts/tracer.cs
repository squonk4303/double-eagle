using Godot;

public partial class Tracer : Node3D
{
	// Uncomment and assign when RayCast3D
	// [Export]
	// private RayCast3D ray;

	public void Initialize(Vector3 startPosition, Vector3 startRotation)
	{
		Position = startPosition;
		Rotation = startRotation;
	}

	public void OnBodyEntered()
	{
		GD.Print("something entered bullet");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Uncomment to free the node when needed
		// QueueFree();
	}
}

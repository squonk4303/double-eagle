extends RigidBody3D


func initialize(start_position: Vector3):
    """To be run when object spawns"""
    position = start_position
    apply_force(Vector3(1, 1, 0) * 200)


func bullet_hit(b_pos):
    # Get direction from center of bullet to center of self
    var direction = b_pos.direction_to(global_position)
    # Suppress force applied in z-direction
    # NOTE: This denormalizes the vector
    direction.z = 0.0
    apply_force(direction * 1200)


func _on_visible_on_screen_notifier_3d_screen_exited() -> void:
    queue_free()

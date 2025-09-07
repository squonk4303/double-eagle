extends RigidBody3D


func initialize(start_position: Vector3):
    """To be run when object spawns"""
    position = start_position
    apply_force(Vector3(1, 1, 0) * 200)

func on_ray_hit():
    print("--- I'M HIT")

func _on_visible_on_screen_notifier_3d_screen_exited() -> void:
    queue_free()

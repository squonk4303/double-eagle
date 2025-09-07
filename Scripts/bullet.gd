extends Node3D

@onready var ray = $RayCast3D


func initialize(start_position, start_rotation):
    position = start_position
    rotation = start_rotation

func _physics_process(delta: float) -> void:
    var space_state = get_world_3d().direct_space_state
    if $RayCast3D.is_colliding():
        print("I collided")
        var collider = ray.get_collider()
        collider.add_force(Vector3(0, 1000, 0))
        collider.on_ray_hit()

    # queue_free()

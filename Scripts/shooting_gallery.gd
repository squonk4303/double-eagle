extends Node3D

@export var ball_scene: PackedScene
@export var bullet_scene: PackedScene


func _on_ball_timer_timeout() -> void:
    var ball = ball_scene.instantiate()
    var z_pos = randi() % 5
    var spawn_location = Vector3(-5.0, 0, z_pos)
    ball.initialize(spawn_location)

    # Spawn by adding the object to the main scene
    add_child(ball)

func _on_marksman_gun_0_fired(b_position: Vector3, b_rotation: Vector3) -> void:
    pass # Replace with function body.

    print("Firing @ ", b_position, b_rotation)
    var bullet = bullet_scene.instantiate()
    bullet.initialize(b_position, b_rotation)
    add_child(bullet)

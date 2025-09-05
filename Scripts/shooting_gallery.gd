extends Node3D

@export var ball_scene: PackedScene


func _on_ball_timer_timeout() -> void:
    var ball = ball_scene.instantiate()
    var z_pos = randi() % 5
    var spawn_location = Vector3(-5.0, 0, z_pos)
    ball.initialize(spawn_location)

    # Spawn by adding the object to the main scene
    add_child(ball)

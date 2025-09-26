extends Node3D

# @onready var ray = $RayCast3D


func initialize(start_position, start_rotation):
    position = start_position
    rotation = start_rotation


func on_body_entered():
    print("something entered bullet")


func _physics_process(delta: float) -> void:
    pass
    # queue_free()

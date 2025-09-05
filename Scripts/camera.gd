extends Node3D

@onready var camera = $Camera3D

@export var bullet_scene: PackedScene
@export var mouse_sens = 0.02

var MAX_PITCH = deg_to_rad(90)
var MIN_PITCH = deg_to_rad(-90)

var to_rotate: Vector3

func _ready():
    # Capture mouse
    Input.mouse_mode = Input.MOUSE_MODE_CAPTURED


func _unhandled_input(event):
    if (
        event is InputEventMouseMotion and
        Input.mouse_mode == Input.MOUSE_MODE_CAPTURED
    ):
        # Set distances to rotate camera
        to_rotate.y = -1 * event.relative.x * mouse_sens
        to_rotate.x = -1 * event.relative.y * mouse_sens

    if event.is_action_pressed("primary_fire"):
        print("Firing @ ", camera.rotation)
        var bullet = bullet_scene.instantiate()
        var bullet_position = camera.position + Vector3(1,1,1)
        bullet.initialize(camera.position, camera.rotation)

        add_child(bullet)

    if event.is_action_pressed("LMB"):
        # Recapture mouse upon clicking inside window
        Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

    if event.is_action_pressed("ESCAPE"):
        # Yield control of mouse when "esc" is pressed
        Input.mouse_mode = Input.MOUSE_MODE_VISIBLE


func _process(delta: float) -> void:
    # TODO: *SHOULD* mouse movement be tied to process delta?
    # Find this out.

    # Rotate Pivot along y-axis
    rotate_y(to_rotate.y * delta)
    # Rotate Camera along its x-axis
    camera.rotate_x(to_rotate.x * delta)
    camera.rotation.x = clamp(camera.rotation.x, MIN_PITCH, MAX_PITCH)
    # Reset rotation vector
    to_rotate = Vector3.ZERO

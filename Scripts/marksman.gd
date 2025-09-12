extends Node3D

@onready var pivot = $Pivot
@onready var camera = $Pivot/Camera3D

@export var mouse_sens = 0.02

var MAX_PITCH = deg_to_rad(90)
var MIN_PITCH = deg_to_rad(-90)

var to_rotate: Vector3

signal gun_0_fired(b_position: Vector3, b_rotation: Vector3)


func _ready():
    # Capture mouse
    Input.mouse_mode = Input.MOUSE_MODE_CAPTURED


func _unhandled_input(event):
    if (
        event is InputEventMouseMotion and
        Input.mouse_mode == Input.MOUSE_MODE_CAPTURED
    ):
        # Set distances to rotate camera
        # Continued in _process(...)
        to_rotate.y = -1 * event.relative.x * mouse_sens
        to_rotate.x = -1 * event.relative.y * mouse_sens

    if event.is_action_pressed("primary_fire"):
        # Tweak position before emitting
        var offset = Vector3(1, -1, -1) * 0.01
        var bullet_position = camera.global_position + offset
        # Emit signal to spawn a bullet in parent scene
        gun_0_fired.emit(bullet_position, camera.global_rotation)

    # Hard-coded input events
    if event is InputEventKey:
        # Yield control of mouse when "esc" is pressed
        if event.pressed and event.keycode == KEY_ESCAPE:
            Input.mouse_mode = Input.MOUSE_MODE_VISIBLE

    if event is InputEventMouseButton:
        # Recapture mouse upon clicking inside window
        if event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
            Input.mouse_mode = Input.MOUSE_MODE_CAPTURED


func _process(delta: float) -> void:
    # TODO: *SHOULD* mouse movement be tied to process delta?
    # Find this out.

    # Rotate Pivot along y-axis
    # And Camera along its x-axis
    pivot.rotate_y(to_rotate.y * delta)
    camera.rotate_x(to_rotate.x * delta)
    camera.rotation.x = clamp(camera.rotation.x, MIN_PITCH, MAX_PITCH)
    # Reset rotation vector
    to_rotate = Vector3.ZERO

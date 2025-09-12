extends Node3D

const BULLET_SPEED = 140

const KILL_TIMER = 4
var timer = 0

var hit_something_yet = false


func _ready():
    $Area3D.body_entered.connect(self.collided)


func initialize(start_position: Vector3, start_rotation: Vector3):
    position = start_position
    rotation = start_rotation


func _physics_process(delta):
    # https://docs.godotengine.org/en/stable/classes/class_node3d.html#class-node3d-property-global-transform
    var forward_dir = global_transform.basis.z.normalized()
    forward_dir = Vector3(0, 0, -1)
    translate(forward_dir * BULLET_SPEED * delta)


func collided(body):
    print("hit something", position)
    if !hit_something_yet:
        if body.has_method("bullet_hit"):
            pass

    hit_something_yet = true
    queue_free()

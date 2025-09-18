extends Node3D

const BULLET_SPEED = 140
const KILL_TIMER = 4.0

var hit_something_yet = false


func _ready():
    $Area3D.body_entered.connect(self.collided)

    # Start timer after which bullet despawns
    await get_tree().create_timer(KILL_TIMER).timeout
    queue_free()


func initialize(start_position: Vector3, start_rotation: Vector3):
    position = start_position
    rotation = start_rotation


func _physics_process(delta):
    # https://docs.godotengine.org/en/stable/classes/class_node3d.html#class-node3d-property-global-transform
    var forward_dir = global_transform.basis.z.normalized()
    forward_dir = Vector3(0, 0, -1)
    translate(forward_dir * BULLET_SPEED * delta)


func collided(body):
    if !hit_something_yet and body.has_method("bullet_hit"):
            body.bullet_hit(position)

    hit_something_yet = true
    queue_free()

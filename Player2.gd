extends KinematicBody

var camera_angle = 0
var mouse_sensitivity = 0.3
var camera_change = Vector2()

var velocity = Vector3()
var direction = Vector3()

#walk variables
var gravity = -9.8 * 3
const MAX_SPEED = 30
const acceleration = 5

#jumping
var jump_height = 10

func _ready():
	# Called every time the node is added to the scene.
	# Initialization here
	pass

func _physics_process(delta):
	aim()
	move(delta)

func _input(event):
	if event is InputEventMouseMotion:
		camera_change = event.relative
		
func move(delta):
	# reset the direction of the player
	direction = Vector3()
	
	# get the rotation of the camera
	var aim = $Head/Camera.get_global_transform().basis

	# check input and change direction
	if Input.is_action_pressed("move_forward"):
		direction -= aim.z
	if Input.is_action_pressed("move_backward"):
		direction += aim.z
	if Input.is_action_pressed("move_left"):
		direction -= aim.x
	if Input.is_action_pressed("move_right"):
		direction += aim.x
	direction.y = 0
	direction = direction.normalized()
	velocity.y += gravity * delta
	
	var temp_velocity = velocity
	temp_velocity.y = 0
	
	# where would the player go at max speed
	var target = direction * MAX_SPEED
	
	# calculate a portion of the distance to go
	temp_velocity = temp_velocity.linear_interpolate(target, acceleration * delta)
	velocity.x = temp_velocity.x
	velocity.z = temp_velocity.z
	
	if Input.is_action_just_pressed("jump"):
		velocity.y = jump_height

	# move
	velocity = move_and_slide(velocity, Vector3(0, 1, 0))

func aim():
	if camera_change.length() > 0:
		$Head.rotate_y(deg2rad(-camera_change.x * mouse_sensitivity))

        # limit how far up/down we look
		var change = -camera_change.y * mouse_sensitivity
		if change + camera_angle < 90 and change + camera_angle > -90:
            # invert mouse (-change)
			$Head/Camera.rotate_x(deg2rad(-change))
			camera_angle += change
		camera_change = Vector2()
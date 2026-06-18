# Handles code when entering the Fall State.
class_name PlayerFall
extends StateParent

@export var groundDetection : RayCast2D
@export var airborneTimer : Timer
var airAcceleration : float = 2.915; var airFriction : float = 12.72; 
var airMovement : float = 172.25 # Float types

func EnterState() :
	playerReference.playerAnimations.play("Fall")
	airborneTimer.start();
	# print("State: Fall")

func UpdateState(_delta : float) :
	var timeElapsed : float = airborneTimer.wait_time - airborneTimer.time_left
	if playerReference.is_on_wall() && playerReference.wallDetection.is_colliding() && InputManager.horizontalButton() != 0.0 : finiteStateMachine.changeToState("Wall")
	elif playerReference.is_on_floor() && groundDetection.is_colliding() :
		if InputManager.horizontalButton() != 0.0 : finiteStateMachine.changeToState("Move")
		# elif timeElapsed >= 2.0 : finiteStateMachine.changeToState("Land")
	flipCharacter()

func PhysicsUpdate(_delta : float) : # Handles code that deals with physics-related movement.
	if InputManager.horizontalButton() != 0.0 : playerReference.velocity = Vector2(move_toward(playerReference.velocity.x, airMovement * InputManager.horizontalButton(), airAcceleration), playerReference.velocity.y)
	else : playerReference.velocity = Vector2(move_toward(playerReference.velocity.x, 0.0, airFriction), playerReference.velocity.y)

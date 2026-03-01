# Handles code when entering the Fall State.
class_name PlayerFall
extends StateParent

@export var groundDetection : RayCast2D
@export var airborneTimer : Timer
var airAcceleration : float = 2.915; var airFriction : float = 12.72; # Float types
var airMovement : float = 172.25; var coyoteCounter : float = 1.06; # Float types

func EnterState() :
	playerController.debugText.Text = "[center]State: Fall[/center]"
	playerController.playerAnimations.Play("Fall")
	airborneTimer.Start();

func UpdateState(_delta : float) :
	var timeElapsed : float = airborneTimer.wait_time - airborneTimer.time_left
	if playerController.is_on_wall() && playerController.wallDetection.is_colliding() && inputManager.horizontalButton() != 0.0 : finiteStateMachine.StateTransition("WallState")
	else : if playerController.is_on_floor() && groundDetection.is_colliding() :
		if inputManager.horizontalButton() != 0.0 : finiteStateMachine.StateTransition("MoveState")
		else : if inputManager.horizontalButton() == 0.0 : finiteStateMachine.StateTransition("Idle")
		else : if timeElapsed >= 2.0 : finiteStateMachine.StateTransition("LandState")
	playerController.flipCharacter()

func PhysicsUpdate(_delta : float) : # Handles code that deals with physics-related movement.
	if inputManager.horizontalButton() != 0.0 :
		playerController.velocity = Vector2(move_toward(playerController.velocity.x, airMovement * inputManager.horizontalButton(), airAcceleration), playerController.velocity.y)
	else : playerController.velocity = Vector2(move_toward(playerController.velocity.x, 0.0, airFriction), playerController.velocity.y)
# Handles code when entering the Crouch State.
class_name PlayerCrouch
extends StateParent

@export_group("Collisions")
@export var crouchCollision : Shape2D

var crouchFriction : float = 1.325; # Float type
var isCrouching : bool = false; # Boolean type

func EnterState() : 
	playerController.debugText.text = "[center]State: Crouch[/center]";

func UpdateState(_delta : float) :
	if inputManager.crouchButton() :
		playerController.pauseInputOnAnimation()
		playerController.playerAnimations.play("Crouch") # Continuosly plays crouch animation.
	else :
		set_process_input(true);
		playerController.playerAnimations.play("Crouch_Recover");
		playerController.playerAnimations.animation_finished.connect(finiteStateMachine.StateTransition("IdleState"))

func PhysicsUpdate(_delta : float) :
	if playerController.velocity.x != 0.0 : playerController.velocity = Vector2(playerController.calculateVelocity(playerController.velocity.x, 0.0, crouchFriction), playerController.velocity.y)
	else : playerController.velocity = Vector2(0.0, playerController.velocity.y)

func ExitState() : playerController.playerAnimations.play("Crouch_Recover")
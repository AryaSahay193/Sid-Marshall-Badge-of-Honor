#Handles code when entering Wall-State.
class_name PlayerWalled
extends StateParent

var wallPushback : float = 343.44; var wallJumpHeight : float = -227.9; 
var wallSlideSpeed : float = 1298.50; var wallSlideAcceleration : float = 26.5;

func EnterState() :
	playerController.debugText.text = "[center]State: Wall[/center]"
	playerController.playerAnimations.play("Wall_Contact")

func UpdateState(_delta : float) :
	if (playerController.is_on_wall() && playerController.wallDetection.is_colliding() && inputManager.horizontalButton() != 0.0) :
		if inputManager.jumpButton() : playerController.playerAnimations.play("Wall_Kick")
		else : playerController.playerAnimations.play("Wall_Slide")
	else : finiteStateMachine.StateTransition("FallState")
	playerController.flipCharacter()

func PhysicsUpdate(_delta : float) :
	playerController.velocity = Vector2(playerController.velocity.x, 0.0) # Cancels gravity when holding towards a wall.
	if inputManager.jumpButton() : playerController.velocity = Vector2((wallPushback * -inputManager.horizontalButton()), wallJumpHeight)
	else : playerController.velocity = Vector2(playerController.velocity.x, move_toward(playerController.velocity.y, wallSlideSpeed, wallSlideAcceleration))
# Handles code when entering the Slide State.
class_name PlayerSlide
extends StateParent

var groundSlideSpeed : float = 188.15; var slideAcceleration : float = 4.293; var slideFriction : float = 0.8427;  # Float types

func EnterState() :
	playerController.debugText.text = "[center]State: Slide[/center]"
	playerController.playerAnimations.play("Slide")
	playerController.pauseInputOnAnimation()

func ExitState() :
	playerController.playerAnimations.play("Slide_Recover")
	set_process_input(true)

func UpdateState(_delta : float) :
	playerController.pauseInputOnAnimation();
	if playerController.is_on_floor() :
		if !inputManager.crouchButton() || playerController.Velocity.X == 0.0 :
			playerController.playerAnimations.play("Slide_Recover")
			playerController.playerAnimations.animation_finished.connect(finiteStateMachine.StateTransition("IdleState")) 
		else : playerController.playerAnimations.play("Slide_Loop")
	else : finiteStateMachine.StateTransition("FallState")

func PhysicsUpdate(_delta : float) :
	set_process_input(false)
	if inputManager.crouchButton() && inputManager.horizontalButton() != 0.0 :
		playerController.velocity = Vector2(playerController.calculateVelocity(playerController.velocity.x, 0.0, slideFriction), playerController.velocity.y);
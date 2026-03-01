# Handles code when entering Move-State.
class_name PlayerMove
extends StateParent

var walkingSpeed : float = 53.0; var maximumSpeed : float = 164.3; var acceleration : float = 2.65; var friction : float = 3.18 # Float types
var grassWalkSFX : AudioStreamPlayer

func EnterState() : # Enters the Move-State, code for walking animation and movement.
	playerController.debugText.Text = "[center]State: Move[/center]"
	if inputManager.horizontalButton() != 0.0 :
		if inputManager.runButton() : playerController.playerAnimations.play("Run")
		else : playerController.playerAnimations.play("Walk")

func UpdateState(_delta : float) :
	if playerController.is_on_floor() :
		if playerController.velocity.x != 0.0 :
			if inputManager.crouchButton() :
				if playerController.velocity.x == maximumSpeed * inputManager.horizontalButton() : finiteStateMachine.StateTransition("SlideState") # Change to Slide State.
				else : finiteStateMachine.StateTransition("CrouchState")
			if inputManager.horizontalButton() != sign(playerController.velocity.x) : playerController.playerAnimations.play("Skid")
			else : if inputManager.runButton() && (playerController.velocity.x > walkingSpeed || playerController.velocity.x < -walkingSpeed) : playerController.playerAnimations.Play("Run")
			else : if playerController.velocity.x < walkingSpeed || playerController.velocity.x > -walkingSpeed : playerController.playerAnimations.play("Walk")
		else : finiteStateMachine.StateTransition("IdleState") # Change to Idle State.
	else : if playerController.velocity.y > 0.0 : finiteStateMachine.StateTransition("FallState") # Change to Fall State.
	playerController.flipCharacter();

func PhysicsUpdate(_delta : float) : # Handles code that deals with physics-related movement.
	if playerController.is_on_floor_only() :
		if inputManager.horizontalButton() != 0.0 :
			if inputManager.runButton() : playerController.velocity = Vector2(playerController.calculateVelocity(playerController.velocity.x, maximumSpeed, acceleration), playerController.velocity.y) # Run-movement code;
			else : playerController.velocity = Vector2(playerController.calculateVelocity(playerController.velocity.x, walkingSpeed, acceleration), playerController.velocity.y) # Walk-movement code.
		else : if inputManager.horizontalButton() == 0.0 && playerController.velocity.x != 0.0 : playerController.velocity = Vector2(playerController.calculateVelocity(playerController.velocity.x, 0.0, friction), playerController.velocity.y) # Player-friction code.

func HandleInput(_event : InputEvent) :
	if playerController.is_on_floor_only() && !playerController.is_on_wall() :
		if inputManager.horizontalButton() == 0.0 && playerController.velocity.x == 0.0 : finiteStateMachine.StateTransition("IdleState") # Change to Idle State.
		else : if inputManager.jumpButton() : finiteStateMachine.StateTransition("JumpState"); # Change to Jump State.
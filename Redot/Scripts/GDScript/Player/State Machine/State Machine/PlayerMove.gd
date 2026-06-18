# Handles code when entering Move-State.
class_name PlayerMove # Handles code when entering Move-State.
extends StateParent

@onready var isSkidding : bool = InputManager.horizontalButton() == 0 && playerReference.velocity.x != 0
var walkingSpeed : float = 53.0; var maximumSpeed : float = 164.3; var acceleration : float = 2.65; var friction : float = 3.18 # Float types
var grassWalkSFX : AudioStreamPlayer

func UpdateState(_delta : float) :
	if InputManager.runButton() : playerReference.playerAnimations.play("Run")
	else : playerReference.playerAnimations.play("Walk")
	
	# if InputManager.horizontalButton() != sign(playerReference.velocity.x) : playerReference.playerAnimations.play("Skid")
	#if InputManager.crouchButton() : 
	#	if playerReference.velocity.x == maximumSpeed * InputManager.horizontalButton() : finiteStateMachine.changeToState("Slide") # Change to Slide State.
	#	else : finiteStateMachine.changeToState("Crouch")

	if InputManager.horizontalButton() != sign(playerReference.velocity.x) : playerReference.playerAnimations.play("Skid")
	elif InputManager.horizontalButton() == 0 : 
		if playerReference.velocity.x == 0.0 : finiteStateMachine.changeToState("Idle") # Change to Idle State.
		else : playerReference.playerAnimations.play("Skid")
	elif playerReference.velocity.y > 0.0 : finiteStateMachine.changeToState("Fall") # Change to Fall State.
	flipCharacter()

func PhysicsUpdate(_delta : float) : # Handles code that deals with physics-related movement.
	if InputManager.runButton() : playerReference.velocity = Vector2(calculateVelocity(playerReference.velocity.x, maximumSpeed, acceleration), playerReference.velocity.y) # Run-movement code;
	else : playerReference.velocity = Vector2(calculateVelocity(playerReference.velocity.x, walkingSpeed, acceleration), playerReference.velocity.y) # Walk-movement code.

func ExitState() :
	playerReference.velocity = Vector2(calculateVelocity(playerReference.velocity.x, 0.0, friction), playerReference.velocity.y) # Player-friction code.

func HandleInput(_event : InputEvent) :
	if playerReference.is_on_floor_only() && !playerReference.is_on_wall() :
		if InputManager.horizontalButton() == 0 && playerReference.velocity.x == 0.0 : finiteStateMachine.changeToState("Idle") # Change to Idle State.
		elif InputManager.jumpButton() : finiteStateMachine.changeToState("Jump") # Change to Jump State.

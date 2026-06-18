# Handles code when entering the Jump State.
class_name PlayerJump
extends StateParent

@export var jumpSFX : AudioStreamPlayer
var horizontalJumpVelocity : float = 100.7; var airVelocity : float = 87.45
var jumpVelocity : float = -182.85; var pitchValue : float = 1.2
var coyoteCounter : float = 1.06; var coyoteTimer : float
var maximumJumps : int = 2; var numberOfJumps : int = 0

func EnterState() :
	playerReference.velocity = Vector2(playerReference.velocity.x, jumpVelocity)
	playerAnimations.play("Jump")
	# print("State: Jump")
	jumpSFX.play()
	numberOfJumps += 1

func UpdateState(_delta : float) :
	if !playerReference.is_on_floor() :
		if InputManager.jumpButton() && numberOfJumps < maximumJumps :
			if numberOfJumps == 2 : playerReference.playerAnimations.play("Double_Jump")
			jumpSFX.PitchScale *= 1.2
	else : numberOfJumps = 0
	if (playerReference.is_on_wall_only() && playerReference.wallDetection.is_colliding() && InputManager.horizontalButton() != 0.0) : finiteStateMachine.changeToState("Wall")
	elif playerReference.velocity.y >= 0.0 : finiteStateMachine.changeToState("Fall")

func PhysicsUpdate(_delta : float) :
	if InputManager.jumpButton() && numberOfJumps < maximumJumps :
		playerReference.velocity = Vector2(playerReference.velocity.x, jumpVelocity)
		numberOfJumps += 1

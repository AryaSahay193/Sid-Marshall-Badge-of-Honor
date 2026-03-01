# Handles code when entering the Jump State.
class_name PlayerJump
extends StateParent

@export var jumpSFX : AudioStreamPlayer
var horizontalJumpVelocity : float = 100.7; var airVelocity : float = 87.45;
var jumpVelocity : float = -182.85; var pitchValue : float = 1.2;
var maximumJumps : int = 2; var numberOfJumps : int = 0;

func EnterState() :
	playerController.debugText.text = "[center]State: Jump[/center]";
	playerController.velocity = Vector2(playerController.velocity.x, jumpVelocity)
	playerController.playerAnimations.play("Jump")
	jumpSFX.play()
	numberOfJumps += 1

func UpdateState(_delta : float) :
	if !playerController.is_on_floor() :
		if inputManager.jumpButton() && numberOfJumps < maximumJumps :
			if numberOfJumps == 2 : playerController.playerAnimations.play("Double_Jump")
			jumpSFX.PitchScale *= 1.2
	else : numberOfJumps = 0
	if (playerController.is_on_wall_only() && playerController.wallDetection.is_colliding() && inputManager.horizontalButton() != 0.0) : finiteStateMachine.StateTransition("WallState")
	else : if(playerController.velocity.y >= 0.0) : finiteStateMachine.StateTransition("FallState")

func PhysicsUpdate(_delta : float) :
	if inputManager.jumpButton() && numberOfJumps < maximumJumps :
		playerController.velocity = Vector2(playerController.Velocity.x, jumpVelocity)
		numberOfJumps += 1
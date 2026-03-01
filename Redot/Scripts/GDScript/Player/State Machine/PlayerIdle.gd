# Handles code when entering Idle-State.
class_name PlayerIdle
extends StateParent

func EnterState() :
	playerController.debugText.text = "[center]State: Idle[/center]"
	playerController.playerAnimations.play("Idle")

func UpdateState(_delta : float) :
	playerController.playerAnimations.play("Idle")
	if playerController.velocity.y > 0.0 : finiteStateMachine.StateTransition("FallState")
	playerController.flipCharacter()

func PhysicsUpdate(_delta : float) :
	playerController.velocity = Vector2(0.0, playerController.Velocity.y)


func HandleInput(_event : InputEvent) : # Changes states if they involve button presses.
	if playerController.is_on_floor_only() :
		if inputManager.lightAttackButton() || inputManager.heavyAttackButton() : finiteStateMachine.StateTransition("AttackCombo1")
		else : if inputManager.horizontalButton() != 0.0 : finiteStateMachine.StateTransition("MoveState")
		else : if inputManager.crouchButton() : finiteStateMachine.StateTransition("CrouchState")
		else : if inputManager.jumpButton() : finiteStateMachine.StateTransition("JumpState")
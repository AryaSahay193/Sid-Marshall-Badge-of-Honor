# Handles code when entering Idle-State.
class_name PlayerIdle
extends StateParent

func UpdateState(_delta : float) :
	flipCharacter()
	playerAnimations.play("Idle")
	if playerReference.is_on_floor() && !playerReference.is_on_wall() :
		if InputManager.horizontalButton() != 0 : finiteStateMachine.changeToState("Move") # Change to Move State.
		# elif InputManager.crouchButton() : finiteStateMachine.changeToState("Crouch") # Change to Crouch State.
	else : if playerReference.velocity.y > 0.0 : finiteStateMachine.changeToState("Fall") # Change to Fall State.

func PhysicsUpdate(_delta : float) :
	playerReference.velocity = Vector2(0.0, playerReference.velocity.y)

func HandleInput(_event : InputEvent) : # One-shot actions
	if playerReference.is_on_floor_only() :
		if InputManager.jumpButton() : finiteStateMachine.changeToState("Jump")
		# elif InputManager.lightAttackButton() || InputManager.heavyAttackButton() : finiteStateMachine.changeToState("Attack_1")

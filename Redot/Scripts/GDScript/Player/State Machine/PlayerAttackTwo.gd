# Handles code when entering the second attack combo.
class_name PlayerAttackTwo
extends StateParent

func EnterState() : 
	if inputManager.lightAttackButton() : playerController.playAnimationOnce("Attack_Light_Jab_Right")
	else : if inputManager.heavyAttackButton() : playerController.playAnimationOnce("Attack_Heavy_Hook_Right")
	set_physics_process(false)

func UpdateState(_delta : float) :
	playerController.playerAnimations.animation_finished.connect(finiteStateMachine.StateTransition("IdleState"))
	set_physics_process(true)

func PhysicsUpdate(_delta : float) :
	playerController.velocity = Vector2(0.0, 0.0)

func HandleInput(_event : InputEvent) : # Changes states if they involve button presses.
	if inputManager.lightAttackButton() || inputManager.heavyAttackButton() : finiteStateMachine.StateTransition("AttackCombo2")
	else : if !Input.is_anything_pressed() : finiteStateMachine.StateTransition("IdleState")
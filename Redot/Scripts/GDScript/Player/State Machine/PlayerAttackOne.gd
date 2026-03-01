# Handles code when entering the first attack combo.
class_name PlayerAttackOne
extends StateParent

func EnterState() : 
	if inputManager.lightAttackButton() : playerController.playAnimationOnce("Attack_Light_Jab_Left")
	else : if inputManager.heavyAttackButton() : playerController.playAnimationOnce("Attack_Heavy_Hook_Left")

func UpdateState(_delta : float) :
	set_physics_process(false)
	playerController.playerAnimations.animation_finished.connect(finiteStateMachine.StateTransition("IdleState"))

func PhysicsUpdate(_delta : float) :
	playerController.velocity = Vector2(0.0, 0.0)

func HandleInput(_event : InputEvent) : # Changes states if they involve button presses.
	if inputManager.lightAttackButton() || inputManager.heavyAttackButton() : finiteStateMachine.StateTransition("AttackCombo2")
	else : if !Input.is_anything_pressed() : finiteStateMachine.StateTransition("IdleState")
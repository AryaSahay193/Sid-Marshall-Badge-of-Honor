# Handles code when entering the Land State.
class_name PlayerLand
extends StateParent

@export var airborneTimer : Timer

func EnterState() :
	playerController.debugText.Text = "[center]State: Land[/center]";
	playerController.playerAnimations.play("Land");
	airborneTimer.stop();

func UpdateState(_delta : float) :
	playerController.playerAnimations.animation_finished.connect(finiteStateMachine.StateTransition("IdleState"))
	playerController.pauseInputOnAnimation()

func PhysicsUpdate(_delta : float) :
	playerController.velocity = Vector2(0, 0)

func ExitState() :
	set_physics_process(true)
	set_process_input(true)
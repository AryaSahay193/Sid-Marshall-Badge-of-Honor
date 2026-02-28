using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class IdleState : ParentState { //Handles code when entering Idle-State.
    public override void EnterState() {
		playerAnimations.Play("Idle");
	}
	
	public override void UpdateState(float delta) {
		playerAnimations.Play("Idle");
		if(playerController.Velocity.Y > 0.0f) finiteStateMachine.StateTransition("FallState");
		flipCharacter();
	}

    public override void PhysicsUpdate(float delta) {
		playerController.Velocity = playerController.Velocity with {X = 0.0f};
    }


    public override void HandleInput(InputEvent @event) { //Changes states if they involve button presses.
		if(playerController.IsOnFloorOnly()) {
			if(inputManager.lightAttackButton() || inputManager.heavyAttackButton()) finiteStateMachine.StateTransition("AttackCombo1");
			else if(inputManager.horizontalButton() != 0.0f) finiteStateMachine.StateTransition("MoveState");
			else if(inputManager.crouchButton()) finiteStateMachine.StateTransition("CrouchState");
			else if(inputManager.jumpButton()) finiteStateMachine.StateTransition("JumpState");
		}
	}
}

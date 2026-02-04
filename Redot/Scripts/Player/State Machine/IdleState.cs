using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class IdleState : ParentState { //Handles code when entering Idle-State.
    public override void EnterState() {
		GD.Print("Idle State");
		playerController.characterVelocity = Vector2.Zero;
		playerAnimations.Play("Idle");
	}
	
	public override void UpdateState(float delta) {
		if(!playerController.IsOnFloor() || characterVelocity.Y > 0.0f) finiteStateMachine.StateTransition("FallState");
		else playerAnimations.Play("Idle");
		flipCharacter();
	}

    public override void PhysicsUpdate(float delta) {
		playerController.characterVelocity = Vector2.Zero;
    }


    public override void HandleInput(InputEvent @event) { //Changes states if they involve button presses.
        if(inputManager.horizontalButton() != 0.0f) finiteStateMachine.StateTransition("MoveState");
		else if(inputManager.jumpButton()) finiteStateMachine.StateTransition("JumpState");
		else if(inputManager.lightAttackButton() || inputManager.heavyAttackButton()) finiteStateMachine.StateTransition("AttackCombo1");
	}
}

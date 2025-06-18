using Godot;
using System;

public partial class IdleState : BaseStateClass {
	private void EnterState() {
		playerAnimations.Play("Idle");
	}

	private void UpdateState() {
		if(moveDirection.X != 0.0f) finiteStateMachine.StateTransition("Sid_Move"); //Change state to movement.
		if(jumpButton) finiteStateMachine.StateTransition("Sid_Jump"); //Change to Jump State.
		if(kickButton) finiteStateMachine.StateTransition("Sid_Kick"); //Change to Kick State.
		if(punchButton) finiteStateMachine.StateTransition("Sid_Punch"); //Change to Punch State
		if(crouchButton) finiteStateMachine.StateTransition("Sid_Crouch"); //Change to Crouch State.
	}
}

using Godot;
using System;

public partial class CrouchState : BaseStateClass {
	private float groundSlideSpeed = 3.5f;

    public override void EnterState() => playerReference.playerAnimations.Play("Crouch");
	
	public override void UpdateState(float delta) {
		if(playerReference.IsOnFloor() && playerReference.characterVelocity.X == 0.0f && Input.IsActionJustReleased("player_crouch")) {
			finiteStateMachine.StateTransition(IdleState); //Change to Idle State.
		} else if(!playerReference.IsOnFloor() && playerReference.characterVelocity.Y <= 0.0f) {
			finiteStateMachine.StateTransition(JumpState); //Change to Jump State.
		}
	}

	public override void PhysicsUpdate(float delta) {}

	public override void ExitState() {
		playerReference.playerAnimations.Play("Crouch_Recover");
	}
}

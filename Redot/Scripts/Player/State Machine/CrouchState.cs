using Godot;
using System;

public partial class CrouchState : BaseStateClass {
	private BaseStateClass baseState;
	private float groundSlideSpeed = 3.5f;

    public override void _Ready() => baseState = new BaseStateClass();

    public void EnterState() => baseState.playerAnimations.Play("Crouch");
	
	public void UpdateState(float delta) {
		if(isGrounded && playerReference.characterVelocity.X == 0.0f && Input.IsActionJustReleased("player_crouch")) {
			finiteStateMachine.StateTransition(IdleState); //Change to Idle State.
		} else if(!isGrounded && playerReference.characterVelocity.Y <= 0.0f) {
			finiteStateMachine.StateTransition(JumpState); //Change to Jump State.
		}
	}

	public void PhysicsUpdate(float delta) {}

	public void ExitState() {
		baseState.playerAnimations.Play("Crouch_Recover");
	}
}

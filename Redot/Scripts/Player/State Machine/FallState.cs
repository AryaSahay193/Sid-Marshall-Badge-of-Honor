using Godot;
using System;

public partial class FallState : BaseStateClass {
	private float airVelocity = 26.5f, airAcceleration = 4.24f, coyoteCounter = 1.5f;
	private BaseStateClass baseState;
	private Vector2 fallMovement;

    public override void _Ready() => baseState = new BaseStateClass();

	//Handles code when entering the Fall State.
    private void EnterState() => baseState.playerAnimations.Play("Fall");
	
	private void UpdateState(float delta) {
		if(isGrounded || isWalled) coyoteCounter = 0.0f;
		else coyoteCounter -= delta;

		if(isGrounded) finiteStateMachine.StateTransition("Sid_Idle");
		//if(characterHealth == 0) finiteState.Machine.StateTransition("Sid_Death");
	}

	//Handles code that deals with physics-related movement.
	private void PhysicsUpdate(float delta) {
		float fallMovementX = Mathf.MoveToward(playerReference.characterVelocity.X, airVelocity, airAcceleration) * playerReference.moveDirection.X;
		fallMovement += new Vector2(fallMovementX, playerReference.gravityValue * delta);
	}
}

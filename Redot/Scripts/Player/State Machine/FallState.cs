using Godot;
using System;

public partial class FallState : BaseStateClass {
	private float airVelocity = 26.5f, airAcceleration = 4.24f, coyoteCounter = 1.5f;
	private Vector2 fallMovement;

	//Handles code when entering the Fall State.
    public override void EnterState() => playerAnimations.Play("Fall");
	
	public override void UpdateState(float delta) {
		if(isGrounded || isWalled) coyoteCounter = 0.0f;
		else coyoteCounter -= delta;

		if(isGrounded) finiteStateMachine.StateTransition("Sid_Idle");
		//if(characterHealth == 0) finiteState.Machine.StateTransition("Sid_Death");
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		float fallMovementX = Mathf.MoveToward(characterVelocity.X, airVelocity, airAcceleration) * moveDirection.X;
		fallMovement += new Vector2(fallMovementX, gravityValue * delta);
	}
}

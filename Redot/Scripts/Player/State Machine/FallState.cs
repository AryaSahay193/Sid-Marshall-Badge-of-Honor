using Godot;
using System;

public partial class FallState : BaseStateClass {
	private float gravityValue = 312.7f, airVelocity = 26.5f, airAcceleration = 4.24f;
	private Vector2 fallMovement;
	
	private void EnterState() => playerAnimations.Play("Fall");
	private void Update(float delta) {
		performFall(delta);
		coyoteCounter -= delta;
	}

	private void ExitState() {
		if(isGrounded) finiteStateMachine.StateTransition("Sid_Idle");
	}

	public void performFall(float delta) {
		float fallMovementX = Mathf.MoveToward(velocity.X, airVelocity, airAcceleration) * moveDirection.X;
		float fallMovementY = gravityValue * (float)delta; //Sets the character gravity.
		fallMovement += new Vector2(fallMovementX, fallMovementY);
	}
}

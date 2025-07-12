using Godot;
using System;

public partial class FallState : BaseStateClass {
	private float airVelocity = 26.5f, airAcceleration = 4.24f, coyoteCounter = 1.5f;
	private BaseStateClass baseState;
	private Vector2 fallMovement;

    public override void _Ready() => baseState = new BaseStateClass();

	//Handles code when entering the Fall State.
    private void EnterState() => baseState.playerAnimations.Play("Fall");
	
	private void FrameUpdate(float delta) {
		coyoteCounter -= (float)delta;
		if(isGrounded) coyoteCounter = 0.0f;
	}

	//Handles code that deals with physics-related movement.
	private void PhysicsUpdate(float delta) {
		float fallMovementX = Mathf.MoveToward(characterVelocity.X, airVelocity, airAcceleration) * moveDirection.X;
		characterVelocity.Y += playerReference.gravityValue * delta; //Acting gravity force applied.
		fallMovement += new Vector2(fallMovementX, characterVelocity.Y);
		playerReference.MoveAndSlide(); //Calls the function so the character can move.

		if(isGrounded) finiteStateMachine.StateTransition("Sid_Idle");
	}
}

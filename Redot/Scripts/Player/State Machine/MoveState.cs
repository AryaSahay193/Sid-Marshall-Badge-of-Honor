using Godot;
using System;

public partial class MoveState : BaseStateClass {
	public float walkingSpeed = 53.0f, runningSpeed = 159.0f, acceleration = 3.0f, runningAcceleration = 6.0f, friction = 5.0f;
	private AudioStreamPlayer grassWalkSoundEffect, grassRunSoundEffect;

	//Handles code when entering Move-State.
    public override void EnterState() { //Enters the Move-State, code for walking animation and movement.
		playerAnimations.Play("Walk");
		if(characterVelocity.X == runningSpeed) playerAnimations.Play("Run");
	}

	//Handles code when exiting Move-State.
	public override void ExitState() {
		characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, friction);
		playerAnimations.Play("Skid");
	}

	public override void UpdateState(float delta) {
		if(isGrounded && characterVelocity.X == 0.0f) {
			finiteStateMachine.StateTransition(IdleState); //Change to Idle State.
		} else if(!isGrounded && characterVelocity.Y <= 0.0f) {
			finiteStateMachine.StateTransition(JumpState); //Change to Jump State.
		} else if(!isGrounded && characterVelocity.Y >= 0.0f) {
			finiteStateMachine.StateTransition(FallState); //Change to Fall State.
		}
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		float currentVelocity;	
		if(runButton) {
			currentVelocity = moveDirection.X * runningSpeed;
			characterVelocity.X = Mathf.MoveToward(characterVelocity.X, currentVelocity, runningAcceleration);
		} else {
			currentVelocity = moveDirection.X * walkingSpeed;
			characterVelocity.X = Mathf.MoveToward(characterVelocity.X, currentVelocity, acceleration);
		} flipCharacter(characterVelocity.X);
	}
}
using Godot;
using System;

public partial class MoveState : BaseStateClass {
	public float walkingSpeed = 53.0f, runningSpeed = 159.0f, acceleration = 3.0f, runningAcceleration = 6.0f, friction = 5.0f;
	private AudioStreamPlayer grassWalkSoundEffect, grassRunSoundEffect;
	private BaseStateClass baseState;

    public override void _Ready() => baseState = new BaseStateClass();

	//Handles code when entering Move-State.
    private void EnterState() { //Enters the Move-State, code for walking animation and movement.
		baseState.playerAnimations.Play("Walk");
		if(playerReference.characterVelocity.X == runningSpeed) baseState.playerAnimations.Play("Run");
	}

	//Handles code when exiting Move-State.
	private void ExitState() {
		playerReference.characterVelocity.X = Mathf.MoveToward(playerReference.characterVelocity.X, 0.0f, friction);
		baseState.playerAnimations.Play("Skid");
	}

	private void UpdateState(float delta) {
		if(isGrounded && playerReference.characterVelocity.X == 0.0f) {
			finiteStateMachine.StateTransition(IdleState); //Change to Idle State.
		} else if(!isGrounded && playerReference.characterVelocity.Y <= 0.0f) {
			finiteStateMachine.StateTransition(JumpState); //Change to Jump State.
		} else if(!isGrounded && playerReference.characterVelocity.Y >= 0.0f) {
			finiteStateMachine.StateTransition(FallState); //Change to Fall State.
		}
	}

	//Handles code that deals with physics-related movement.
	private void PhysicsUpdate(float delta) {	
		if(runButton) {
			currentVelocity = playerReference.moveDirection.X * runningSpeed;
			playerReference.characterVelocity.X = Mathf.MoveToward(playerReference.characterVelocity.X, currentVelocity, runningAcceleration);
		} else {
			currentVelocity = playerReference.moveDirection.X * walkingSpeed;
			playerReference.characterVelocity.X = Mathf.MoveToward(playerReference.characterVelocity.X, currentVelocity, acceleration);
		} baseState.flipCharacter(playerReference.characterVelocity.X);
	}
}

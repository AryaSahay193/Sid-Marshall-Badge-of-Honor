using Godot;
using System;

public partial class MoveState : BaseStateClass {
	public float walkingSpeed = 53.0f, runningSpeed = 159.0f, acceleration = 3.0f, runningAcceleration = 6.0f, friction = 5.0f;
	private AudioStreamPlayer grassWalkSoundEffect, grassRunSoundEffect;

	//Handles code when entering Move-State.
    public override void EnterState() { //Enters the Move-State, code for walking animation and movement.
		playerReference.playerAnimations.Play("Walk");
		if(playerReference.characterVelocity.X == runningSpeed) playerReference.playerAnimations.Play("Run");
	}

	//Handles code when exiting Move-State.
	public override void ExitState() {
		playerReference.characterVelocity.X = Mathf.MoveToward(playerReference.characterVelocity.X, 0.0f, friction);
		playerReference.playerAnimations.Play("Skid");
	}

	public override void UpdateState(float delta) {
		if(playerReference.IsOnFloor() && playerReference.characterVelocity.X == 0.0f) {
			finiteStateMachine.StateTransition(IdleState); //Change to Idle State.
		} else if(!playerReference.IsOnFloor() && playerReference.characterVelocity.Y <= 0.0f) {
			finiteStateMachine.StateTransition(JumpState); //Change to Jump State.
		} else if(!playerReference.IsOnFloor() && playerReference.characterVelocity.Y >= 0.0f) {
			finiteStateMachine.StateTransition(FallState); //Change to Fall State.
		}
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		float currentVelocity;	
		if(playerReference.runButton) {
			currentVelocity = playerReference.moveDirection.X * runningSpeed;
			playerReference.characterVelocity.X = Mathf.MoveToward(playerReference.characterVelocity.X, currentVelocity, runningAcceleration);
		} else {
			currentVelocity = playerReference.moveDirection.X * walkingSpeed;
			playerReference.characterVelocity.X = Mathf.MoveToward(playerReference.characterVelocity.X, currentVelocity, acceleration);
		} playerReference.flipCharacter(playerReference.characterVelocity.X);
	}
}
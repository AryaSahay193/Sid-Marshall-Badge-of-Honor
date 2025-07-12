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
		if(characterVelocity.X == runningSpeed) baseState.playerAnimations.Play("Run");
	}

	//Handles code when exiting Move-State.
	private void ExitState() {
		characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, friction);
		baseState.playerAnimations.Play("Skid");
	}

	//Handles code that deals with physics-related movement.
	private void PhysicsUpdate(float delta) {
		if(runButton) {
			currentVelocity = moveDirection.X * runningSpeed;
			characterVelocity.X = Mathf.MoveToward(characterVelocity.X, currentVelocity, runningAcceleration);
		} else {
			currentVelocity = moveDirection.X * walkingSpeed;
			characterVelocity.X = Mathf.MoveToward(characterVelocity.X, currentVelocity, acceleration);
		} flipCharacter();

		characterVelocity.Y += playerReference.gravityValue * delta; //Acting gravity force applied.
		playerReference.MoveAndSlide(); //Calls the function so the character can move.
		
		if(isGrounded && characterVelocity.X == 0.0f) {
			finiteStateMachine.StateTransition("Sid_Idle"); //Change to Idle State.
		} else if(!isGrounded && characterVelocity.Y <= 0.0f) {
			finiteStateMachine.StateTransition("Sid_Jump"); //Change to Jump State.
		} else if(!isGrounded && characterVelocity.Y >= 0.0f) {
			finiteStateMachine.StateTransition("Sid_Fall"); //Change to Fall State.
		}
	}
}

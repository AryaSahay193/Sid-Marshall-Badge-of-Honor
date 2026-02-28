using Godot;
using System;

public partial class MoveState : ParentState {
	private float walkingSpeed = 53.0f, maximumSpeed = 164.3f, acceleration = 2.65f, friction = 3.18f;
	private AudioStreamPlayer grassWalkSFX;

	//Handles code when entering Move-State.
    public override void EnterState() { //Enters the Move-State, code for walking animation and movement.
		if(playerController.Velocity.X == maximumSpeed * inputManager.horizontalButton()) playerAnimations.Play("Run");
		else playerAnimations.Play("Walk");
	}

	//Handles code when exiting Move-State.
	public override void ExitState() {
		playerController.Velocity = playerController.Velocity with {X = Mathf.MoveToward(playerController.Velocity.X, 0.0f, friction)}; //Player-friction code.
		playerAnimations.Play("Skid");
	}

	public override void UpdateState(float delta) {
		if(playerController.IsOnFloor()) {
			if(playerController.Velocity == Vector2.Zero) finiteStateMachine.StateTransition("IdleState"); //Change to Idle State.
				if(playerController.Velocity.X != 0.0f) {
				if(inputManager.runButton() && (playerController.Velocity.X > walkingSpeed || playerController.Velocity.X < -walkingSpeed)) playerAnimations.Play("Run");
				else if(playerController.Velocity.X < walkingSpeed || playerController.Velocity.X > -walkingSpeed) playerAnimations.Play("Walk");
			}
		} else if(playerController.Velocity.Y > 0.0f) finiteStateMachine.StateTransition("FallState"); //Change to Fall State.
		flipCharacter();
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		if(playerController.IsOnFloorOnly()) {
			if(inputManager.horizontalButton() != 0.0f) {
				if(inputManager.runButton()) playerController.Velocity = playerController.Velocity with {X = calculateVelocity(playerController.Velocity.X, maximumSpeed, acceleration)}; //Run-movement code;
				else playerController.Velocity = playerController.Velocity with {X = calculateVelocity(playerController.Velocity.X, walkingSpeed, acceleration)}; //Walk-movement code.
			} else if(inputManager.horizontalButton() == 0.0f && playerController.Velocity.X != 0.0f) playerController.Velocity = playerController.Velocity with {X = calculateVelocity(playerController.Velocity.X, 0.0f, friction)}; //Player-friction code.
		}
	}

	private float calculateVelocity(float startingVelocity, float endingVelocity, float increment) {
		float oneAxisSpeed = Mathf.MoveToward(startingVelocity, endingVelocity * inputManager.horizontalButton(), increment);
		return oneAxisSpeed;
	}

    public override void HandleInput(InputEvent @event) {
		if(playerController.IsOnFloor()) {
			if(inputManager.horizontalButton() == 0.0f && playerController.Velocity.X == 0.0f) finiteStateMachine.StateTransition("IdleState"); //Change to Idle State.
			else if(inputManager.jumpButton()) finiteStateMachine.StateTransition("JumpState"); //Change to Jump State.
		}
    }
}

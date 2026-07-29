using Godot;
using System;

public partial class PlayerRun : BaseState {
	private float maximumSpeed = 164.3f, acceleration = 2.65f, friction = 3.18f;
	private AudioStreamPlayer grassWalkSFX;

	//Handles code when entering Move-State.
    public override void EnterState() { //Enters the Move-State, code for walking animation and movement.
		playerController.debugText.Text = "[center]State: Run[/center]";
	}

	//Handles code when exiting Move-State.
	public override void ExitState() {
		playerController.Velocity = playerController.Velocity with {X = Mathf.MoveToward(playerController.Velocity.X, 0.0f, friction)}; //Player-friction code.
		playerAnimations.Play("Skid");
	}

	public override void UpdateState(float delta) {
		if(playerController.Velocity.Y > 0.0f) finiteStateMachine.ChangeToState("FallState"); // Change to Fall State.
		if(inputManager.horizontalDirection() != 0.0f) {
			if(inputManager.runPressed()) {
				if(playerController.Velocity.X == maximumSpeed * inputManager.horizontalDirection()) playerAnimations.Play("Run"); 
				else playerAnimations.Play("Run_Transition"); 
			} else finiteStateMachine.ChangeToState("WalkState"); // Change to Walk State.
		} else {
			if(playerController.Velocity.X != 0.0f) playerAnimations.Play("Skid"); // playerAnimations.AnimationFinished += () => finiteStateMachine.ChangeToState("IdleState");
			else finiteStateMachine.ChangeToState("IdleState"); // Change to Idle State.
		} playerController.flipCharacter();
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		if(inputManager.horizontalDirection() != 0.0f) {
			if(inputManager.runPressed()) playerController.Velocity = playerController.Velocity with {X = calculateVelocity(playerController.Velocity.X, maximumSpeed, acceleration)}; // Run-movement code;
		} else playerController.Velocity = playerController.Velocity with {X = Mathf.MoveToward(playerController.Velocity.X, 0.0f, friction)}; //Player-friction code.
	}

    public override void HandleInput(InputEvent @event) {
		if(playerController.IsOnFloor()) {
			if(inputManager.jumpPressed()) finiteStateMachine.ChangeToState("JumpState"); // Change to Jump State.
		}
    }

	private float calculateVelocity(float startingVelocity, float endingVelocity, float increment) {
		float oneAxisSpeed = Mathf.MoveToward(startingVelocity, endingVelocity * inputManager.horizontalDirection(), increment);
		return oneAxisSpeed;
	}
}

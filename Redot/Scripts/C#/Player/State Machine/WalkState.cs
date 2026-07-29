using Godot;
using System;

public partial class PlayerWalk : BaseState {
	private float walkingSpeed = 53.0f, acceleration = 2.65f, friction = 3.18f;
	private AudioStreamPlayer grassWalkSFX;

	//Handles code when entering Move-State.
    public override void EnterState() { //Enters the Move-State, code for walking animation and movement.
		playerController.debugText.Text = "[center]State: Walk[/center]";
	}

	//Handles code when exiting Move-State.
	public override void ExitState() => playerController.Velocity = playerController.Velocity with {X = Mathf.MoveToward(playerController.Velocity.X, 0.0f, friction)}; //Player-friction code.

	public override void UpdateState(float delta) {
		if(playerController.Velocity.Y > 0.0f) finiteStateMachine.ChangeToState("FallState"); // Change to Fall State.
		
		if(inputManager.crouchPressed()) finiteStateMachine.ChangeToState("CrouchState"); // Change to Crouch State.
		if(inputManager.horizontalDirection() != 0.0) {
			if(inputManager.runPressed()) finiteStateMachine.ChangeToState("RunState"); // Change to Run State.
			else playerAnimations.Play("Walk");
		} else finiteStateMachine.ChangeToState("IdleState"); // Change to Idle State.
		playerController.flipCharacter();
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		if(inputManager.horizontalDirection() != 0.0f) {
			playerController.Velocity = playerController.Velocity with {X = calculateVelocity(playerController.Velocity.X, walkingSpeed, acceleration)}; // Walk-movement code.
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

/*public partial class PlayerMove : BaseState {
	private float walkingSpeed = 53.0f, maximumSpeed = 164.3f, acceleration = 2.65f, friction = 3.18f;
	private AudioStreamPlayer grassWalkSFX;

	//Handles code when entering Move-State.
    public override void EnterState() { //Enters the Move-State, code for walking animation and movement.
		playerController.debugText.Text = "[center]State: Move[/center]";
	}

	//Handles code when exiting Move-State.
	public override void ExitState() {
		playerController.Velocity = playerController.Velocity with {X = Mathf.MoveToward(playerController.Velocity.X, 0.0f, friction)}; //Player-friction code.
		playerAnimations.Play("Skid");
	}

	public override void UpdateState(float delta) {
		if(playerController.Velocity.X != 0.0f) {
			if(inputManager.runPressed()) {
				if(playerController.Velocity.X == maximumSpeed * inputManager.horizontalDirection()) playerAnimations.Play("Run"); 
				else playerAnimations.Play("Run_Transition"); 
			} else playerAnimations.Play("Walk");
		}
		
		if(inputManager.horizontalDirection() == 0.0f) {
			if(playerController.Velocity.X != 0.0f) {
				playerAnimations.Play("Skid");
				playerAnimations.AnimationFinished += () => finiteStateMachine.ChangeToState("IdleState");
			} else finiteStateMachine.ChangeToState("IdleState"); // Change to Idle State.
		} else if(playerController.Velocity.Y > 0.0f) finiteStateMachine.ChangeToState("FallState"); // Change to Fall State.
		playerController.flipCharacter();
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		if(inputManager.horizontalDirection() != 0.0f) {
			if(inputManager.runPressed()) playerController.Velocity = playerController.Velocity with {X = calculateVelocity(playerController.Velocity.X, maximumSpeed, acceleration)}; // Run-movement code;
			else playerController.Velocity = playerController.Velocity with {X = calculateVelocity(playerController.Velocity.X, walkingSpeed, acceleration)}; // Walk-movement code.
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
}*/

using Godot;
using System;

public partial class MoveState : ParentState {
	private float walkingSpeed = 53.0f, maximumSpeed = 164.3f, acceleration = 2.65f, friction = 3.18f;
	private AudioStreamPlayer grassWalkSFX;

	//Handles code when entering Move-State.
    public override void EnterState() { //Enters the Move-State, code for walking animation and movement.
		GD.Print("Move State");
		if(characterVelocity.X == maximumSpeed * inputManager.horizontalButton()) playerAnimations.Play("Run");
		else playerAnimations.Play("Walk");
	}

	//Handles code when exiting Move-State.
	public override void ExitState() {
		characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, friction);
		playerAnimations.Play("Skid");
	}

	public override void UpdateState(float delta) {
		if(characterVelocity.Y > 0.0f) finiteStateMachine.StateTransition("FallState"); //Change to Fall State.
		if(inputManager.runButton() && characterVelocity.X == maximumSpeed * inputManager.horizontalButton()) playerAnimations.Play("Run");
		else playerAnimations.Play("Walk");
		flipCharacter();
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		if(inputManager.horizontalButton() != 0.0f) {
            if(inputManager.runButton()) characterVelocity.X = Mathf.MoveToward(characterVelocity.X, maximumSpeed * inputManager.horizontalButton(), acceleration * 2.0f); //Run-movement code;
        	else characterVelocity.X = Mathf.MoveToward(characterVelocity.X, walkingSpeed * inputManager.horizontalButton(), acceleration); //Walk-movement code.
        } else characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, friction); //Player-friction code.
	}

    public override void HandleInput(InputEvent @event) {
		if(inputManager.horizontalButton() == 0.0f) finiteStateMachine.StateTransition("IdleState"); //Change to Idle State.
        else if(inputManager.jumpButton()) finiteStateMachine.StateTransition("JumpState"); //Change to Jump State.
    }
}

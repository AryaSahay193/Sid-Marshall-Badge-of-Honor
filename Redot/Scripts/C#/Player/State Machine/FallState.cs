using Godot;
using System;

public partial class FallState : ParentState {
	private float airAcceleration = 2.915f, airFriction = 12.72f;
	private float airMovement = 172.25f, coyoteCounter = 1.06f;

	//Handles code when entering the Fall State.
    public override void EnterState() { 
		playerAnimations.Play("Fall");
		GD.Print("Fall State");
	}
	
	public override void UpdateState(float delta) {
		if(playerController.IsOnFloorOnly()) finiteStateMachine.StateTransition("IdleState");
		flipCharacter();
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		if(inputManager.horizontalButton() != 0.0) {
			playerController.Velocity = playerController.Velocity with {X = Mathf.MoveToward(playerController.Velocity.X, airMovement * inputManager.horizontalButton(), airAcceleration)};
		} else playerController.Velocity = playerController.Velocity with {X = Mathf.MoveToward(playerController.Velocity.X, 0.0f, airFriction)};
	}
}

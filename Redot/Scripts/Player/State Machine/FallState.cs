using Godot;
using System;

public partial class FallState : ParentState {
	private float airAcceleration = 2.915f, coyoteCounter = 1.06f;
	private Vector2 fallMovement;

	//Handles code when entering the Fall State.
    public override void EnterState() { 
		GD.Print("Fall State");
		playerAnimations.Play("Fall");
	}
	
	public override void UpdateState(float delta) {
		if(playerController.IsOnFloor()) finiteStateMachine.StateTransition("IdleState");
		flipCharacter();
		//if(characterHealth == 0) finiteState.Machine.StateTransition("Sid_Death");
		//if(playerReference.IsOnFloor() || playerReference.IsOnWall()) coyoteCounter = 0.0f;
		//else coyoteCounter -= delta;
	}

	//Handles code that deals with physics-related movement.
	public override void PhysicsUpdate(float delta) {
		//characterVelocity.Y += (float)Math.Pow(playerController.gravityValue, 1.1f) * (float)delta; //Sets the character gravity.
		//GD.Print("Fall Velocity: " + characterVelocity);
		//float airMoveParameter = -1000.0f;
		//characterVelocity.Y += playerController.gravityValue;
		//characterVelocity = new Vector2(characterVelocity.X, characterVelocity.Y += playerController.gravityValue * (float)delta);
		/*if(characterVelocity.Y >= airMoveParameter) {
			if(airborneTimer.WaitTime - airborneTimer.TimeLeft <= 2.0f && groundDetection.IsColliding() && !IsOnWallOnly()) currentState = PlayerState.Land;
			else characterMovement(airVelocity, airAcceleration, airFriction);
		}*/
	}
}

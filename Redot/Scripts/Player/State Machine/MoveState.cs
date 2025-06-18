using Godot;
using System;

public partial class MoveState : BaseStateClass {
	private AudioStreamPlayer grassWalkSoundEffect, grassRunSoundEffect;
	public float walkingSpeed = 53.0f, runningSpeed = 159.0f, acceleration = 3.0f, friction = 5.0f;

	public new void EnterState() {
	}
	
	private void UpdateState(double delta) {
		performMovement();
		flipCharacter();
	}

	private new void ExitState() {
	}

	private void _UnhandledInput(InputEvent @event) {
		if(jumpButton) finiteStateMachine.StateTransition("Sid_Jump"); //Change to Jump State.
		if(kickButton) finiteStateMachine.StateTransition("Sid_Kick"); //Change to Kick State.
		if(punchButton) finiteStateMachine.StateTransition("Sid_Punch"); //Change to Punch State
	}

	private void performMovement() {
		float runningAcceleration = 6.0f;
		currentVelocity = moveDirection.X * walkingSpeed;
		if(moveDirection != Vector2.Zero) {
			grassWalkSoundEffect.Play();
			//whenAudioFinished();
			if(runButton) {
				currentVelocity = moveDirection.X * runningSpeed;
				velocity.X = Mathf.MoveToward(velocity.X, currentVelocity, runningAcceleration);
				playerAnimations.Play("Run");
				grassRunSoundEffect.Play();
			} else { 
				velocity.X = Mathf.MoveToward(velocity.X, currentVelocity, acceleration);
				playerAnimations.Play("Walk");
			}
		} else { 
			velocity.X = Mathf.MoveToward(velocity.X, 0.0f, friction);
			if(velocity.X != 0.0f && (velocity.X < walkingSpeed || velocity.X > -walkingSpeed)) playerAnimations.Play("Skid");
			finiteStateMachine.StateTransition("Sid_Idle");
		}
	}

	public void flipCharacter() {
		if(moveDirection.X != 0.0f) {
			if(moveDirection.X < 0.0f) playerAnimations.FlipH = true;
			else if(moveDirection.X > 0.0f) playerAnimations.FlipH = false;
		}
	}
}

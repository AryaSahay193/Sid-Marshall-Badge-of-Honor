using Godot;
using System;

public partial class JumpState : BaseStateClass {
	private float wallPushback = 10.0f, wallJumpHeight = 13.0f, wallSlideSpeed;
	private float jumpVelocity = -132.5f;
	private int maximumJumps = 2, numberOfJumps = 0;
	private AudioStreamPlayer jumpSoundEffect;

	public void EnterState() {
		playerAnimations.Play("Jump");
		jumpSoundEffect.Play();
	}
	
	public void UpdateState(float delta) => performJump(delta);
	public void ExitState() {
		if(velocity.Y > 0.0f) finiteStateMachine.StateTransition("Sid_Fall");
	}

	private void performJump(float delta) {
		if(Input.IsActionJustPressed("player_jump") && numberOfJumps < maximumJumps) {
			if(numberOfJumps == 1) { 
				playerAnimations.Play("Double-Jump");
				//jumpSoundEffect.PitchScale {jumpSoundEffect; 1.2f;}; 
			} if(numberOfJumps == 0) { 
				playerAnimations.Play("Jump");
				jumpSoundEffect.Play(); 
			}
			//whenAnimationFinished();
			velocity.Y = jumpVelocity;
			numberOfJumps += 1;
		} 
		if(playerReference.IsOnFloor()) numberOfJumps = 0;
		else if(numberOfJumps == 0) numberOfJumps += 1;
	}
}

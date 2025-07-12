using Godot;
using System;

public partial class JumpState : BaseStateClass {
	private float wallPushback = 10.0f, wallJumpHeight = 13.0f, wallSlideSpeed;
	private float jumpVelocity = -132.5f;
	private int maximumJumps = 2, numberOfJumps = 0;
	
	private AudioStreamPlayer jumpSoundEffect;
	private BaseStateClass baseState;

    public override void _Ready() => baseState = new BaseStateClass();

	//Handles code when entering the Jump State.
    public void EnterState() {
		baseState.playerAnimations.Play("Jump");
		jumpSoundEffect.Play();
		numberOfJumps += 1;
	}
	
	public void FrameUpdate() { 
		if(numberOfJumps == 1) { 
			baseState.playerAnimations.Play("Double-Jump");
			//jumpSoundEffect.PitchScale {jumpSoundEffect; 1.2f;}; 
		} if(numberOfJumps == 0) numberOfJumps += 1;
	}

	//Handles code that deals with physics-related movement.
	private void PhysicsUpdate(float delta) {
		characterVelocity.Y = jumpVelocity;
		characterVelocity.Y += playerReference.gravityValue * delta; //Acting gravity force applied.
		playerReference.MoveAndSlide(); //Calls the function so the character can move.

		if(!isGrounded && characterVelocity.Y >= 0.0f) {
			finiteStateMachine.StateTransition("Sid_Fall");
		} else if(isGrounded) {
			finiteStateMachine.StateTransition("Sid_Idle");
		} 
	}
}
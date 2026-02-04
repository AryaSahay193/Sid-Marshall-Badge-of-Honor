using Godot;
using System;

public partial class JumpState : ParentState {
	[Export] private AudioStreamPlayer jumpSoundEffect;
	private float wallPushback = 10.0f, wallJumpHeight = 13.0f, wallSlideSpeed;
	private float jumpVelocity = -119.25f, pitchValue = 1.2f;
	private int maximumJumps = 2, numberOfJumps = 0;

	//Handles code when entering the Jump State.
    public override void EnterState() {
		GD.Print("Jump State");
		numberOfJumps = maximumJumps;
		characterVelocity = new Vector2(characterVelocity.X, jumpVelocity);
		playerAnimations.Play("Jump");
	}
	
	public override void UpdateState(float delta) {
		if(inputManager.jumpButton()) {
			jumpSoundEffect.Play();
			numberOfJumps += 1;
		}

		if(playerController.IsOnFloor()) finiteStateMachine.StateTransition("IdleState"); 
		else if(!playerController.IsOnFloor() && characterVelocity.Y >= 0.0f) finiteStateMachine.StateTransition("FallState");
		
		if(numberOfJumps == 2) { 
			jumpSoundEffect.PitchScale *= pitchValue;
			playerAnimations.Play("Double_Jump"); 
		} if(numberOfJumps == 0) numberOfJumps += 1;
	}

    public override void PhysicsUpdate(float delta) {
        characterVelocity = new Vector2(characterVelocity.X, jumpVelocity);
    }
}

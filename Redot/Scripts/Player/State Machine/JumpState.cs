using Godot;
using System;

public partial class JumpState : ParentState {
	[Export] private AudioStreamPlayer jumpSoundEffect;
	private float horizontalJumpVelocity = 100.70f, airVelocity = 87.45f;
	private float jumpVelocity = -166.95f, pitchValue = 1.2f;
	private int maximumJumps = 2, numberOfJumps = 0;

	//Handles code when entering the Jump State.
    public override void EnterState() {
		playerController.Velocity = new Vector2(playerController.Velocity.X, jumpVelocity);
		playerAnimations.Play("Jump");
		jumpSoundEffect.Play();
	}
	
	public override void UpdateState(float delta) {
		if(playerController.Velocity.Y >= 0.0f) finiteStateMachine.StateTransition("FallState");
		if(inputManager.jumpButton() && numberOfJumps < maximumJumps) {
			playerAnimations.Play("Double_Jump");
			jumpSoundEffect.Play();
			numberOfJumps += 1;
		} if(playerController.IsOnFloor()) numberOfJumps = 0;
		
		if(numberOfJumps == 2) { 
			jumpSoundEffect.PitchScale *= pitchValue;
			playerAnimations.Play("Double_Jump"); 
		} if(numberOfJumps == 0) numberOfJumps += 1;
	}

    public override void PhysicsUpdate(float delta) {
        if(inputManager.jumpButton() && numberOfJumps < maximumJumps) {
			playerController.Velocity = new Vector2(playerController.Velocity.X, jumpVelocity);
			numberOfJumps += 1;
		}
    }
}

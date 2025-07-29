using Godot;
using System;

public partial class JumpState : BaseStateClass {
	[Export] private AudioStreamPlayer jumpSoundEffect;
	private float wallPushback = 10.0f, wallJumpHeight = 13.0f, wallSlideSpeed;
	private float jumpVelocity = -132.5f, pitchValue = 1.2f;
	private int maximumJumps = 2, numberOfJumps = 0;

	//Handles code when entering the Jump State.
    public override void EnterState() {
		playerReference.characterVelocity.Y = jumpVelocity;
		playerReference.playerAnimations.Play("Jump");
		jumpSoundEffect.Play();
		numberOfJumps += 1;
	}
	
	public override void UpdateState(float delta) { 
		if(numberOfJumps == 1) { 
			jumpSoundEffect.PitchScale *= pitchValue;
			playerReference.playerAnimations.Play("Double_Jump"); 
		} if(numberOfJumps == 0) numberOfJumps += 1;

		if(playerReference.IsOnFloor()) finiteStateMachine.StateTransition(IdleState); 
		else if(!playerReference.IsOnFloor() && playerReference.characterVelocity.Y >= 0.0f) finiteStateMachine.StateTransition(FallState);
		//else if(Input.IsActionJustPressed("Kick")) finiteStateMachine.StateTransition("Sid_Kick");
		//else if(Input.IsActionJustPressed("Punch")) finiteStateMachine.StateTransition("Sid_Punch");
	}
}
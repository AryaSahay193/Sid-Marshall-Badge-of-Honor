using Godot;
using System;

public partial class JumpState : BaseStateClass {
	private float wallPushback = 10.0f, wallJumpHeight = 13.0f, wallSlideSpeed;
	private float jumpVelocity = -132.5f, pitchValue = 1.2f;
	private int maximumJumps = 2, numberOfJumps = 0;
	private AudioStreamPlayer jumpSoundEffect;
	private BaseStateClass baseState;

    public override void _Ready() { 
		jumpSoundEffect = GetNode<AudioStreamPlayer>(".../GameWorld/SidMarshall/Sounds/SFX_Jump");
		baseState = new BaseStateClass();
	}

	//Handles code when entering the Jump State.
    public void EnterState() {
		playerReference.characterVelocity.Y = jumpVelocity;
		baseState.playerAnimations.Play("Jump");
		jumpSoundEffect.Play();
		numberOfJumps += 1;
	}
	
	public void UpdateState(float delta) { 
		if(numberOfJumps == 1) { 
			baseState.playerAnimations.Play("Double_Jump");
			jumpSoundEffect.PitchScale *= pitchValue; 
		} if(numberOfJumps == 0) numberOfJumps += 1;

		if(isGrounded) finiteStateMachine.StateTransition(IdleState); 
		else if(!isGrounded && playerReference.characterVelocity.Y >= 0.0f) finiteStateMachine.StateTransition(FallState);
		//else if(Input.IsActionJustPressed("Kick")) finiteStateMachine.StateTransition("Sid_Kick");
		//else if(Input.IsActionJustPressed("Punch")) finiteStateMachine.StateTransition("Sid_Punch");
	}
}
using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class IdleState : BaseStateClass {
	//Handles code when entering Idle-State.
    public override void EnterState() { 
		playerReference.characterVelocity = Vector2.Zero;
		playerReference.playerAnimations.Play("Idle"); 
	}

	public override void UpdateState(float delta) {
		if(playerReference.IsOnFloor() && playerReference.characterVelocity.X != 0.0f) {
			finiteStateMachine.StateTransition("Sid_Move"); //Change to Move State.
		} else if(!playerReference.IsOnFloor() && playerReference.characterVelocity.Y <= 0.0f) {
			finiteStateMachine.StateTransition("Sid_Jump"); //Change to Jump State.
		} else if(!playerReference.IsOnFloor() && playerReference.characterVelocity.Y >= 0.0f) {
			finiteStateMachine.StateTransition("Sid_Fall"); //Chagne to Fall State.
		}
	}
}

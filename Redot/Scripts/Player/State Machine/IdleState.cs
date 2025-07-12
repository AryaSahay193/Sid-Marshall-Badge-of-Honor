using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class IdleState : BaseStateClass {
	private BaseStateClass baseState;

    public override void _Ready() { 
		baseState = new BaseStateClass();
	}
	
	//Handles code when entering Idle-State.
    private void EnterState() { 
		characterVelocity = Vector2.Zero;
		baseState.playerAnimations.Play("Idle"); 
	}

	//Handles State-Changes.
	private void PhysicsUpdate(float delta) {
		characterVelocity.Y += playerReference.gravityValue * delta; //Acting gravity force applied.
		playerReference.MoveAndSlide(); //Calls the function so the character can move.

		if(isGrounded && characterVelocity.X != 0.0f) {
			finiteStateMachine.StateTransition("Sid_Move"); //Change to Move State.
		} else if(!isGrounded && characterVelocity.Y <= 0.0f) {
			finiteStateMachine.StateTransition("Sid_Jump"); //Change to Jump State.
		} else if(!isGrounded && characterVelocity.Y >= 0.0f) {
			finiteStateMachine.StateTransition("Sid_Fall"); //Chagne to Fall State.
		}
	}
}

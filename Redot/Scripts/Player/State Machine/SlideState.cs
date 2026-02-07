using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class SlideState : ParentState {
	private float groundSlideSpeed = 188.15f, slideAcceleration = 4.293f, slideFriction = 0.8427f;

	//Handles code when entering the Jump State.
    public override void EnterState() {
		playerController.debugText.Text = "[center]State: Slide[/center]";
		//playerController.Velocity = playerController.Velocity with {X = groundSlideSpeed * inputManager.horizontalButton()};
		playerAnimations.Play("Slide");
		pauseInputOnAnimation();
	}

    public override void ExitState() {
        playerAnimations.Play("Slide_Recover");
		SetProcessInput(true);
    }

	
	public override void UpdateState(float delta) {
		pauseInputOnAnimation();
		if(playerController.IsOnFloor()) {
			if(!inputManager.crouchButton() || playerController.Velocity.X == 0.0f) {
				playerAnimations.Play("Slide_Recover");
				playerAnimations.AnimationFinished += () => finiteStateMachine.StateTransition("IdleState"); 
			} else playerAnimations.Play("Slide_Loop");
		} else finiteStateMachine.StateTransition("FallState");
	}

    public override void PhysicsUpdate(float delta) {
		SetProcessInput(false);
        if(inputManager.crouchButton() && inputManager.horizontalButton() != 0.0f) {
			playerController.Velocity = playerController.Velocity with {X = calculateVelocity(playerController.Velocity.X, 0.0f, slideFriction)};
		}
    }
}
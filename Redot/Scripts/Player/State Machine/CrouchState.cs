using Godot;
using System;

public partial class CrouchState : ParentState {
	[ExportGroup("Collisions")]
	[Export] private Shape2D crouchCollision;
	private float crouchFriction = 1.325f;
	private bool isCrouching = false;

    public override void EnterState() { 
		playerController.debugText.Text = "[center]State: Crouch[/center]";
	}
	
	public override void UpdateState(float delta) {
		if(inputManager.crouchButton()) {
			pauseInputOnAnimation();
			playerAnimations.Play("Crouch"); //Continuosly plays crouch animation.
		} else {
			SetProcessInput(true);
			playerAnimations.Play("Crouch_Recover");
			playerAnimations.AnimationFinished += () => finiteStateMachine.StateTransition("IdleState");
		}
	}

	public override void PhysicsUpdate(float delta) {
		if(playerController.Velocity.X != 0.0f) playerController.Velocity = playerController.Velocity with {X = calculateVelocity(playerController.Velocity.X, 0.0f, crouchFriction)};
		else playerController.Velocity = playerController.Velocity with {X = 0.0f};
	}

	public override void ExitState() => playerAnimations.Play("Crouch_Recover");
}

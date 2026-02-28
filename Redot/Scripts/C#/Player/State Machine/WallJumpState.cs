using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class WallJumpState : ParentState { //Handles code when entering Idle-State.
    private float wallPushback = 343.44f, wallJumpHeight = -227.90f, wallSlideSpeed = 1298.50f, wallSlideAcceleration = 26.50f;
    
    public override void EnterState() {
		playerController.debugText.Text = "[center]State: Wall[/center]";
		playerAnimations.Play("Wall_Contact");
	}
	
	public override void UpdateState(float delta) {
		if(playerController.IsOnWall() && inputManager.horizontalButton() != 0.0f) {
			if(inputManager.jumpButton()) playerAnimations.Play("Wall_Kick");
			else playerAnimations.Play("Wall_Slide");
		} else finiteStateMachine.StateTransition("FallState");
		flipCharacter();
	}

    public override void PhysicsUpdate(float delta) {
		playerController.Velocity = playerController.Velocity with {Y = 0.0f}; //Cancels gravity when holding towards a wall.
		if(inputManager.jumpButton()) playerController.Velocity = new Vector2((wallPushback * -inputManager.horizontalButton()), wallJumpHeight);
		else playerController.Velocity = new Vector2(playerController.Velocity.X, Mathf.MoveToward(playerController.Velocity.Y, wallSlideSpeed, wallSlideAcceleration));
    }
}

using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class PlayerCrouch : BaseState {
	[ExportGroup("Collisions")]
	[Export] private Shape2D normalCollision, crouchCollision;

	private TileMapLayer currentTile;
	private float crouchFriction = 1.325f, normalHeight, crouchHeight, collisionDifference;
	private bool isCrouching = false;

    public override void EnterState() { 
		playerController.debugText.Text = "[center]State: Crouch[/center]";
		// singletonReference.playAnimationOnce(playerAnimations, "Crouch");

		playerAnimations.Play("Crouch");
		playerAnimations.AnimationFinished += () => playerAnimations.Stop();
	}

	public override void PhysicsUpdate(float delta) { // Handles both physics and collision logic
		if(playerController.Velocity.X != 0.0f) playerController.Velocity = playerController.Velocity with {X = Mathf.MoveToward(playerController.Velocity.X, 0.0f, crouchFriction)};
		else playerController.Velocity = playerController.Velocity with {X = 0.0f};

		if(normalCollision is CapsuleShape2D regularCapsule) normalHeight = regularCapsule.Height;
		if(crouchCollision is CapsuleShape2D crouchCapsule) crouchHeight = crouchCapsule.Height;
		collisionDifference = (normalHeight - crouchHeight)/2;

		if(!inputManager.crouchPressed()) {
			playerController.playerCollision.Shape = normalCollision;
			// playerAnimations.Offset = playerAnimations.Offset with {Y = 0.0f};
			ExitState(); 
		} else {
			//if(inputManager.jumpPressed() && currentTile) finiteStateMachine.ChangeToState("Fall"); // Checks for one-way tile collisions
			playerController.playerCollision.Shape = crouchCollision;
			// playerAnimations.Offset = playerAnimations.Offset with {Y = -collisionDifference};
		}
	}

	public override void ExitState() { 
		SetProcessInput(true);
		playerAnimations.Play("Crouch_Recover");
		playerAnimations.AnimationFinished += () => finiteStateMachine.ChangeToState("IdleState");
	}
}

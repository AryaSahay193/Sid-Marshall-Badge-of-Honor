using System;
using System.Collections.Generic;
using Godot;

//This class will not be used directly, it only serves as a base structure for each state for the player. 
//We will be "borrowing" the structure and variables from this class to use in our state logic classes.
//The "public" keyword helps us use all of the variables in this class for our children classes.
public partial class BaseStateClass : Node {
	public const String DeathState = "Sid_Death";
	public const String IdleState = "Sid_Idle";
	public const String MoveState = "Sid_Move";
	public const String JumpState = "Sid_Jump";
	public const String FallState = "Sid_Fall";
	public const String PunchState = "Sid_Punch";
	public const String KickState = "Sid_Kick";

	[Export] public CharacterBody2D playerReference;
	[Export] public StateHandler finiteStateMachine;
	[Export] public AnimatedSprite2D playerAnimations;
	[Export] public CollisionShape2D playerCollider;
	
	public float gravityValue = 312.7f;
	public bool runButton, crouchButton, kickButton, punchButton, jumpButton;
	public bool isGrounded, isWalled, isRoofed, inBattleMode;
	public Vector2 characterVelocity, moveDirection;
    
	//Base method for initializing variables.
    public override void _Ready() {
		moveDirection = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		punchButton = Input.IsActionJustPressed("player_punch");
		kickButton = Input.IsActionJustPressed("player_kick");
		jumpButton = Input.IsActionJustPressed("player_jump");
		crouchButton = Input.IsActionPressed("player_down");
		runButton = Input.IsActionPressed("player_run");

		characterVelocity = playerReference.Velocity;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
		isGrounded = playerReference.IsOnFloor();
		isRoofed = playerReference.IsOnCeiling();
		isWalled = playerReference.IsOnWall();
    }
	
	//Base method that handles button presses.
	//public virtual void HandleInput(InputEvent @event) {}
	
	//Base method that executes code when entering the state.
	public virtual void EnterState() { //All states can play animations upon entering.
		playerAnimations.Play();
	}
	
	//Base method that executes code when exiting the current state.
	public virtual void ExitState() {}

	//Base method that handles logic of the current state.
	public virtual void UpdateState(float delta) {}
	
	//Base method that usually handles physics and time-based calculations rather than frame-based.
	public virtual void PhysicsUpdate(float delta) { //All states are affected by gravity.
		characterVelocity.Y += gravityValue;
		playerReference.MoveAndSlide();
	}

	public void flipCharacter(float direction) {
		if(direction != 0.0f) {
			if(direction < 0.0f) playerAnimations.FlipH = true;
			else if(direction > 0.0f) playerAnimations.FlipH = false;
		}
	}
}
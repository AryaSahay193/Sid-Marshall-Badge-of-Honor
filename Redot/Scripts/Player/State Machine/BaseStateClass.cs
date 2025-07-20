using System;
using System.Collections.Generic;
using Godot;

//This class will not be used directly, it only serves as a base structure for each state for the player. 
//We will be "borrowing" the structure and variables from this class to use in our state logic classes.
//The "public" keyword helps us use all of the variables in this class for our children classes.
public partial class BaseStateClass : Node {
	//State signals and state names.
	[Signal] public delegate void stateFinishedEventHandler(String newState);
	public const String DeathState = "Sid_Death";
	public const String IdleState = "Sid_Idle";
	public const String MoveState = "Sid_Move";
	public const String JumpState = "Sid_Jump";
	public const String FallState = "Sid_Fall";
	public const String PunchState = "Sid_Punch";
	public const String KickState = "Sid_Kick";

	public AnimatedSprite2D playerAnimations;
	public CollisionShape2D playerCollider;
	public StateHandler finiteStateMachine;
	public PlayerScript playerReference;
	
	public bool runButton, crouchButton, kickButton, punchButton, jumpButton;
	public bool isGrounded, isWalled, isRoofed, inBattleMode;
	public float currentVelocity;
    
	//Base method for initializing variables.
    public override void _Ready() {
		finiteStateMachine = new StateHandler();
		playerAnimations = GetNode<AnimatedSprite2D>("Animations");
		playerCollider = GetNode<CollisionShape2D>("Collision");
		playerReference = new PlayerScript(); //Instantiates a new script for references.

		punchButton = Input.IsActionJustPressed("player_punch");
		kickButton = Input.IsActionJustPressed("player_kick");
		jumpButton = Input.IsActionJustPressed("player_jump");
		crouchButton = Input.IsActionPressed("player_down");
		runButton = Input.IsActionPressed("player_run");

		isGrounded = playerReference.IsOnFloor();
		isRoofed = playerReference.IsOnCeiling();
		isWalled = playerReference.IsOnWall();
    }
	
	//Base method that handles button presses.
	public virtual void HandleInput(InputEvent @event) {}
	
	//Base method that executes code when entering the state.
	public virtual void EnterState() {}
	
	//Base method that executes code when exiting the current state.
	public virtual void ExitState() {}

	//Base method that handles logic of the current state.
	public virtual void UpdateState(float delta) {}
	
	//Base method that usually handles physics and time-based calculations rather than frame-based.
	public virtual void PhysicsUpdate(float delta) {}

	public void flipCharacter(float direction) {
		if(direction != 0.0f) {
			if(direction < 0.0f) playerAnimations.FlipH = true;
			else if(direction > 0.0f) playerAnimations.FlipH = false;
		}
	}
}

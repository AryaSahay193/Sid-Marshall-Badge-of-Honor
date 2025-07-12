using System;
using System.Collections.Generic;
using Godot;

//This class will not be used directly, it only serves as a base structure for each state for the player. 
//We will be "borrowing" the structure and variables from this class to use in our state logic classes.
//The "public" keyword helps us use all of the variables in this class for our children classes.
public partial class BaseStateClass : Node {
	public TileMapLayer grassyTerrain, hardFloorTerrain;
	public Vector2 characterVelocity, moveDirection;
	public AnimatedSprite2D playerAnimations;
	public StateHandler finiteStateMachine;
	public CollisionShape2D playerCollider;
	public PlayerScript playerReference;
	
	public bool runButton, crouchButton, kickButton, punchButton, jumpButton;
	public bool isGrounded, isWalled, isRoofed, inBattleMode;
	public float currentVelocity;
    
	//Base method for initializing variables.
    public override void _Ready() {
		moveDirection = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		playerAnimations = GetNode<AnimatedSprite2D>("Animations");
		playerCollider = GetNode<CollisionShape2D>("Collision");
		playerReference = new PlayerScript(); //Instantiates a new script for references.
		characterVelocity = playerReference.Velocity;

		punchButton = Input.IsActionJustPressed("player_punch");
		kickButton = Input.IsActionJustPressed("player_kick");
		jumpButton = Input.IsActionJustPressed("player_jump");
		crouchButton = Input.IsActionPressed("player_down");
		runButton = Input.IsActionPressed("player_run");

		isGrounded = playerReference.IsOnFloor();
		isRoofed = playerReference.IsOnCeiling();
		isWalled = playerReference.IsOnWall();
    }
	
	//Base method that executes code when entering the state.
	public virtual void EnterState() {}
	
	//Base method that executes code when exiting the current state.
	public virtual void ExitState() {}

	//Base method that handles button presses.
	public virtual void HandleInput(InputEvent @event) {}

	//Base method that updates the current state.
	public virtual void FrameUpdate(float delta) {}
	
	//Base method that usually handles time-based calculations rather than frame-based.
	public virtual void PhysicsUpdate(float delta) {}

	public void flipCharacter() {
		if(moveDirection.X != 0.0f) {
			if(moveDirection.X < 0.0f) playerAnimations.FlipH = true;
			else if(moveDirection.X > 0.0f) playerAnimations.FlipH = false;
		}
	}
}

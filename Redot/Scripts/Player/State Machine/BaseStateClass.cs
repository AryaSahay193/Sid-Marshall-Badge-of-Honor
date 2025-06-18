using Godot;
using System;

//This class will not be used directly, it only serves as a base structure for each state for the player. 
//We will be "borrowing" the structure and variables from this class to use in our state logic classes.
//The "public" keyword helps us use all of the variables in this class for our children classes.
public partial class BaseStateClass : Node {
	public TileMapLayer grassyTerrain, hardFloorTerrain;
	public CharacterBody2D playerReference;
	public StateHandler finiteStateMachine;
	public AnimatedSprite2D playerAnimations;
	public Vector2 moveDirection, velocity;
	public CollisionShape2D playerCollider;
	public cameramovement cameraScript;
	
	public bool runButton, crouchButton, kickButton, punchButton, jumpButton;
	public bool isGrounded, isWalled, isRoofed;
	public float currentVelocity, gravityValue = 312.7f;
	public float coyoteCounter = 1.5f;

    //Base method for initializing variables.
    public override void _Ready() {
        moveDirection = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		playerAnimations = GetNode<AnimatedSprite2D>("AnimatedSprites");
		playerCollider = GetNode<CollisionShape2D>("Collision");

		isGrounded = playerReference.IsOnFloor();
		isWalled = playerReference.IsOnWall();
		isRoofed = playerReference.IsOnCeiling();
    }
	
	//Base method that executes code when entering the state.
	public virtual void EnterState() {}
	
	//Base method that executes code when exiting the current state.
	public virtual void ExitState() {}

	//Base method that updates the current state.
	public void Update(float delta) {}

	public virtual void PhysicsUpdate(float delta) {}

	//Base method that handels input.
	public virtual void InputHandler(InputEvent @event) {
		if(isGrounded) {
			moveDirection = Input.GetVector("player_left", "player_right", "player_up", "player_down");
			punchButton = Input.IsActionJustPressed("player_punch");
			kickButton = Input.IsActionJustPressed("player_kick");
			crouchButton = Input.IsActionPressed("player_down");
			runButton = Input.IsActionPressed("player_run");
		} else jumpButton = Input.IsActionJustPressed("player_jump");
	}
}

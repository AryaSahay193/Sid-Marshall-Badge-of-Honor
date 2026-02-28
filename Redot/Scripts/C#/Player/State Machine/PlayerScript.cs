using Godot;
using System;

public partial class PlayerScript : CharacterBody2D {
	public bool runButton, crouchButton, kickButton, punchButton, jumpButton, inBattleMode;
	public float currentHealth, maximumHealth = 100.00f, gravityValue = 312.7f;
	public TileMapLayer grassyTerrain, hardFloorTerrain;
	public Vector2 characterVelocity, moveDirection;
	[Export] public AnimatedSprite2D playerAnimations;
	[Export] public CollisionShape2D playerCollider;
	
	public override void _Ready() {
		moveDirection = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		punchButton = Input.IsActionJustPressed("player_punch");
		kickButton = Input.IsActionJustPressed("player_kick");
		jumpButton = Input.IsActionJustPressed("player_jump");
		crouchButton = Input.IsActionPressed("player_down");
		runButton = Input.IsActionPressed("player_run");
		currentHealth = maximumHealth;
	}
	
	//Function that handles physics and time-related code.
	public override void _PhysicsProcess(double delta) {
		characterVelocity = Velocity;
		characterVelocity.Y += gravityValue;
		MoveAndSlide();
	}

	public void flipCharacter(float direction) {
		if(direction != 0.0f) {
			if(direction < 0.0f) playerAnimations.FlipH = true;
			else if(direction > 0.0f) playerAnimations.FlipH = false;
		}
	}
}

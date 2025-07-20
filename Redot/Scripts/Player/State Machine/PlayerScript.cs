using Godot;
using System;

public partial class PlayerScript : CharacterBody2D {
	public float currentHealth, maximumHealth = 100.00f;
	public TileMapLayer grassyTerrain, hardFloorTerrain;
	public Vector2 characterVelocity, moveDirection;
	
	public bool isGrounded, isWalled, isRoofed;
	public float currentVelocity, gravityValue = 312.7f;
	
	public override void _Ready() {
		moveDirection = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		currentHealth = maximumHealth;
	}

	public override void _PhysicsProcess(double delta) {
		characterVelocity.Y += gravityValue * (float)delta; //Force of Gravity will always be acting on the player.
		characterVelocity = Velocity; //Setting the Velocity Vector2 equal to velocity.
		MoveAndSlide(); //A necessary movement to make movement in Redot engine work.
	}
}

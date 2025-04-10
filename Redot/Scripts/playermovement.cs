using Godot;
using System;

public partial class playermovement : CharacterBody2D {
	private int maximumJumps = 2;
	private int jumpsLeft;
	private float walkingSpeed = 10.0f;
	private float runningSpeed;
	private float acceleration = 2.5f;
	private float friction = 3.0f;
	private float gravityScale = 98.1f;
	private Vector2 axisMovement = Input.GetVector("left", "right", "up", "down");
	private Vector2 characterVelocity;

	public override void _Ready() { //Method for initializing variables.
		jumpsLeft = maximumJumps;
	}

	public override void _PhysicsProcess(double delta) { //Main method for running code, runs every frame.
		characterVelocity = axisMovement * walkingSpeed;
		if(Input.IsActionJustPressed("ui_left")) {
			characterVelocity.X -= walkingSpeed;
		} else if(Input.IsActionJustPressed("ui_right")) {
			characterVelocity.X += walkingSpeed;
		}
		
		characterVelocity.Y += gravityScale * (float)delta;
		MoveAndSlide();
	}
}

using Godot;
using System;

public partial class playermovement : CharacterBody2D {
	private int maximumJumps = 2, jumpsLeft;
	private const float jumpVelocity = -265.0f;
	private const float walkingSpeed = 79.5f, runningSpeed = 159.0f;
	private float acceleration, friction;
	private float wallJumpSpeed, wallSlideSpeed;

    public override void _Ready() {
        jumpsLeft = maximumJumps;
    }

    public override void _PhysicsProcess(double delta) {
		Vector2 direction = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		Vector2 velocity = Velocity;

		//Jump and Gravity.
		if (!IsOnFloor()) velocity += GetGravity() * (float)delta; //Gravity.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) velocity.Y = jumpVelocity; //Jump.

		//Walking movements.
		if (direction != Vector2.Zero) velocity.X = direction.X * walkingSpeed;
		else velocity.X = Mathf.MoveToward(Velocity.X, 0, walkingSpeed);

		Velocity = velocity;
		MoveAndSlide();
	}
}

using Godot;
using System;
using System.Diagnostics;

public partial class playermovement : CharacterBody2D {
	private int maximumJumps = 2, jumpsLeft;
	private float jumpVelocity = -265.0f;
	private float walkingSpeed = 53.0f, runningSpeed = 159.0f;
	private float acceleration = 3.0f, friction = 5.0f, currentSpeed;
	private float wallJumpSpeed, wallSlideSpeed;
	private Vector2 direction, velocity;
	private AnimatedSprite2D playerAnimations;

    public override void _Ready() {
        playerAnimations = GetNode<AnimatedSprite2D>("Animations");
		jumpsLeft = maximumJumps;
    }

    public override void _PhysicsProcess(double delta) {
		direction = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		velocity = Velocity;

		//Jump and Gravity.
		if (!IsOnFloor()) velocity += GetGravity() * (float)delta; //Gravity.
		if (Input.IsActionJustPressed("player_jump") && IsOnFloor()) {
			velocity.Y = jumpVelocity;
			if(direction.Y > 0.0f) playerAnimations.Play("Jump");
			else playerAnimations.Play("Fall");
			if(IsOnFloor()) playerAnimations.Play("Land");
		}

		//Walking movements.
		if(IsOnFloor()) {
			currentSpeed = direction.X * walkingSpeed;
			if(direction != Vector2.Zero) {
				if(Input.IsActionPressed("player_run")) {
					currentSpeed = direction.X * runningSpeed;
					velocity.X = Mathf.MoveToward(velocity.X, currentSpeed, acceleration);
					playerAnimations.Play("Run");
				} else {
					velocity.X = Mathf.MoveToward(velocity.X, currentSpeed, acceleration);
					playerAnimations.Play("Walk");
				}
			} else { 
				velocity.X = Mathf.MoveToward(velocity.X, 0.0f, friction);
				playerAnimations.Play("Idle");
			}
		}

		flipCharacter(direction);
		Velocity = velocity;
		MoveAndSlide();
	}

	private void flipCharacter(Vector2 direction) {
		if(direction.X < 0) playerAnimations.FlipH = true;
		if(direction.X > 0) playerAnimations.FlipH = false;
	}
}

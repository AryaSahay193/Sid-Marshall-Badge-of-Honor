using Godot;
using System;
using System.Diagnostics;

public partial class playermovement : CharacterBody2D {
	private bool isIdle, isMoving, isRunning, isAttacking, isCrouching, isSliding, isDiving = false;
	private float acceleration = 3.0f, friction = 5.0f, currentVelocity;
	private float jumpVelocity = -106.0f, gravityValue = 291.5f;
	private float walkingSpeed = 53.0f, runningSpeed = 159.0f;
	private float wallJumpSpeed, wallSlideSpeed;
	private int maximumJumps = 2, numberOfJumps = 0;
	private CollisionShape2D playerCollider;
	private AnimationPlayer animationPlayer;
	private AnimationTree playerAnimator;
	private Vector2 direction, velocity;
	private Sprite2D playerSprites;
	private Variant blendSpaceAmount = new int();

    public override void _Ready() {
		playerSprites = GetNode<Sprite2D>("Main Sprites");
		animationPlayer = GetNode<AnimationPlayer>("Main Sprites/AnimationTree"); //Grabs the Animation Player reference
        playerAnimator = GetNode<AnimationTree>("Main Sprites/AnimationTree"); //Grabs the Animation Tree reference.
		playerAnimator.Set("parameters/conditions/isMoving", blendSpaceAmount); //Assigns the parameter as a Boolean.
		playerAnimator.Get("parameters/playback");
		playerAnimator.Active = true; //Sets the Animation Tree active at the start.
		numberOfJumps = maximumJumps;
    }

    public override void _PhysicsProcess(double delta) {
		direction = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		velocity = Velocity;

		//Handles animations and movement.
		handlePlayerSprites(direction); //Handles flipping sprites and animations.
		if(IsOnFloor()) {
			performMovement();
			performCrouchandSlide();
		}

		if(!IsOnFloor()) velocity.Y += gravityValue * (float)delta; //Gravity.
		if(velocity.Y < 0.0f) {} //Play "Jump" Animation
		else if(velocity.Y > 0.0f) {
			//Play "Jump - Fall" Animation
			if(IsOnFloor()) {} //Play "Jump - Land" Animation
		}

		//performAttack();
		performJump();
		Velocity = velocity;
		MoveAndSlide();
	}
	
	private void handlePlayerSprites(Vector2 direction) {
		if(direction.X < 0.0f) playerSprites.FlipH = true;
		else if(direction.X > 0.0f) playerSprites.FlipH = false;
		
		if(IsOnFloor()) {
			if(direction.X == 0.0f) {
				isIdle = true;
				isMoving = false;
				isRunning = false;
			} else if(direction.X != 0.0f) {
				isIdle = false;
				isMoving = true;
				isRunning = false;
			} else if(isMoving && Input.IsActionPressed("player_run")) {
				isIdle = false;
				isRunning = true;
			}
		}
	}

	private void performMovement() {
		float runningAcceleration = 6.0f;
		currentVelocity = direction.X * walkingSpeed;
		if(direction != Vector2.Zero) {
			if(Input.IsActionPressed("player_run")) {
				currentVelocity = direction.X * runningSpeed;
				velocity.X = Mathf.MoveToward(velocity.X, currentVelocity, runningAcceleration);
				//Play "Run" Animation
			} else {
				velocity.X = Mathf.MoveToward(velocity.X, currentVelocity, acceleration);
				//Play "Walk" Animation
			}
		} else { 
			velocity.X = Mathf.MoveToward(velocity.X, 0.0f, friction);
			//Play "Skidding" Animation
		}
	}

	private void performJump() {
		if(Input.IsActionJustPressed("player_jump") && numberOfJumps < maximumJumps) {
			velocity.Y = jumpVelocity;
			numberOfJumps += 1;
		} 
		if(IsOnFloor()) numberOfJumps = 0;
		else if(numberOfJumps == 0) numberOfJumps += 1;
	}

	private void performAttack() {
		if(isAttacking) {
			velocity.X = 0.0f;
			//Play "Kick - Normal"
			playerSprites.FlipH = false;
		} 
	}

	private void performCrouchandSlide() {
		if(Input.IsActionPressed("player_down")) {}
	}
}

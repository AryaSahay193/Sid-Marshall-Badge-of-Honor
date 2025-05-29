using Godot;
using System;
using System.Diagnostics;

public partial class playermovement : CharacterBody2D {
	private bool isIdle, isMoving, isRunning, isAirborne, isJumping, isFalling, doubleJump, isAttacking, isCrouching;
	private float currentVelocity, acceleration = 3.0f, friction = 5.0f, airAcceleration = 0.3f;
	private float walkingSpeed = 53.0f, runningSpeed = 159.0f;
	private float jumpVelocity = -132.5f, gravityValue = 312.7f, airVelocity = 25.0f, coyoteCounter = 1.5f, coyoteTime;
	private float wallPushback = 10.0f, wallJumpHeight = 13.0f, wallSlideSpeed;
	private int animationVelocityX, animationVelocityY, maximumJumps = 2, numberOfJumps = 0;
	
	[Export] private AnimationTree playerAnimator;
	[Export] private AnimationPlayer animationPlayer;
	private CollisionShape2D playerCollider;
	private Vector2 direction, velocity;
	private Sprite2D playerSprites;
	private Camera2D camera2D;

    public override void _Ready() {
		playerSprites = GetNode<Sprite2D>("MainSprite");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer"); //Grabs the Animation Player reference
		playerAnimator = GetNode<AnimationTree>("AnimationTree"); //Grabs the Animation Tree reference
		playerAnimator.Active = true; //Always sets Animation Tree to true.
		camera2D = GetNode<Camera2D>("Camera");
		numberOfJumps = maximumJumps;
    }

    public override void _Process(double delta) {
		direction = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		velocity = Velocity;

		//Handles animations and movement.
		handlePlayerAnimations(); //Handles flipping sprites and animations.
		if(IsOnFloor()) {
			performMovement();
			performCrouchandSlide();
			coyoteCounter = coyoteTime;
		}

		//performAttack();
		performJump((float)delta);
		Velocity = velocity;
		MoveAndSlide();
	}
	
	public void handlePlayerAnimations() {
		if(direction.X == 0.0f) animationPlayer.Play("Idle");
		else if(direction.X != 0.0f) {
			if(direction.X < 0.0f) playerSprites.FlipH = true;
			else if(direction.X > 0.0f) playerSprites.FlipH = false;
		}
		
		isAirborne = !IsOnFloor();
		doubleJump = isAirborne && numberOfJumps == 1;
		isIdle = IsOnFloor() && velocity.X == 0.0f;
		isFalling = isAirborne && velocity.Y > 0.0f;
		isMoving = IsOnFloor() && velocity.X != 0.0f;
		isRunning = isMoving && Input.IsActionPressed("player_run");
		isJumping = IsOnFloor() && Input.IsActionJustPressed("player_jump");
		isCrouching = IsOnFloor() && Input.IsActionJustPressed("player_down");
		isAttacking = Input.IsActionJustPressed("player_attack");
		playerAnimator.Set("parameters/conditions/IsGrounded", IsOnFloor() && !IsOnWall());
		playerAnimator.Set("parameters/conditions/IsWalled", IsOnWall() && !IsOnFloor());
		playerAnimator.Set("parameters/conditions/IsAirborne", isAirborne);
		playerAnimator.Set("parameters/conditions/doubleJump", doubleJump);
		playerAnimator.Set("parameters/conditions/IsFalling", isFalling);
		playerAnimator.Set("parameters/conditions/IsIdle", isIdle);
		playerAnimator.Set("parameters/conditions/IsMoving", isMoving);
		playerAnimator.Set("parameters/conditions/IsRunning", isRunning);
		playerAnimator.Set("parameters/conditions/IsAttacking", isAttacking);
		playerAnimator.Set("parameters/conditions/IsCrouching", isCrouching);
	}

	public void performMovement() {
		float runningAcceleration = 6.0f;
		currentVelocity = direction.X * walkingSpeed;
		if(direction != Vector2.Zero) {
			if(Input.IsActionPressed("player_run")) {
				currentVelocity = direction.X * runningSpeed;
				velocity.X = Mathf.MoveToward(velocity.X, currentVelocity, runningAcceleration);
			} else velocity.X = Mathf.MoveToward(velocity.X, currentVelocity, acceleration);
		} else { 
			velocity.X = Mathf.MoveToward(velocity.X, 0.0f, friction);
			//animationPlayer.Play("Skidding");
		}
	}

	public void performJump(float delta) {
		float airBorneSpeed = airVelocity * direction.X;
		if(!IsOnFloor()) {
			coyoteCounter -= delta;
			velocity.Y += gravityValue * (float)delta; //Gravity.
			if(velocity.Y > 0.0f) Mathf.MoveToward(currentVelocity, airBorneSpeed, airAcceleration);
		} if(Input.IsActionJustPressed("player_jump") && numberOfJumps < maximumJumps) {
			velocity.Y = jumpVelocity;
			numberOfJumps += 1;
			//playerAnimator.Set("parameters/Airborne/blend_position", 0);
			//if(!IsOnFloor() && velocity.Y >= 0.0f) playerAnimator.Set("parameters/Airborne/blend_position", 2);
		} 
		if(IsOnFloor()) numberOfJumps = 0;
		else if(numberOfJumps == 0) numberOfJumps += 1;
	}

	public void performWallJump() {
		if(IsOnWall() && direction.X != 0.0f) {
			velocity.X = (wallPushback * -direction.X);
			velocity.Y = wallJumpHeight;
		} 
	}

	public void performAttack() {
		isAttacking = Input.IsActionPressed("player_down") && IsOnFloor();
		if(isAttacking) {
			velocity.X = 0.0f;
			//Play "Kick - Normal"
			playerSprites.FlipH = false;
		} 
	}

	public void performCrouchandSlide() {
		if(Input.IsActionPressed("player_down")) {}
	}
}

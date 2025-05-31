using Godot;
using System;
using System.Diagnostics;

public partial class playermovement : CharacterBody2D {
	private bool isIdle, isMoving, isAttacking, isCrouching;
	private float currentVelocity, acceleration = 3.0f, friction = 5.0f, airAcceleration = 0.3f;
	private float walkingSpeed = 53.0f, runningSpeed = 159.0f, groundSlideSpeed = 3.5f;
	private float jumpVelocity = -132.5f, gravityValue = 312.7f, airVelocity = 25.0f, coyoteCounter = 1.5f, coyoteTime;
	private float wallPushback = 10.0f, wallJumpHeight = 13.0f, wallSlideSpeed;
	private int animationVelocityX, animationVelocityY, maximumJumps = 2, numberOfJumps = 0;
	
	private AnimatedSprite2D playerAnimations;
	private CollisionShape2D playerCollider;
	private Vector2 direction, velocity;
	private cameramovement cameraScript;

    public override void _Ready() {
		playerAnimations = GetNode<AnimatedSprite2D>("AnimatedSprites");
		cameraScript = new cameramovement();
		numberOfJumps = maximumJumps;
    }

	//Main method of the player, reads code for each frame the game runs.
    public override void _Process(double delta) {
		direction = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		velocity = Velocity;

		//Handles animations and movement.
		if(!cameraScript.canMoveCamera) {
			if(IsOnFloor()) {
				coyoteCounter = coyoteTime; //Resets the CoyoteCounter to 0, for jumping.
				performCrouchandSlide(); //Handles the crouching and sliding mechanism.
				performMovement(); //Handles player movement (walk, run, acceleration, friction).
				flipCharacter(); //Handles flipping sprites and animations.
			}

			//performAttack();
			performJump((float)delta); //Handles the jumping mechanism.
			Velocity = velocity; //Setting the Velocity Vector2 equal to velocity.
			MoveAndSlide(); //A necessary movement to make movement in Redot engine work.
		}
	}
	
	public void flipCharacter() {
		if(direction.X != 0.0f) {
			if(direction.X < 0.0f) playerAnimations.FlipH = true;
			else if(direction.X > 0.0f) playerAnimations.FlipH = false;
		}
	}

	public void performMovement() {
		float runningAcceleration = 6.0f;
		currentVelocity = direction.X * walkingSpeed;
		if(direction != Vector2.Zero) {
			if(Input.IsActionPressed("player_run")) {
				currentVelocity = direction.X * runningSpeed;
				velocity.X = Mathf.MoveToward(velocity.X, currentVelocity, runningAcceleration);
				playerAnimations.Play("Run");
			} else { 
				velocity.X = Mathf.MoveToward(velocity.X, currentVelocity, acceleration);
				playerAnimations.Play("Walk");
			}
		} else { 
			velocity.X = Mathf.MoveToward(velocity.X, 0.0f, friction);
			if(velocity.X != 0.0f && (velocity.X < walkingSpeed || velocity.X > -walkingSpeed)) playerAnimations.Play("Skid");
			whenAnimationFinished();
		}
	}

	public void performJump(float delta) {
		float airBorneSpeed = airVelocity * direction.X;
		float airborneCanJump = -30.0f;
		if(!IsOnFloor()) {
			velocity.Y += gravityValue * (float)delta; //Sets the character gravity.
			coyoteCounter -= delta;
			if(velocity.Y > 0.0f) {
				if(direction.X != 0.0f) Mathf.MoveToward(currentVelocity, airBorneSpeed, airAcceleration);
				if(velocity.Y <= airborneCanJump) numberOfJumps = 3;
				playerAnimations.Play("Fall");
			}
		} if(Input.IsActionJustPressed("player_jump") && numberOfJumps < maximumJumps) {
			if(numberOfJumps == 1) playerAnimations.Play("Double-Jump");
			if(numberOfJumps == 0) playerAnimations.Play("Jump");
			whenAnimationFinished();
			velocity.Y = jumpVelocity;
			numberOfJumps += 1;
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
		if(IsOnFloor() && Input.IsActionJustPressed("player_punch")) {
			velocity = new Vector2(0.0f, 0.0f);
			//playerAnimations.Play("Punch");
			//playerAnimations.FlipH = false;
			whenAnimationFinished();
		} 
	}

	public void performCrouchandSlide() {
		if(velocity.X <= walkingSpeed || velocity.X >= -walkingSpeed) { 
			if(Input.IsActionPressed("player_down")) {
				velocity.X = Mathf.MoveToward(velocity.X, 0.0f, friction);
				playerAnimations.Play("Crouch");
			} else if(Input.IsActionJustReleased("player_down")) playerAnimations.Play("Crouch (Recover)");
		} else if(velocity.X == runningSpeed) {
			velocity.X = Mathf.MoveToward(velocity.X, 0.0f, groundSlideSpeed);
			playerAnimations.Play("Slide");
			if(playerAnimations.IsPlaying()) return;
			playerAnimations.Play("Slide (Loop)");
			if(playerAnimations.IsPlaying()) return;
			playerAnimations.Play("Slide (Recover)");
			whenAnimationFinished();
		}
	}

	//Signal Method of AnimatedSprite2D node when playing one animation after another.
	private void whenAnimationChanges() {

	}

	//Signal Method of AnimatedSprite2D node when executing something once the animation is done.
	private void whenAnimationFinished() {
		if(playerAnimations.Animation == "Skid") playerAnimations.Play("Idle");
		if(playerAnimations.IsPlaying()) return;
		playerAnimations.Stop(); //This way the animation does not loop.
		if(!IsOnFloor() && velocity.Y < 0.0f) playerAnimations.Play("Fall");
		if(IsOnFloor() && direction.X == 0.0f) playerAnimations.Play("Idle");
	}
}

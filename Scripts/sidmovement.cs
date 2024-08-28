using Godot;

/*Remember, masks means you detect objects
Layers mean that objects detect you.
*/
public partial class sidmovement : CharacterBody2D {
	//General variables. I just kept everything divisible by 13 just to stay consistent.
	private float walkingSpeed = 70.0f;
	private float runningSpeed = 150.0f;
	private float JumpingSpeed = -300.0f;
	private float slidingSpeed = 20.0f;
	public float gravity = 825.0f;
	private float acceleration = 1.5f;
	private float friction = 5.0f;

	//Action-specific parameters
	private float climbingSpeed = 65.0f;
	private float wallJumpRicochet = 78.0f;
	private float wallSlideSpeed = -52.0f;

	//Counter variables.
	int jumpCounter = 0;
	int maxJumpCount = 2;

	//Timer Variables
	private float Timer = 2.0f;
	private float TimerReset = 1.0f;

	//Boolean Variables
	bool movement;
	bool airborne;
	bool skidCheck;
	bool doubleJumpCheck;
	bool slideCheck;
	bool diveRollCheck;
	bool wallJumpCheck;
	bool wallSlideCheck;
	bool climbCheck;

	//Reference Variables
	public AnimationPlayer animation;
	public Sprite2D sprite;

    public override void _Ready() {
		AddToGroup("Sid Marshall"); //Adds Sid Marshall to Player group.
		animation.Active = true; //Sets the AnimationPlayer to be active when starting the game.
    }
    
	public override void _PhysicsProcess(double delta) {
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector2 velocity = Velocity; //Creating a vector component.
		
		//Flips the sprite if the player presses left
		/*if(direction.X < 0) {
			Sprite2D.Scale.X = (Math.Abs(sprite.Scale.X) * -1);
		}*/

		// Add the gravity.
		if (!IsOnFloor()) {
			//GetNode<AnimatedSprite2D>("AnimatedSprite").Play("Jump Fall");
			velocity.Y += gravity * (float)delta;
			} else if(IsOnFloor()) {
				jumpCounter = 0;
			}

		// Handle Jump.
		/*TO-DO: Add timer logic for double-jump, dive-roll and wall-jump. You cannot interact while the
		action is happening (you can change direction midair for double-jump, however).*/

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		
		//FlipSprites(direction);
		
		//Jumping Mechanism
		if (Input.IsActionJustPressed("ui_jump") && jumpCounter < maxJumpCount) {
			velocity.Y = JumpingSpeed; //Sets the jump-height.
			jumpCounter += 1; //Adds 1 to Jump Counter if you jump once.
		}

		//Walking Mechanism.
		if(direction.X <= 0) {
			velocity.X = direction.X * walkingSpeed;
		} else if(direction.X >= 0) {
			velocity.X = -direction.X * -walkingSpeed;
		} else if(direction == Vector2.Zero) {
			animation.Play("Idle");
			velocity.X = Mathf.MoveToward(Velocity.X, 0, walkingSpeed);
		}

		//Acceleration and Running Mechanism.
		if(velocity.X != 0 && Input.IsActionPressed("ui_right") && Input.IsActionPressed("ui_run")) {
			velocity.X = Mathf.MoveToward(Velocity.X, runningSpeed, acceleration);
		} else if(Input.IsActionPressed("ui_left") && Input.IsActionPressed("ui_run")) {
			velocity.X = Mathf.MoveToward(Velocity.X, -runningSpeed, acceleration);
		}

		//Friction Mechanism
		if(velocity.X == 0 && Input.IsActionJustReleased("ui_left")) {
			velocity.X = Mathf.MoveToward(velocity.X, 0, friction);
		} else if(Input.IsActionJustReleased("ui_right")) {
			velocity.X = Mathf.MoveToward(velocity.X, 0, friction);
		}

		//Skidding Mechanism
		/*if(velocity.X > 0 && Input.IsActionJustReleased("ui_run") && Input.IsActionPressed("ui_left")) {
			velocity.X = Mathf.MoveToward(0, velocity.X, friction);
		} else if(velocity.X < 0 && Input.IsActionJustRelease("ui_run") && Input.IsActionPressed("ui_right")) {
			velocity.X = Mathf.MoveToward(0, -velocity.X, -friction);
		}
		*/

		//Sliding Mechanism.
		/*if(IsOnFloor() && Input.IsActionPressed("ui_run") && Input.IsActionPressed("ui_action")) {
			if(direction == Vector2.Right) {
				velocity.X = Mathf.Lerp(Velocity.X, 0, friction);
				if(Input.IsActionJustReleased("ui_action")) {
					}
				timerCooldown(slideCheck, (float)delta);
		} else if(direction == Vector2.Left) {
			velocity.X = Mathf.Lerp(-Velocity.X, 0, friction);
			}
			
			timerCooldown(slideCheck, (float)delta);
			if(Input.IsActionJustReleased("ui_action")) {
			}
		}*/

		//Dive-Roll Mechanic
		/*float diveRollSpeed = 130.0f;
		if(Input.IsActionJustPressed("ui_left")) {
			if(Input.IsActionJustPressed("ui_left")) {
				velocity.X = Mathf.Lerp(-diveRollSpeed, 0, friction);
			}
			timerCooldown(diveRollCheck, (float)delta);
		} else if(Input.IsActionJustPressed("ui_right")) {
			if(Input.IsActionJustPressed("ui_right")) {
				velocity.X = Mathf.Lerp(diveRollSpeed, 0, -friction);
			}
			timerCooldown(diveRollCheck, (float)delta);
		}*/

		//Wall Jump Mechanic
		/*if(IsOnWall() && Input.IsActionJustPressed("jump")) {
			wallJumpCheck = true;
			if(Input.IsActionPressed("ui_left")) {
				velocity.X = wallJumpRicochet;
				velocity.Y = -JumpingSpeed;
				timerCooldown(wallJumpCheck, (float)delta);
			} else if(Input.IsActionPressed("ui_right")) {
				velocity.X = -wallJumpRicochet;
				velocity.Y = -JumpingSpeed;
				timerCooldown(wallJumpCheck, (float)delta);
			}
		}

		if(IsOnWall() && !IsOnFloor()) {
			wallSlideCheck = true;
			if(Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right")) {
				velocity.Y += wallSlideSpeed * (float)delta;
			}
		} else if(!IsOnWall() && !IsOnFloor()) {
			wallSlideCheck = false;
		}*/
		
		/*Climbing Mechanism
		//We have to add the Ladder-signal for this to work.
		if(body.IsInGroup("Interactible")) {
			if(Input.IsActionPressed("ui_up")) {
				climbCheck = true;
				velocity.Y -= climbingSpeed;
			} else if(Input.IsActionPressed("ui_down")) {
				velocity.Y += climbingSpeed;
			} else if(Input.IsActionJustReleased("ui_up") || Input.IsActionJustReleased("ui_down")) {
				animations.Stop();
			}
		} else {
			climbCheck = false;
		}*/

		//Sliding Mechanisms
		/*while(direction.X != 0 && Input.IsActionPressed("ui_run")) {
			Mathf.Lerp(velocity.X, 0, -slidingSpeed);
		} if(velocity.X == 0) {
			//Play Idle animation
		}*/

		Velocity = velocity;
		MoveAndSlide();
	}
}

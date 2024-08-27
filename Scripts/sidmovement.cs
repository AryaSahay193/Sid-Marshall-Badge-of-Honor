using System.Security.Cryptography.X509Certificates;
using Godot;

public partial class sidmovement : CharacterBody2D {
	//General variables. I just kept everything divisible by 13 just to stay consistent.
	private float walkingSpeed = 65.0f; //05 * 13
	private float runningSpeed = 156.0f; //12 * 13
	private float JumpingSpeed = -260.0f; //-(20 * 13)
	public float gravity = 832.0f; //64 * 13
	private float acceleration = 0.9f; // 0.0070 * 13, rounded
	private float friction = 0.03f; // 0.0023 * 13, rounded

	//Action-specific parameters
	private float climbingSpeed = 65.0f; //In the Y-direction
	private float wallJumpRicochet = 78.0f; //06 * 13
	private float wallSlideSpeed = -52.0f; //-(04 * 13)

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
	public AnimationTree animation;
	public AnimatedSprite2D sprite;

    public override void _Ready() {
		AddToGroup("Sid Marshall"); //Adds Sid Marshall to Player group.
		animation.Active = true; //Sets the AnimatedTree to be active when starting the game.
    }
    
	public override void _PhysicsProcess(double delta) {
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector2 velocity = Velocity; //Creating a vector component.
		//FlipSprites(direction);

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
			velocity.X = Mathf.MoveToward(Velocity.X, 0, walkingSpeed);
		}

		//Running Mechanism.
		if(direction.X >= 0 && Input.IsActionPressed("ui_run")) {
			velocity.X = Mathf.MoveToward(Velocity.X, -runningSpeed, walkingSpeed);
		} else if(direction.X <= 0 && Input.IsActionPressed("ui_run")) {
			velocity.X = Mathf.MoveToward(Velocity.X, runningSpeed, walkingSpeed);
		}

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

		//This code sets parameters for the Animation Tree to handle.
		if(velocity.X == 0) {
			animation.Set("parameters/Movement/blend_position", 0); //Plays the idle animation
		} else if(velocity.X != 0) {
			animation.Set("parameters/Movement/blend_position", velocity.X); //Plays walk or run, depening on Sid's velocity.
			if(direction.X < 0) {
				sprite.FlipH = true; //If the direction of X is less than 0 (aka, moving left), flip the spirtes.
			} else if(direction.X > 0) {
				sprite.FlipH = false; //If the direction of X is greater than 0 (aka, moving right), leave the sprites as is.
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	//Collectibles
	/*public static void CollectibleDetected(Node2D body) {
		if(body.IsInGroup("Collectible")) {
			GetTree().Root.GetNode<StaticBody2D>("Jigsaw Puzzle"); //Gets the Jigsaw node, required for the method OnJigsawEntered();
		}
	}*/

	private void timerCooldown(bool action, float delta) {
		float timer = 0.5f;
		float timerReset = 0.5f;

		if(action) {
			timer -= delta;
			if(timer <= 0) {
				action = false;
				Velocity = new Vector2(0, 0);
			}
		}

		timer = timerReset;
	}
}
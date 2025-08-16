using Godot;
using System;
using System.Diagnostics;

public partial class SidMarshall : CharacterBody2D {
	//Accessible methods for any scripts (public modifier)
	[Signal] public delegate int DamageEventHandler(int health); //Signal for damaging enemies.
	public float walkingSpeed = 53.0f, maximumSpeed = 164.3f, acceleration = 1.325f, friction = 1.113f;
	public float horizontalDirection, currentHealth, maximumHealth = 100.0f;
	public PlayerState currentState = PlayerState.Idle;
	public Vector2 characterVelocity;

	private float coyoteCounter = 0.53f, coyoteTime, wallPushback = 10.0f, wallJumpHeight = 13.0f, groundSlideSpeed = 3.5f, wallSlideSpeed = 135.0f;
	private float jumpVelocity = -132.5f, gravityValue = 312.7f, airVelocity = 25.0f, airAcceleration = 1.25f;
	private int maximumJumps = 2, numberOfJumps = 0;
	private bool inBattleMode = false, previouslyGrounded; //By default.
	
	private AudioStreamPlayer jumpSoundEffect, grassWalkSoundEffect, grassRunSoundEffect;
	private CollisionShape2D normalCollision, crouchCollision;
	private AnimatedSprite2D playerAnimations;
	private CpuParticles2D dustParticles;
    
	public override void _Ready() {
		grassWalkSoundEffect = GetNode<AudioStreamPlayer>("Sounds/SFX_Grass_Walk");
		grassRunSoundEffect = GetNode<AudioStreamPlayer>("Sounds/SFX_Grass_Run");
		jumpSoundEffect = GetNode<AudioStreamPlayer>("Sounds/SFX_Jump");
		playerAnimations = GetNode<AnimatedSprite2D>("Animations");
		dustParticles = GetNode<CpuParticles2D>("DustParticles");
		previouslyGrounded = IsOnFloor(); //Boolean that states if the player was grounded on the last frame.
		currentHealth = maximumHealth;
		numberOfJumps = maximumJumps;
    }

	public enum PlayerState {
		Idle, Move, Jump, Fall, Land, Walled, //Traversal states (movement). 
		Crouched, Dive, Grab, Punch, Kick //Action states (evade, attacks, etc).
	}

	//Main method of the player, reads code for each frame the game runs.
    public override void _Process(double delta) {
		horizontalDirection = Input.GetAxis("player_left", "player_right");
		characterVelocity = Velocity;

		//State Machine with Conditions.
		StateConditions((float)delta);
		switch(currentState) {
			case PlayerState.Idle :
				if(inBattleMode) playerAnimations.Play("Battle_Idle");
				else playerAnimations.Play("Idle");
				dustParticles.Emitting = false;
				break;
			case PlayerState.Move :
				characterMovement();
				break;
			case PlayerState.Jump :
				if(Input.IsActionJustPressed("player_jump")) {
					characterVelocity.Y += jumpVelocity;
					playerAnimations.Play("Jump");
				} break;
			case PlayerState.Fall :
				characterFall();
				break;
			case PlayerState.Land :
				playerAnimations.Play("Land");
				whenAnimationFinished();
				characterVelocity = Vector2.Zero;
				currentState = PlayerState.Idle;
				break;
			case PlayerState.Crouched :
				characterCrouch();
				if(Input.IsActionJustReleased("player_down")) {
					playerAnimations.Play("Crouch_Recover");
					currentState = PlayerState.Idle;
				} break;
			case PlayerState.Walled : 
				characterWallJump();
				break;
		} GD.Print(currentState);
		Velocity = characterVelocity; //Setting the Velocity Vector2 equal to velocity.
		flipCharacter(); //Handles flipping of sprites based on direction pressed.
		MoveAndSlide(); //A necessary method to make movement work in Redot engine.
	}

	public void StateConditions(float delta) { //Method that sets conditions for each state.
		if(IsOnFloor()) {
			//if(!previouslyGrounded) currentState = PlayerState.Land;
			//if(Input.IsActionPressed("player_down")) currentState = PlayerState.Crouched;
			if(horizontalDirection == 0.0f && characterVelocity.X == 0.0f) currentState = PlayerState.Idle;
			else currentState = PlayerState.Move;
			coyoteCounter = coyoteTime; //CoyoteTime = 1.5f.
			numberOfJumps = 0; //Can double-jump.
		} else if(IsOnWallOnly()) {
			if(horizontalDirection != 0.0f) currentState = PlayerState.Walled;
		} else {
			if(characterVelocity.Y < 0.0f) currentState = PlayerState.Jump;
			else currentState = PlayerState.Fall;
			characterVelocity.Y += gravityValue * (float)delta; //Sets the character gravity.	
			dustParticles.Emitting = false;
			coyoteCounter -= (float)delta;
		}
	}

	private void flipCharacter() { //Method that helps to flip the sprite of a character.
		if(horizontalDirection != 0.0f) {
			if(horizontalDirection < 0.0f) playerAnimations.FlipH = true;
			else if(horizontalDirection > 0.0f) playerAnimations.FlipH = false;
		} if(IsOnFloor() && characterVelocity.X != (walkingSpeed * horizontalDirection) || characterVelocity.X != 0.0f) dustParticles.Direction = (characterVelocity * -horizontalDirection);
	}

	private void characterMovement() {
		if(horizontalDirection != 0.0f) {
			grassWalkSoundEffect.Play();
			whenAudioFinished();
			if(Input.IsActionPressed("player_run")) {
				characterVelocity.X = Mathf.MoveToward(characterVelocity.X, maximumSpeed * horizontalDirection, acceleration * 2.0f);
				playerAnimations.Play("Run");
				grassRunSoundEffect.Play();
				dustParticles.Emitting = true;
			} else { 
				characterVelocity.X = Mathf.MoveToward(characterVelocity.X, walkingSpeed * horizontalDirection, acceleration);
				playerAnimations.Play("Walk");
			}
		} else { 
			characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, friction);
			if(characterVelocity.X > walkingSpeed || characterVelocity.X < -walkingSpeed) {
				playerAnimations.Play("Skid");
				whenAnimationFinished();
			}
		}
	}

	private void characterJump() {
		if(Input.IsActionJustPressed("player_jump") && numberOfJumps <= maximumJumps) {
			characterVelocity.Y = jumpVelocity;
			numberOfJumps += 1;
		}
		if(numberOfJumps == 1) { 
			playerAnimations.Play("Jump");
			jumpSoundEffect.Play(); 
		} else if(numberOfJumps == 2) { 
			playerAnimations.Play("Double_Jump");
			jumpSoundEffect.PitchScale *= (valueGenerator() + 1.0f); 
		} whenAnimationFinished();
	}

	private void characterFall() {
		playerAnimations.Play("Fall");
		if(characterVelocity.Y < 0.0f && horizontalDirection != 0.0f) Mathf.MoveToward(characterVelocity.X, (airVelocity * horizontalDirection), airAcceleration);
		else Mathf.MoveToward(characterVelocity.X, 0.0f, airAcceleration);
	}

	private void characterWallJump() {
		characterVelocity.Y = wallSlideSpeed;
		playerAnimations.Play("Wall_Contact");
		if(playerAnimations.IsPlaying()) playerAnimations.FlipH = false;
		whenAnimationFinished();
		if(!IsOnWall() && characterVelocity.Y < 0.0f) currentState = PlayerState.Fall;
	}

	private void characterCrouch() {
		if(characterVelocity.X == maximumSpeed) {
			characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, groundSlideSpeed);
			playerAnimations.Play("Slide");
			whenAnimationFinished();
		} else {
			characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, friction);
			playerAnimations.Play("Crouch");
			if(Input.IsActionJustReleased("player_down")) {
				playerAnimations.Play("Crouch_Recover");
				if(characterVelocity.X != 0.0f) {
					playerAnimations.Play("Skid");
					whenAnimationFinished();
				} currentState = PlayerState.Idle;
			}
		}
	}

	public float valueGenerator() {
		Random randomNumber = new Random();
		float randomNumberInRange = (float)randomNumber.NextDouble();
		return randomNumberInRange;
	}

	//Signal Method of AudioStreamPlayer, executes code for what to do after the audio clip finishes playing.
	private void whenAudioFinished() {
		GetTree().GetNodesInGroup("Ground - Grass");
		if(IsOnFloor() && horizontalDirection != 0.0f) {
			//If Tileset tag is "Ground - Grass," execute these lines of code.
			grassWalkSoundEffect.Play(); //Infinite loop of grass sound effect when walking.
			if(Input.IsActionPressed("player_run")) grassRunSoundEffect.Play(); //Infinite loop of grass sound effect when running.
		}
	}

	//Signal Method for AnimatedSprite2D, when one animation plays the next one will play when it's done.
	private void whenAnimationFinished() {
		String animationName = playerAnimations.Animation; //Shorthand version of the expression.
		if(playerAnimations.IsPlaying()) return; //Lets the animation play (does nothing).
		playerAnimations.Stop(); //This way the animation does not loop.
		if(IsOnFloor()) {
			if(horizontalDirection == 0.0f && characterVelocity.X == 0.0f) playerAnimations.Play("Idle"); //If the player is still or finishes the skid animation, play idle.
			if(animationName == "Skid" && characterVelocity.X != 0.0f) return;
			if(animationName == "Slide") playerAnimations.Play("Slide_Loop");
			else if(animationName == "Slide_Loop") playerAnimations.Play("Slide_Recover");
			else if(animationName == "Slide_Recover") playerAnimations.Play("Idle");
		} else if(IsOnWallOnly()) if(animationName == "Wall_Contact") playerAnimations.Play("Wall_Slide");
		else if(characterVelocity.Y > 0.0f) playerAnimations.Play("Fall");
	}
}

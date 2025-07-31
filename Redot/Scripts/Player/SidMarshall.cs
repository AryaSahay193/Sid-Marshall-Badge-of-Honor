using Godot;
using System;
using System.Diagnostics;

public partial class SidMarshall : CharacterBody2D {
	private float horizontalDirection, walkingSpeed = 53.0f, runningSpeed = 159.0f, groundSlideSpeed = 3.5f, acceleration = 3.0f, friction = 2.5f;
	private float coyoteCounter = 0.53f, coyoteTime, wallPushback = 10.0f, wallJumpHeight = 13.0f, wallSlideSpeed;
	private float jumpVelocity = -132.5f, gravityValue = 312.7f, airVelocity = 25.0f, airAcceleration = 1.25f;
	private int maximumJumps = 2, numberOfJumps = 0;
	private bool inBattleMode = false; //By default.
	
	[Signal] public delegate int DamageEventHandler(int health); //Signal for damaging enemies.
	private AudioStreamPlayer jumpSoundEffect, grassWalkSoundEffect, grassRunSoundEffect;
	private CollisionShape2D normalCollision, crouchCollision;
	private PlayerState currentState = PlayerState.Idle;
	private Vector2 characterVelocity, collisionSize;
	private AnimatedSprite2D playerAnimations;
    
	public override void _Ready() {
		grassWalkSoundEffect = GetNode<AudioStreamPlayer>("Sounds/SFX_Grass_Walk");
		grassRunSoundEffect = GetNode<AudioStreamPlayer>("Sounds/SFX_Grass_Run");
		jumpSoundEffect = GetNode<AudioStreamPlayer>("Sounds/SFX_Jump");
		playerAnimations = GetNode<AnimatedSprite2D>("Animations");
		numberOfJumps = maximumJumps;
    }

	private enum PlayerState {
		Idle, Move, Jump, Fall, Walled, //Traversal states (movement). 
		Crouched, Dive, Grab, Punch, Kick //Action states (evade, attacks, etc).
	}

	//Main method of the player, reads code for each frame the game runs.
    public override void _Process(double delta) {
		horizontalDirection = Input.GetAxis("player_left", "player_right");
		characterVelocity = Velocity;

		//State Machine with Conditions.
		StateHandler((float)delta);
		switch(currentState) {
			case PlayerState.Idle :
				playerAnimations.Play("Idle");
				break;
			case PlayerState.Move :
				performMovement();
				break;
			case PlayerState.Jump :
				performJump();
				break;
			case PlayerState.Fall :
				characterFall();
				break;
			case PlayerState.Crouched :
				performCrouchandSlide();
				if(Input.IsActionJustReleased("player_down")) {
					playerAnimations.Play("Crouch_Recover");
					currentState = PlayerState.Idle;
				} break;
		}
		Velocity = characterVelocity; //Setting the Velocity Vector2 equal to velocity.
		flipCharacter(); //Handles flipping of sprites based on direction pressed.
		MoveAndSlide(); //A necessary method to make movement work in Redot engine.
	}
	
	public void flipCharacter() { //Method that helps to flip the sprite of a character.
		if(horizontalDirection != 0.0f) {
			if(horizontalDirection < 0.0f) playerAnimations.FlipH = true;
			else if(horizontalDirection > 0.0f) playerAnimations.FlipH = false;
		}
	}

	public void StateHandler(float delta) { //Method that sets conditions for each state.
		if(IsOnFloor()) {
			coyoteCounter = coyoteTime; //CoyoteTime = 1.5f.
			numberOfJumps = 0; //Can double-jump.
			if(Input.IsActionPressed("player_down")) currentState = PlayerState.Crouched; 
			if(horizontalDirection == 0.0f && characterVelocity.X == 0.0f) currentState = PlayerState.Idle;
			else currentState = PlayerState.Move;
		} else {
			coyoteCounter -= (float)delta;
			characterVelocity.Y += gravityValue * (float)delta; //Sets the character gravity.
			if(characterVelocity.Y < 0.0f) currentState = PlayerState.Jump;
			if(characterVelocity.Y > 0.0f) currentState = PlayerState.Fall;
			if(IsOnWall()) currentState = PlayerState.Walled;
		}
	}

	public void performMovement() {
		if(horizontalDirection != 0.0f) {
			grassWalkSoundEffect.Play();
			whenAudioFinished();
			if(Input.IsActionPressed("player_run")) {
				characterVelocity.X = Mathf.MoveToward(characterVelocity.X, runningSpeed * horizontalDirection, acceleration * 2.0f);
				playerAnimations.Play("Run");
				grassRunSoundEffect.Play();
			} else { 
				characterVelocity.X = Mathf.MoveToward(characterVelocity.X, walkingSpeed * horizontalDirection, acceleration);
				playerAnimations.Play("Walk");
			}
		} else { 
			characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, friction);
			playerAnimations.Play("Skid");
			whenAnimationFinished();
		}
	}

	public void performJump() {
		if(Input.IsActionJustPressed("player_jump") && numberOfJumps < maximumJumps) {
			characterVelocity.Y = jumpVelocity;
			numberOfJumps += 1;
			if(numberOfJumps == 1) { 
				playerAnimations.Play("Jump");
				jumpSoundEffect.Play(); 
			} else if(numberOfJumps == 2) { 
				playerAnimations.Play("Double_Jump");
				jumpSoundEffect.PitchScale *= valueRandomizer(); 
			} whenAnimationFinished();
		}
	}

	public void characterFall() {
		playerAnimations.Play("Fall");
		if(!IsOnFloor()) {
			if(horizontalDirection != 0.0f) Mathf.MoveToward(characterVelocity.X, (airVelocity * horizontalDirection), airAcceleration);
			else Mathf.MoveToward(characterVelocity.X, 0.0f, airAcceleration);
		} else {
			playerAnimations.Play("Land");
			whenAnimationFinished();
		}
	}

	public float valueRandomizer() {
		Random randomNumber = new Random();
		float randomNumberInRange = (float)randomNumber.NextDouble() + 1.0f;
		return randomNumberInRange;
	}

	public void performCrouchandSlide() {
		if(Input.IsActionPressed("player_down")) {
			if(characterVelocity.X == runningSpeed) {
				characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, groundSlideSpeed);
				playerAnimations.Play("Slide");
				if(playerAnimations.IsPlaying()) return;
				playerAnimations.Play("Slide (Loop)");
				if(playerAnimations.IsPlaying()) return;
				playerAnimations.Play("Slide (Recover)");
				whenAnimationFinished();
			} else {
				characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, friction);
				if(Input.IsActionJustReleased("player_down")) playerAnimations.Play("Crouch (Recover)");
				playerAnimations.Play("Crouch");
			}
		}
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

	//Signal Method of AnimatedSprite2D node when executing something once the animation is done.
	private void whenAnimationFinished() {
		if(playerAnimations.Animation == "Skid") playerAnimations.Play("Idle");
		if(IsOnFloor() && horizontalDirection == 0.0f) playerAnimations.Play("Idle");
		if(!IsOnFloor() && characterVelocity.Y < 0.0f) playerAnimations.Play("Fall");
		if(playerAnimations.IsPlaying()) return;
		playerAnimations.Stop(); //This way the animation does not loop.
	}
}

using Godot;
using System;

//Class that handles player animations, player sounds and particle effects.
public partial class PlayerEffects : Node2D {
	[Export] AudioStreamPlayer jumpSFX, grassWalkSFX;
	[Export] AnimatedSprite2D playerAnimations;
	[Export] CpuParticles2D dustParticles;
	[Export] Timer airBorneTimer;
	private PlayerController playerScript; 
	private GlobalData singletonReference;
	private InputManager inputManager;
	private Vector2 playerVelocity;
	private float playerDirection;

	public override void _Ready() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
		inputManager = GetNode<InputManager>("/root/InputManager");
		playerScript = singletonReference.playerReference;
		playerVelocity = singletonReference.playerVelocity;
	}

	public override void _Process(double delta) {
		Vector2 playerVelocity = playerScript.characterVelocity;
		playerDirection = inputManager.horizontalButton(); //Short-hand reference to player direction.
		flipCharacter();	
		switch(playerScript.currentState) {
			case PlayerState.Idle :
				//if(playerScript.inBattleMode) playerAnimations.Play("Battle_Idle");
				playerAnimations.Play("Idle");
				break;
			case PlayerState.Move :
				if(Input.IsActionPressed("player_run") || playerVelocity.X == playerScript.maximumSpeed) {
					if(playerScript.IsOnFloor() && playerVelocity.X != (playerScript.walkingSpeed * playerDirection) || playerVelocity.X != 0.0f) dustParticles.Direction = (playerVelocity * -playerDirection);
					if(playerVelocity.X > playerScript.walkingSpeed || playerVelocity.X < -playerScript.walkingSpeed) playerAnimations.Play("Skid");
					playerAnimations.Play("Run");
					grassWalkSFX.PitchScale *= (valueGenerator() + 1.0f);
				} else { 
					playerAnimations.Play("Walk");
					grassWalkSFX.Play(); 
				} break;
			case PlayerState.Jump :
				if(playerScript.numberOfJumps == 2) {
					jumpSFX.PitchScale = (valueGenerator() + 1.0f);
					playerAnimations.Play("Double_Jump");
				} else {
					playAnimationOnce("Jump");
					jumpSFX.Play();
				} break;
			case PlayerState.Fall :
				playerAnimations.Play("Fall");
				airBorneTimer.Start();
				break;
			case PlayerState.Land :
				playAnimationOnce("Land");
				break;
			case PlayerState.Crouch :
				if(Input.IsActionJustReleased("player_down")) playAnimationOnce("Crouch_Recover");
				else playAnimationOnce("Crouch");
				break;
			case PlayerState.Slide :
				playAnimationOnce("Slide");
				playerAnimations.Play("Slide_Loop");
				break;
			case PlayerState.Walled :
				if(playerScript.currentState == PlayerState.Fall && inputManager.horizontalButton() == 0.0f) playerAnimations.FlipH = true;
				if(playerScript.wallDetection.IsColliding()) playAnimationOnce("Wall_Contact");
				playerAnimations.Play("Wall_Slide");
				if(playerScript.wallRayDirection == -1.0f) playerAnimations.FlipH = true;
				else playerAnimations.FlipH = false;
				break;
			case PlayerState.WallKick :
				playAnimationOnce("Wall_Kick"); 
				break;
			case PlayerState.Punch :
				if(playerAnimations.IsPlaying() == false) playerScript.currentState = PlayerState.Idle; 
				//else playerAnimations.Play("Punch");
				break;
			case PlayerState.Kick :
				if(playerAnimations.IsPlaying() == false) playerScript.currentState = PlayerState.Idle;
				else playerAnimations.Play("Kick");
				break;
		} 
		playerVelocity = playerScript.Velocity;
		whenAudioFinished();
		whenAnimationFinishes();
	}

	private void flipCharacter() { //Method that helps to flip the sprite of a character.
		if(playerDirection != 0.0f) {
			if(playerDirection < 0.0f) playerAnimations.FlipH = true;
			else playerAnimations.FlipH = false;
		}
	}

	private float valueGenerator() {
		Random randomNumber = new Random();
		float randomNumberInRange = (float)randomNumber.NextDouble();
		return randomNumberInRange;
	}

	//Signal Method of AudioStreamPlayer, executes code for what to do after the audio clip finishes playing.
	private void whenAudioFinished() {
		/*GetTree().GetNodesInGroup("Ground - Grass");
		if(playerScript.IsOnFloor() && playerDirection != 0.0f) {
			//If Tileset tag is "Ground - Grass," execute these lines of code.
			grassWalkSFX.Play(); //Infinite loop of grass sound effect when walking.
			if(Input.IsActionPressed("player_run")) grassRunSFX.Play(); //Infinite loop of grass sound effect when running.
		}*/
	}

	private void playAnimationOnce(String animationName) {
		playerAnimations.Play(animationName);
		if(playerAnimations.IsPlaying()) return; //Lets the animation play (does nothing).
		playerAnimations.Stop(); //This way the animation does not loop.
	}

	//Signal Method for AnimatedSprite2D, when one animation plays the next one will play when it's done.
	private void whenAnimationFinishes() {
		String animationName = playerAnimations.Animation; //Shorthand version of the expression.
		if(playerScript.IsOnFloor()) {
			if(playerDirection == 0.0f && playerVelocity.X == 0.0f) playerAnimations.Play("Idle"); //If the player is still or finishes the skid animation, play idle.
			if(playerScript.currentState == PlayerState.Crouch) {
				if(animationName == "Crouch") playerAnimations.Stop();
				if(animationName == "Crouch_Recover") playerAnimations.Stop();
			} else if(playerScript.currentState == PlayerState.Slide) {
				if(animationName == "Slide") playerAnimations.Play("Slide_Loop");
				else if(animationName == "Slide_Loop") playerAnimations.Play("Slide_Recover");
				else if(animationName == "Slide_Recover") playerAnimations.Play("Idle");
			} 
		} else if(playerScript.IsOnWallOnly()) {
			if(playerScript.currentState == PlayerState.Walled) playerAnimations.Play("Wall_Slide");
			else if(playerScript.currentState == PlayerState.WallKick) playerAnimations.Play("Wall_Kick");
		} //else if(playerVelocity.Y > 0.0f) playerAnimations.Play("Fall");
	}

	//Signal method that handles a special type of animation to be played.
	private void whenAirTimerFinishes() => playerAnimations.Play("Fall_High");
}

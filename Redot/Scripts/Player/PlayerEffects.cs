using Godot;
using System;

//Class that handles player animations, player sounds and particle effects.
public partial class PlayerEffects : Node2D {
	[ExportGroup("Audio")]
	[Export] AudioStreamPlayer jumpSFX, grassWalkSFX;
	
	[ExportGroup("Animations")]
	[Export] AnimatedSprite2D playerAnimations;
	
	[ExportGroup("Effects")]
	[Export] CpuParticles2D dustParticles;
	
	[ExportGroup("References")]
	[Export] private PlayerController playerScript;
	[Export] Timer airBorneTimer;

	private SceneManager sceneManager; 
	private GlobalData singletonReference;
	private InputManager inputManager;
	private Vector2 playerVelocity;
	private float playerDirection;
	private Tween colorChange;

	public override void _Ready() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
		inputManager = GetNode<InputManager>("/root/InputManager");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");
		playerVelocity = singletonReference.playerVelocity;
	}

	public override void _Process(double delta) {
		Vector2 playerVelocity = playerScript.characterVelocity;
		playerDirection = inputManager.horizontalButton(); //Short-hand reference to player direction.
		flipCharacter();	
		switch(playerScript.currentState) {
			case PlayerState.Idle :
				if(playerScript.isInBattleMode) playerAnimations.Play("Battle_Idle");
				else playerAnimations.Play("Idle");
				break;
			case PlayerState.Move :
				if((playerDirection == 0.0f || playerDirection == -playerDirection) && playerVelocity.X != 0.0f) playerAnimations.Play("Skid");
				if(Input.IsActionPressed("player_run") || (playerVelocity.X > playerScript.walkingSpeed || playerVelocity.X < -playerScript.walkingSpeed)) {
					grassWalkSFX.PitchScale *= (singletonReference.randomDecimal() + 1.0f);
					dustParticles.Direction = (playerVelocity * -playerDirection);
					playerAnimations.Play("Run"); 
				} else { 
					playerAnimations.Play("Walk");
					grassWalkSFX.Play(); 
				} break;
			case PlayerState.Jump :
				if(inputManager.jumpButton() && playerScript.characterVelocity.Y < 0.0f && playerScript.numberOfJumps < playerScript.maximumJumps) jumpSFX.Play();
				if(playerScript.numberOfJumps == 2) {
					jumpSFX.PitchScale = (singletonReference.randomDecimal() + 1.0f);
					playerAnimations.Play("Double_Jump");
				} else playAnimationOnce("Jump");
				break;
			case PlayerState.Fall :
				playerAnimations.Play("Fall");
				airBorneTimer.Start();
				//airBorneTimer.Timeout += timerFinished;
				break;
			case PlayerState.Land :
				if(playerScript.IsOnFloorOnly()) {
					playAnimationOnce("Land");
					if(!playerAnimations.IsPlaying()) playerScript.currentState = PlayerState.Idle;
				} break;
			case PlayerState.Crouch :
				if(!inputManager.crouchButton()) playAnimationOnce("Crouch_Recover");
				else playAnimationOnce("Crouch");
				break;
			case PlayerState.Slide :
				if(playerScript.GetRealVelocity().Y < 0.0f && playerScript.GetFloorAngle() > 0.0f) playerAnimations.FlipH = true;
				else playerAnimations.FlipH = false;
				if(playerVelocity.X > 0.0f) playerAnimations.FlipH = false;
				else playerAnimations.FlipH = true;
				playAnimationOnce("Slide");
				if(inputManager.crouchButton()) playerAnimations.Play("Slide_Loop");
				else if(!inputManager.crouchButton() || playerVelocity.X == 0.0f) {
					playAnimationOnce("Slide_Recover");
					playerScript.currentState = PlayerState.Idle;
				} break;
			case PlayerState.Walled :
				if(playerScript.currentState == PlayerState.Fall && inputManager.horizontalButton() == 0.0f) playerAnimations.FlipH = true;
				if(playerScript.wallDetection.IsColliding()) playAnimationOnce("Wall_Contact");
				playerAnimations.Play("Wall_Slide");
				if(playerScript.wallRayDirection == -1.0f) playerAnimations.FlipH = true;
				else playerAnimations.FlipH = false;
				break;
			case PlayerState.WallKick :
				playAnimationOnce("Wall_Kick");
				playerDirection = -playerDirection;
				if(playerScript.IsOnFloor()) {
					playerScript.currentState = PlayerState.Idle;
					//playerAnimations.FlipH = true;
				}
				break;
			case PlayerState.Door :
				playAnimationOnce("Door_Enter_Push");
				colorChange.TweenProperty(playerAnimations, "modulate", new Color("#2d1e2f"), 2.0f);
				//playerAnimations.AnimationFinished += () => sceneManager.transitionToScene(); //Change scene when animation finishes.
				break;
			case PlayerState.Punch :
				if(playerAnimations.IsPlaying() == false) playerScript.currentState = PlayerState.Idle; 
				else playerAnimations.Play("Punch");
				break;
			case PlayerState.Kick :
				if(playerAnimations.IsPlaying() == false) playerScript.currentState = PlayerState.Idle;
				else playerAnimations.Play("Kick");
				break;
			case PlayerState.Grab :
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
		} if(playerVelocity.X != 0.0f && playerDirection != -playerDirection) {
			dustParticles.Direction = (playerVelocity * -playerDirection);
			playerAnimations.Play("Skid");
		}
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
		bool animationPlaying = playerAnimations.IsPlaying();
		if(animationPlaying) {
			playerAnimations.Play(animationName);
			animationPlaying = !animationPlaying;
		}
		/*float frameNumber = playerAnimations.SpriteFrames.GetFrameCount(animationName);
		playerAnimations.Play(animationName);
		if(playerAnimations.IsPlaying()) return; //Lets the animation play (does nothing).
		if(playerAnimations.Frame == frameNumber) playerAnimations.Stop(); //This way the animation does not loop.*/
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
	//private void timerFinished() => playerAnimations.Play("Fall_High");
}

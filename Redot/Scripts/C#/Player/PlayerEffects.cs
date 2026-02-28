using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

//Class that handles player animations, player sounds and particle effects.
public partial class PlayerEffects : Node2D {
	[ExportGroup("Audio")]
	[Export] AudioStreamPlayer jumpSFX, grassWalkSFX;
	
	[ExportGroup("Animations")]
	[Export] public AnimatedSprite2D playerAnimations;
	
	[ExportGroup("Effects")]
	[Export] CpuParticles2D dustParticles;

	[ExportGroup("Raycasts")]
	[Export] private RayCast2D groundDetection;
	
	[ExportGroup("References")]
	[Export] private PlayerController playerScript;

	public event Action readyToChangeScene;
	private DoorScript doorScript;
	private GlobalData singletonReference;
	private InputManager inputManager;
	private SceneManager sceneManager; 
	private Vector2 playerVelocity;
	private Tween colorChange;
	private float playerDirection;
	private bool isCrouching;

	public override void _Ready() {
		singletonReference = GetNode<GlobalData>("/root/GlobalData");
		inputManager = GetNode<InputManager>("/root/InputManager");
		sceneManager = GetNode<SceneManager>("/root/SceneManager");
		playerVelocity = singletonReference.playerVelocity; //Shorthand reference.
		doorScript = singletonReference.doorScript;
		//singletonReference.playerEffects = this;
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
				if(playerDirection == 0.0f && playerVelocity.X != playerScript.maximumSpeed * playerDirection) playAnimationOnce("Skid");
				if(Input.IsActionPressed("player_run") || (playerVelocity.X > playerScript.walkingSpeed || playerVelocity.X < -playerScript.walkingSpeed)) {
					grassWalkSFX.PitchScale *= (singletonReference.randomDecimal() + 1.0f);
					dustParticles.Direction = (playerVelocity * -playerDirection);
					playerAnimations.Play("Run"); 
				} else { 
					playerAnimations.Play("Walk");
					grassWalkSFX.Play(); 
				} break;
			case PlayerState.Jump :
				if(playerScript.numberOfJumps == 2) {
					jumpSFX.PitchScale = (singletonReference.randomDecimal() + 1.0f);
					playerAnimations.Play("Double_Jump");
				} else {
					playAnimationOnce("Jump");
					jumpSFX.Play();
				} break;
			case PlayerState.Fall :
				playerAnimations.Play("Fall");
				//airBorneTimer.Start();
				//airBorneTimer.Timeout += () => playerAnimations.Play("Fall_High");
				break;
			case PlayerState.Land :
				if(groundDetection.IsColliding() && playerScript.IsOnFloorOnly()) {
					playAnimationOnce("Land");
					if(playerAnimations.IsPlaying()) playerDirection = 0.0f;
					pauseInputOnAnimation("Land");
				} break;
			case PlayerState.Crouch :
				if(inputManager.crouchButton()) isCrouching = true;
				if(!isCrouching) {
					playAnimationOnce("Crouch_Recover");
					playerAnimations.AnimationFinished += () => playerScript.currentState = PlayerState.Idle;
				} else playAnimationOnce("Crouch");
				break;
			case PlayerState.Slide :
				if(playerScript.GetRealVelocity().Y < 0.0f && playerScript.GetFloorAngle() > 0.0f || playerVelocity.X < 0.0f) playerAnimations.FlipH = true;
				else playerAnimations.FlipH = false;	
				if(inputManager.crouchButton()) { 
					playAnimationOnce("Slide");
					playerAnimations.Play("Slide_Loop");
					if(!inputManager.crouchButton() || playerVelocity.X == 0.0f) {
						playAnimationOnce("Slide_Recover");
						playerAnimations.AnimationFinished += () => playerScript.currentState = PlayerState.Idle;
						playerDirection = 0.0f;
					}
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
				break;
			case PlayerState.Door :
				playerAnimations.Play("Door_Open");
				playerAnimations.AnimationFinished += enterDoor;
				break;
			/*case PlayerState.Punch :
				playerAnimations.Play("Punch");
				playerAnimations.AnimationFinished += () => playerScript.currentState = PlayerState.Idle;
				break;
			case PlayerState.Kick :
				playerAnimations.Play("Kick"); 
				playerAnimations.AnimationFinished += () => playerScript.currentState = PlayerState.Idle;
				break;*/
			case PlayerState.Grab :
				break;
		} playerVelocity = playerScript.Velocity;
		//doorScript.sidOpenDoor += () => playerScript.currentState = PlayerState.Door;
	}

	private void enterDoor() {
		colorChange = GetTree().CreateTween();
		playerAnimations.Play("Door_Enter_Push");
		colorChange.TweenProperty(playerAnimations, "modulate", new Color("#2d1e2f"), 2.0f);
		playerAnimations.AnimationFinished += () => readyToChangeScene?.Invoke(); //Emits signal when animation finishes
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

	private void playAnimationOnce(String animationName) {
		bool animationPlaying = playerAnimations.IsPlaying();
		if(animationPlaying) {
			playerAnimations.Play(animationName);
			animationPlaying = !animationPlaying;
		}
	}

	private void pauseInputOnAnimation(String animationName) {
		if(playerAnimations.IsPlaying()) {
			//playerScript.ProcessMode = Node.ProcessModeEnum = false;
			playerScript.SetPhysicsProcess(false);
			playerDirection = 0.0f;
		} else {
			playerAnimations.AnimationFinished += () => playerScript.SetPhysicsProcess(true);
			playerVelocity = Vector2.Zero;
		}
	}
}

using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
 
public enum PlayerState { //Character States
        Idle, Move, Jump, Fall, Land, Walled, WallKick, Door, //Traversal states (movement). 
        Crouch, Slide, Dive, Grab, Punch, Kick //Action states (evade, attacks, etc).
}

public partial class PlayerController : CharacterBody2D {
    public Action<int> DamageToEnemy; //Signal sent for when the player damages enemies.
    public Action<float> StaminaDeplete; //Signal sent for stamina-value changes.
    public event Action readyToChangeScene; //Signal emits when animation finishes, to change scene.
    
    [ExportGroup("Debug")]
    [Export] private RichTextLabel statePrint;

    [ExportGroup("Animations")]
	[Export] public AnimatedSprite2D playerAnimations;
    
    [ExportGroup("Audio")]
	[Export] AudioStreamPlayer jumpSFX, grassWalkSFX;

    [ExportGroup("Collisions")] //For organization purposes in the game engine.
    [Export] private CollisionShape2D playerCollision; //Regular player collision.
    [Export] private Shape2D crouchCollision, normalCollision; //Collisions used for crouching.

    [ExportGroup("Effects")]
	[Export] CpuParticles2D dustParticles;
    
    [ExportGroup("Raycasts")] //For organization purposes in the game engine.
    [Export] private RayCast2D groundDetection, ceilingDetection; //GroundDetection used for Landing state.
    [Export] public RayCast2D wallDetection; //Raycast used for wall-jump direction.
    
    [ExportGroup("Timers")] //For organization purposes in the game engine.
    [Export] private Timer wallJumpTimer, airborneTimer;
    
    public float wallRayDirection, walkingSpeed = 53.0f, maximumSpeed = 164.3f, gravityValue = 186.03f;
    public float horizontalDirection, currentHealth, maximumHealth = 100.0f;
    public int maximumJumps = 2, numberOfJumps = 0;
    public bool isInBattleMode = false, isCrouching = false;
    public PlayerState currentState = PlayerState.Idle, previousState;
    public Vector2 characterVelocity;

    private float jumpVelocity = -119.25f, horizontalJumpVelocity = 100.70f, airVelocity = 87.45f;
    private float acceleration = 2.65f, airAcceleration = 2.915f, airFriction = 1.272f, friction = 3.180f;
    private float coyoteCounter = 0.53f, coyoteTime, slideFriction = 2.915f, floorAngle, maxFloorAngle;
    private float wallPushback = 171.72f, wallJumpHeight = -187.09f, wallSlideSpeed = 100.17f;
    private float staminaValue = 0.5f;
    private EnemyBaseClass enemyBlueprint;
    private GlobalData singletonReference;
    private CameraScript cameraReference;
    private InputManager inputManager;
    private SceneManager sceneManager;
    private DoorScript doorScript;

    public override void _Ready() {
        enemyBlueprint = GetNode<EnemyBaseClass>("res://Scripts/Enemies/EnemyBaseClass.cs");
        singletonReference = GetNode<GlobalData>("/root/GlobalData");
        inputManager = GetNode<InputManager>("/root/InputManager"); 
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        doorScript = singletonReference.doorScript;
        cameraReference = singletonReference.playerCamera;
        isInBattleMode = singletonReference.isInBattleMode;
        singletonReference.playerController = this; 
        wallRayDirection = wallDetection.Scale.X;
        currentHealth = maximumHealth;
        maxFloorAngle = FloorMaxAngle;
        numberOfJumps = maximumJumps;
    }

    public override void _Process(double delta) {
        flipSprite();
        switch(currentState) {
            case PlayerState.Idle :
                if(isInBattleMode) playerAnimations.Play("Battle_Idle");
				else playerAnimations.Play("Idle");
                break;
            case PlayerState.Move :
                if(horizontalDirection == 0.0f && characterVelocity.X != maximumSpeed && characterVelocity.X != walkingSpeed) playAnimationOnce("Skid");
				if(inputManager.runButton() || (characterVelocity.X > walkingSpeed || characterVelocity.X < -walkingSpeed)) {
					//grassWalkSFX.PitchScale *= (singletonReference.randomDecimal() + 1.0f);
					//dustParticles.Direction = (characterVelocity * -horizontalDirection);
					playerAnimations.Play("Run");
                } else { 
					playerAnimations.Play("Walk");
					grassWalkSFX.Play(); 
				} break;
            case PlayerState.Jump :
                if(numberOfJumps == 2) {
					jumpSFX.PitchScale = (singletonReference.randomDecimal() + 1.0f);
					playerAnimations.Play("Double_Jump");
				} else {
					playAnimationOnce("Jump");
					jumpSFX.Play();
				} break;
            case PlayerState.Fall :
                playerAnimations.Play("Fall");
                break;
            case PlayerState.Land :
                if(groundDetection.IsColliding() && IsOnFloorOnly()) {
					playAnimationOnce("Land");
					if(playerAnimations.IsPlaying()) horizontalDirection = 0.0f;
					pauseInputOnAnimation("Land");
				} break;
            case PlayerState.Crouch :
                if(inputManager.crouchButton()) isCrouching = true;
				if(!isCrouching) {
					playAnimationOnce("Crouch_Recover");
					playerAnimations.AnimationFinished += () => currentState = PlayerState.Idle;
				} else playAnimationOnce("Crouch");
                break;
            case PlayerState.Slide :
                if(GetRealVelocity().Y < 0.0f && GetFloorAngle() > 0.0f || characterVelocity.X < 0.0f) playerAnimations.FlipH = true;
				else playerAnimations.FlipH = false;	
				if(inputManager.crouchButton()) { 
					playAnimationOnce("Slide");
					playerAnimations.Play("Slide_Loop");
					if(!inputManager.crouchButton() || characterVelocity.X == 0.0f) {
						playAnimationOnce("Slide_Recover");
						playerAnimations.AnimationFinished += () => currentState = PlayerState.Idle;
						horizontalDirection = 0.0f;
					}
				} break;
            case PlayerState.Walled :
                if(currentState == PlayerState.Fall && inputManager.horizontalButton() == 0.0f) playerAnimations.FlipH = true;
				if(wallDetection.IsColliding()) playAnimationOnce("Wall_Contact");
				playerAnimations.Play("Wall_Slide");
				if(wallRayDirection == -1.0f) playerAnimations.FlipH = true;
				else playerAnimations.FlipH = false;
                break;
            case PlayerState.WallKick :
                playAnimationOnce("Wall_Kick");
                break;
            case PlayerState.Door :
                playerAnimations.Play("Door_Open");
                playerAnimations.AnimationFinished += () => playerAnimations.Play("Door_Enter_Push");
                Tween colorChange = GetTree().CreateTween();
		        colorChange.TweenProperty(playerAnimations, "modulate", new Color("#2d1e2f"), 2.0f);
		        playerAnimations.AnimationFinished += () => readyToChangeScene?.Invoke(); //Emits signal when animation finishes
                break;
        }
    }

    //Main method of the player, reads code for each frame the game runs.
    public override void _PhysicsProcess(double delta) {
        horizontalDirection = inputManager.horizontalButton(); //Axis type, assigns X-axis controls (Left and Right).
        characterVelocity = Velocity;

        Stack<PlayerState> stateHistory = new Stack<PlayerState>();
        PlayerState originalState = currentState;
        if(currentState != null) {
            stateHistory.Push(originalState);
            while(currentState != originalState) {
                stateHistory.Push(currentState);
            } previousState = stateHistory.Pop() - 1;
        }
        /*if(previousState.ToString() == "-1") GD.Print(PlayerState.Idle);
        else GD.Print(previousState);*/
        
        //State Machine with Conditions.
        StateConditions((float)delta);
        switch(currentState) { //Movement State Machine.
            case PlayerState.Idle :
                StaminaDeplete?.Invoke(staminaValue);
                if(inputManager.jumpButton() && characterVelocity.Y <= 0.0f) currentState = PlayerState.Jump;
                break;
            case PlayerState.Move :
                characterMovement(walkingSpeed, acceleration, friction);
                break;
            case PlayerState.Jump :
                if(inputManager.jumpButton() && numberOfJumps < maximumJumps) { 
                    characterVelocity = new Vector2(characterVelocity.X, jumpVelocity);
                    numberOfJumps += 1;
                } break;
            case PlayerState.Fall :
                float airMoveParameter = -1000.0f;
                characterVelocity = new Vector2(characterVelocity.X, characterVelocity.Y += gravityValue * (float)delta);
                if(characterVelocity.Y >= airMoveParameter) {
                    if(airborneTimer.WaitTime - airborneTimer.TimeLeft <= 2.0f && groundDetection.IsColliding() && !IsOnWallOnly()) currentState = PlayerState.Land;
                    else characterMovement(airVelocity, airAcceleration, airFriction);
                } break;
            case PlayerState.Land :
                if(IsOnFloor()) characterVelocity = new Vector2(0.0f, 0.0f);
                break;
            case PlayerState.Crouch :
                if(FloorMaxAngle > 0.0f) currentState = PlayerState.Idle;
                else characterFriction(slideFriction);
                break;
            case PlayerState.Slide :
                if(FloorMaxAngle > 0.0f) currentState = PlayerState.Idle;
                else characterFriction((slideFriction/2));
                break;
            case PlayerState.Walled : 
                characterVelocity.Y = Mathf.MoveToward(characterVelocity.Y, wallSlideSpeed, acceleration);
                if(!wallDetection.IsColliding() || horizontalDirection == 0.0f) currentState = PlayerState.Fall;
                //if(inputManager.jumpButton()) currentState = PlayerState.WallKick;
                break;
            case PlayerState.WallKick :
                characterVelocity = new Vector2((wallPushback * -horizontalDirection), wallJumpHeight);
                break;
            case PlayerState.Door : //Activated state from the door script.
                //GlobalPosition = sceneManager.characterDoorSpawn();
                SetPhysicsProcess(false);
                break;
        }
        Velocity = characterVelocity; //Setting the Velocity Vector2 equal to velocity.
        flipCharacter(); //Flips the player (Raycasts, animations will be flipped in Animation script).
        MoveAndSlide(); //A necessary method to make movement work in Redot engine.
        statePrint.Text = "[center]State: " + currentState.ToString() + "[/center]";
    }
    
    public void StateConditions(float delta) { //Method that sets conditions for each state.
        //enemyBlueprint.InitiateBattle += (isInBattleMode) => isInBattleMode = true;
        if(IsOnWallOnly()) {
            if(wallDetection.IsColliding() && horizontalDirection != 0.0f) {
                if(inputManager.jumpButton()) currentState = PlayerState.WallKick;
                else currentState = PlayerState.Walled;
            }
        } else {
            if(IsOnFloorOnly()) {    
                //if(currentState != PlayerState.Jump) characterVelocity.Y = 0.0f; //Resets the gravity. 
                coyoteCounter = coyoteTime; //CoyoteTime = 1.5f.
                FloorStopOnSlope = true; //Keeps the character on the slope without sliding.
                FloorSnapLength = 10; //Allows the player to stick to the floor.
                numberOfJumps = 0; //Resets the counter.
                airborneTimer.Stop(); //Stops timer when on the ground.
    
                //Movement logic.
                if(horizontalDirection == 0.0f && characterVelocity.X == 0.0f) currentState = PlayerState.Idle;
                else currentState = PlayerState.Move;
                
                //Crouch and Slide logic.
                if(!inputManager.crouchButton()) {
                    isCrouching = false;
                    playerCollision.Shape = normalCollision;
                    playerCollision.Position = new Vector2(0, 1);
                } else if(inputManager.crouchButton() && FloorMaxAngle != 0.0f) {
                    isCrouching = true;
                    characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, slideFriction);
                    playerCollision.Shape = crouchCollision;
                    playerCollision.Position = new Vector2(0, 9);
                } if(isCrouching) {
                    horizontalDirection = 0.0f;
                    if(characterVelocity.X <= walkingSpeed && characterVelocity.X >= -walkingSpeed) currentState = PlayerState.Crouch;
                    else currentState = PlayerState.Slide;
                    FloorStopOnSlope = false;
                    FloorMaxAngle = 0.0f; //Allows players to slide only on flat or negative slope surfaces.
                } else FloorMaxAngle = maxFloorAngle;
            } else {
                coyoteCounter -= (float)delta;
                characterVelocity.Y += gravityValue * (float)delta; //Sets the character gravity.
                if(characterVelocity.Y > 0.0f) currentState = PlayerState.Fall;
                else airborneTimer.Start();

                //Attack logic
                if(isInBattleMode) {
                    if(inputManager.punchButton()) currentState = PlayerState.Punch;
                    else if(inputManager.kickButton()) currentState = PlayerState.Kick;
                    else if(inputManager.grabButton()) currentState = PlayerState.Grab;
                }
            } if(inputManager.jumpButton() && characterVelocity.Y <= 0.0f) currentState = PlayerState.Jump;
        }
    }
 
    private void flipCharacter() {
        if(horizontalDirection != 0.0f) {
            if(horizontalDirection > 0.0f) {
                wallRayDirection = 1.0f;
                wallDetection.Scale = new Vector2(wallRayDirection, wallDetection.Scale.Y);
            } else {
                wallRayDirection = -1.0f;
                wallDetection.Scale = new Vector2(wallRayDirection, wallDetection.Scale.Y);
            }
        }
    }
    
    private void flipSprite() { //Method that helps to flip the sprite of a character.
		if(horizontalDirection != 0.0f) {
			if(horizontalDirection < 0.0f) playerAnimations.FlipH = true;
			else playerAnimations.FlipH = false;
		} /*if(playerVelocity.X != 0.0f && playerDirection != -playerDirection) {
			dustParticles.Direction = (playerVelocity * -playerDirection);
			playerAnimations.Play("Skid");
		}*/
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
			SetPhysicsProcess(false);
			horizontalDirection = 0.0f;
		} else {
			playerAnimations.AnimationFinished += () => SetPhysicsProcess(true);
			characterVelocity = Vector2.Zero;
		}
	}

    private void characterFriction(float decrementValue) { 
        if(IsOnFloor() && horizontalDirection == 0.0f) characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, decrementValue); //Player-friction code.
    }

    //Both in-air and on the ground.
    private void characterMovement(float endingSpeed, float incrementValue, float decrementValue) {
        if(horizontalDirection != 0.0f) {
            if(IsOnFloorOnly() && inputManager.runButton()) {
                characterVelocity.X = Mathf.MoveToward(characterVelocity.X, maximumSpeed * horizontalDirection, acceleration * 2.0f); //Run-movement code;
            } else characterVelocity.X = Mathf.MoveToward(characterVelocity.X, endingSpeed * horizontalDirection, incrementValue); //Walk-movement code.
        } else characterFriction(decrementValue); //Player-friction code.
    }
}

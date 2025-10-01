using Godot;
using System;
using System.Diagnostics;
 
public enum PlayerState { //Character States
        Idle, Move, Jump, Fall, Land, Walled, WallKick, //Traversal states (movement). 
        Crouch, Slide, Dive, Grab, Punch, Kick //Action states (evade, attacks, etc).
}

public partial class PlayerController : CharacterBody2D {
    [Signal] public delegate int DamageDealtEventHandler(int health); //Signal sent for when the player damages enemies.
    public float wallRayDirection, walkingSpeed = 53.0f, maximumSpeed = 164.3f;
    public float horizontalDirection, currentHealth, maximumHealth = 100.0f;
    public int maximumJumps = 2, numberOfJumps = 0;
    public PlayerState currentState = PlayerState.Idle;
    public Vector2 characterVelocity;
    public RayCast2D wallDetection;

    private bool isInBattleMode, isCrouching = false; //By default.
    private float jumpVelocity = -121.90f, horizontalJumpVelocity = 100.70f, gravityValue = 186.03f, airVelocity = 87.45f;
    private float acceleration = 2.65f, airAcceleration = 2.915f, airFriction = 1.272f, friction = 3.180f;
    private float wallPushback = 171.72f, wallJumpHeight = -187.09f, wallSlideSpeed = 100.17f;
    private float coyoteCounter = 0.53f, coyoteTime, slideFriction = 2.173f, floorAngle;
    [Export] private Shape2D crouchCollision, normalCollision;
    private RayCast2D groundDetection, ceilingDetection;
    private CollisionShape2D playerCollision;
    private Timer wallJumpTimer, landTimer;
    private InputManager inputManager; 

    public override void _Ready() {
        inputManager = GetNode<InputManager>("/root/InputManager");
        playerCollision = GetNode<CollisionShape2D>("PlayerCollision");
        groundDetection = GetNode<RayCast2D>("GroundDetection"); //Raycast used for the Landing state.
        wallDetection = GetNode<RayCast2D>("WallDetection"); //Raycast used for wall-jump direction.
        wallJumpTimer = GetNode<Timer>("Timers/WallJumpTimer");
        landTimer = GetNode<Timer>("Timers/LandTimer");
        wallRayDirection = wallDetection.Scale.X;
        currentHealth = maximumHealth;
        numberOfJumps = maximumJumps;
    }

    //Main method of the player, reads code for each frame the game runs.
    public override void _PhysicsProcess(double delta) {
        horizontalDirection = inputManager.horizontalButton(); //Axis type, assigns X-axis controls (Left and Right).
        characterVelocity = Velocity * ((float)delta * 60.0f);
        float FPSstabalizer = (float)delta * 60.0f;
 
        //State Machine with Conditions.
        StateConditions((float)delta);
        switch(currentState) {
            case PlayerState.Idle :
                if(inputManager.jumpButton()) currentState = PlayerState.Jump;
                break;
            case PlayerState.Move :
                if(inputManager.jumpButton()) currentState = PlayerState.Jump;
                else characterMovement(walkingSpeed, acceleration, friction);
                break;
            case PlayerState.Jump :
                if(numberOfJumps < maximumJumps) {
                    characterVelocity = new Vector2(characterVelocity.X, jumpVelocity) * FPSstabalizer;
                    numberOfJumps += 1;
                } break;
            case PlayerState.Fall :
                float airMoveParameter = -1000.0f;
                characterVelocity = new Vector2(characterVelocity.X, characterVelocity.Y += gravityValue * (float)delta);
                if(characterVelocity.Y >= airMoveParameter) characterMovement(airVelocity, airAcceleration, airFriction);
                break;
            case PlayerState.Land :
                if(IsOnFloor()) characterVelocity.X = 0.0f;
                landTimer.Start();
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
                characterVelocity.Y = Mathf.MoveToward(characterVelocity.Y, wallSlideSpeed, acceleration) * FPSstabalizer;
                break;
            case PlayerState.WallKick :
                characterVelocity = new Vector2(-(wallPushback * horizontalDirection), wallJumpHeight) * FPSstabalizer;
                numberOfJumps = 0;
                break;
            case PlayerState.Punch :
                characterVelocity = Vector2.Zero;
                break;
            case PlayerState.Kick :
                characterVelocity = Vector2.Zero;
                break;
        }
        Velocity = characterVelocity; //Setting the Velocity Vector2 equal to velocity.
        flipCharacter(); //Flips the player (Raycasts, animations will be flipped in Animation script).
        MoveAndSlide(); //A necessary method to make movement work in Redot engine.
        GD.Print("Current State: " + currentState);
    }
 
    public void StateConditions(float delta) { //Method that sets conditions for each state.
        if(inputManager.jumpButton() || characterVelocity.Y < 0.0f) currentState = PlayerState.Jump;
        if(IsOnWallOnly()) {
            if(wallDetection.IsColliding() && horizontalDirection != 0.0f) currentState = PlayerState.Walled;
            if(wallDetection.IsColliding() && inputManager.jumpButton() && horizontalDirection != 0.0f) currentState = PlayerState.WallKick;
        } else if(IsOnFloorOnly()) {    
            coyoteCounter = coyoteTime; //CoyoteTime = 1.5f.
            characterVelocity.Y = 0.0f; //Resets the gravity.
            numberOfJumps = 0; //Resets the counter.
 
            //Movement logic.
            if(inputManager.jumpButton() || characterVelocity.Y < 0.0f) currentState = PlayerState.Jump;
            if(horizontalDirection == 0.0f && characterVelocity.X == 0.0f) currentState = PlayerState.Idle;
            else currentState = PlayerState.Move;
            
            //Crouch and Slide logic.
            if(!inputManager.crouchButton()) {
                isCrouching = false;
                if(FloorMaxAngle > 63) FloorStopOnSlope = false;
                else FloorStopOnSlope = true;
                playerCollision.Shape = normalCollision;
                playerCollision.Position = new Vector2(0, 1);
            } else {
                isCrouching = true;
                characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, slideFriction);
                playerCollision.Shape = crouchCollision;
                playerCollision.Position = new Vector2(0, 9);
            } if(isCrouching) {
                if(characterVelocity.X <= walkingSpeed && characterVelocity.X >= -walkingSpeed) currentState = PlayerState.Crouch;
                else currentState = PlayerState.Slide;
                FloorStopOnSlope = false;
                FloorMaxAngle = 0.0f; //Allows players to slide only on flat or negative slope surfaces.
                FloorSnapLength = 1; //Allows the player to stick to the floor.
            }
        } else {
            if(characterVelocity.Y > 0.0f) currentState = PlayerState.Fall;
            if(!IsOnFloor() && groundDetection.IsColliding()) currentState = PlayerState.Land;
            characterVelocity.Y += gravityValue * (float)delta; //Sets the character gravity.   
            coyoteCounter -= (float)delta;
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

    //Signal method for when timer finishes when landing on the floor.
    private void whenLandTimerFinishes() => currentState = PlayerState.Idle;  
}

using Godot;
using System;
using System.Diagnostics;
 
public enum PlayerState { //Character States
        Idle, Move, Jump, Fall, Land, Walled, //Traversal states (movement). 
        Crouch, Dive, Grab, Punch, Kick //Action states (evade, attacks, etc).
}

public partial class PlayerController : CharacterBody2D {
    [Signal] public delegate int DamageDealtEventHandler(int health); //Signal sent for when the player damages enemies.
    public float wallRayDirection, walkingSpeed = 53.0f, maximumSpeed = 164.3f;
    public float horizontalDirection, currentHealth, maximumHealth = 100.0f;
    public int maximumJumps = 2, numberOfJumps = 0;
    public PlayerState currentState = PlayerState.Idle;
    public Vector2 characterVelocity;

    private bool isCrouching = false; //By default.
    private float jumpVelocity = -121.90f, horizontalJumpVelocity = 100.70f, gravityValue = 186.03f, airVelocity = 87.45f;
    private float acceleration = 2.65f, airAcceleration = 2.915f, airFriction = 1.272f, friction = 3.180f;
    private float wallPushback = 1060.0f, wallJumpHeight = 1325.0f, wallSlideSpeed = 116.6f;
    private float coyoteCounter = 0.53f, coyoteTime, slideFriction = 36.570f;
    private Shape2D crouchCollision, normalCollision; 
    private RayCast2D groundDetection, wallDetection, ceilingDetection;
    private CollisionShape2D playerCollision;
    private Timer wallJumpTimer, landTimer;
    private InputManager inputManager; 

    public override void _Ready() {
        inputManager = GetNode<InputManager>("/root/InputManager");
        playerCollision = GetNode<CollisionShape2D>("PlayerCollision");
        crouchCollision = GD.Load<Shape2D>("/root/Scenes/Resources/sid_crouch_collision.res"); //Preload for crouch-collision shape.
        normalCollision = GD.Load<Shape2D>("/root/Scenes/Resources/sid_normal_collision.res"); //Preload for normal-collision shape.
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
        characterVelocity = Velocity;
 
        //State Machine with Conditions.
        StateConditions((float)delta);
        switch(currentState) {
            case PlayerState.Idle :
                if(inputManager.jumpButton()) currentState = PlayerState.Jump;
                else if(IsOnFloorOnly() && inputManager.crouchButton()) currentState = PlayerState.Crouch;
                else characterVelocity.X = 0.0f;
                break;
            case PlayerState.Move :
                if(inputManager.jumpButton()) currentState = PlayerState.Jump;
                else if(inputManager.crouchButton()) currentState = PlayerState.Crouch;
                else characterMovement(walkingSpeed, acceleration, friction);
                break;
            case PlayerState.Jump :
                characterJump((float)delta);
                break;
            case PlayerState.Fall :
                float airMoveParameter = -1000.0f;
                characterVelocity = new Vector2(characterVelocity.X, characterVelocity.Y += gravityValue * (float)delta);
                if(characterVelocity.Y >= airMoveParameter) characterMovement(airVelocity, airAcceleration, airFriction);
                if(!IsOnFloor() && groundDetection.IsColliding()) currentState = PlayerState.Land;
                break;
            case PlayerState.Land :
                characterVelocity.X = 0.0f;
                landTimer.Start();
                break;
            case PlayerState.Crouch :
                if(!inputManager.crouchButton()) currentState = PlayerState.Idle;
                else characterCrouch();
                break;
            case PlayerState.Walled : 
                characterVelocity.Y = Mathf.MoveToward(characterVelocity.Y, wallSlideSpeed, acceleration);
                if(wallDetection.IsColliding() && horizontalDirection == 0.0f || !wallDetection.IsColliding()) currentState = PlayerState.Fall;
                //if(Input.IsActionPressed("player_jump")) currentState = PlayerState.Jump; 
                break;
        } //GD.Print("Current State: " + currentState);
        Velocity = characterVelocity; //Setting the Velocity Vector2 equal to velocity.
        flipCharacter(); //Flips the player (Raycasts, animations will be flipped in Animation script).
        MoveAndSlide(); //A necessary method to make movement work in Redot engine.
    }
 
    public void StateConditions(float delta) { //Method that sets conditions for each state.
        if(inputManager.jumpButton() && (IsOnWallOnly() || IsOnFloorOnly()) && numberOfJumps < maximumJumps) currentState = PlayerState.Jump;
        if(IsOnWallOnly() && wallDetection.IsColliding()) currentState = PlayerState.Walled;
        else if(IsOnFloorOnly()) {    
            if(horizontalDirection == 0.0f && characterVelocity.X == 0.0f) currentState = PlayerState.Idle;
            else currentState = PlayerState.Move;
            coyoteCounter = coyoteTime; //CoyoteTime = 1.5f.
            characterVelocity.Y = 0.0f; //Resets the gravity.
            numberOfJumps = 0; //Resets the counter.
        } else {
            if(characterVelocity.Y > 0.0f) currentState = PlayerState.Fall;
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
 
    //Both in-air and on the ground.
    private void characterMovement(float endingSpeed, float incrementValue, float decrementValue) {
        if(horizontalDirection != 0.0f) {
            if(IsOnFloorOnly() && inputManager.runButton()) {
                characterVelocity.X = Mathf.MoveToward(characterVelocity.X, maximumSpeed * horizontalDirection, acceleration * 2.0f); //Run-movement code;
            } else characterVelocity.X = Mathf.MoveToward(characterVelocity.X, endingSpeed * horizontalDirection, incrementValue); //Walk-movement code.
        } else characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, decrementValue); //Player-friction code.
    }

    private void characterJump(float delta) {
        if(inputManager.jumpButton()) {
            if(IsOnFloorOnly() || numberOfJumps < maximumJumps) {
                characterVelocity = new Vector2(characterVelocity.X, jumpVelocity) * (float)delta;
                numberOfJumps += 1;
            } else if(IsOnWallOnly() && wallDetection.IsColliding()) {
                characterVelocity = new Vector2(-(wallPushback * horizontalDirection), wallJumpHeight);
                numberOfJumps = 0;
            }
        }
    }
 
    private void characterCrouch() {
        if(inputManager.crouchButton()) {
            playerCollision.Shape = crouchCollision;
            isCrouching = true;
            if(characterVelocity.X != 0.0f) {
                if(characterVelocity.X == maximumSpeed || characterVelocity.X == -maximumSpeed) characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, -(slideFriction/2)); //Sliding    
                else characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, -slideFriction); //Crouching
            }
        } else {
            playerCollision.Shape = normalCollision;
            currentState = PlayerState.Idle;
            isCrouching = false;
        }
    }

    //Signal method for when timer finishes when landing on the floor.
    private void whenLandTimerFinishes() => currentState = PlayerState.Idle;  
}

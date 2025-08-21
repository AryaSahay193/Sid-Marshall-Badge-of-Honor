using Godot;
using System;
using System.Diagnostics;
 
public partial class SidMarshall : CharacterBody2D {
    //Accessible methods for any scripts (public modifier)
    [Signal] public delegate int DamageEventHandler(int health); //Signal for damaging enemies.
    public float walkingSpeed = 53.0f, maximumSpeed = 164.3f;
    public float horizontalDirection, currentHealth, maximumHealth = 100.0f;
    public int maximumJumps = 2, numberOfJumps = 0;
    public bool inBattleMode = false;
    public PlayerState currentState = PlayerState.Idle;
    public Vector2 characterVelocity;
 
    private float rayDirection, acceleration = 2.65f, airAcceleration = 2.756f, airFriction = 1.219f, friction = 1.113f;
    private float jumpVelocity = -127.2f, horizontalJumpVelocity = 100.7f, gravityValue = 270.3f, airVelocity = 87.45f;
    private float wallPushback = 1060.0f, wallJumpHeight = 1325.0f, wallSlideSpeed = 116.6f;
    private float coyoteCounter = 0.53f, coyoteTime, groundSlideSpeed = 3.5f; 
    private RayCast2D groundDetection, wallDetection;
    private CollisionShape2D playerCollisionShape;
    private InputManager inputManager;
    
    public override void _Ready() {
        //inputManager = GetNode<InputManager>("/root/InputManager");
        playerCollisionShape = GetNode<CollisionShape2D>("PlayerCollision");
        groundDetection = GetNode<RayCast2D>("GroundDetection"); //Raycast used for the Landing state.
        wallDetection = GetNode<RayCast2D>("WallDetection"); //Raycast used for wall-jump direction.
        rayDirection = wallDetection.Scale.X;
        currentHealth = maximumHealth;
        numberOfJumps = maximumJumps;
    }
 
    public enum PlayerState {
        Idle, Move, Jump, Fall, Land, Walled, //Traversal states (movement). 
        Crouched, Dive, Grab, Punch, Kick //Action states (evade, attacks, etc).
    }
 
    //Main method of the player, reads code for each frame the game runs.
    public override void _Process(double delta) {
        horizontalDirection = Input.GetAxis("player_left", "player_right"); //Assigns X-axis controls (Left and Right).
        characterVelocity = Velocity;
 
        //State Machine with Conditions.
        StateConditions((float)delta);
        switch(currentState) {
            case PlayerState.Idle :
                characterVelocity.X = 0.0f;
                break;
            case PlayerState.Move :
                characterMovement(walkingSpeed, acceleration, friction);
                break;
            case PlayerState.Jump :
                characterJump();
                break;
            case PlayerState.Fall :
                characterMovement(airVelocity, airAcceleration, airFriction);
                break;
            case PlayerState.Land :
                currentState = PlayerState.Idle;
                break;
            case PlayerState.Crouched :
                if(Input.IsActionJustReleased("player_down")) currentState = PlayerState.Idle;
                else characterCrouch();
                break;
            case PlayerState.Walled : 
                characterVelocity.Y = Mathf.MoveToward(characterVelocity.Y, wallSlideSpeed, acceleration);
                if(!IsOnWall() || IsOnWall() && horizontalDirection == 0.0f) currentState = PlayerState.Fall;
                break;
        } GD.Print("Current State: " + currentState);
        Velocity = characterVelocity; //Setting the Velocity Vector2 equal to velocity.
        flipCharacter(); //Flips the player (Raycasts, animations will be flipped in Animation script).
        MoveAndSlide(); //A necessary method to make movement work in Redot engine.
    }

    //Physics-Process method, handles code dealing with time or physics.
    public override void _PhysicsProcess(double delta) {
        if(IsOnFloorOnly()) {
            if(horizontalDirection == 0.0f && characterVelocity.X == 0.0f) currentState = PlayerState.Idle;
            else currentState = PlayerState.Move;
            characterVelocity.Y = 0.0f; //Resets the gravity.
        }
    }
 
    public void StateConditions(float delta) { //Method that sets conditions for each state.
        if(Input.IsActionJustPressed("player_jump") && numberOfJumps < maximumJumps) currentState = PlayerState.Jump;
        if(characterVelocity.Y < 0.0f && groundDetection.IsColliding()) currentState = PlayerState.Land;
        if(IsOnWallOnly() && wallDetection.IsColliding()) currentState = PlayerState.Walled;
        else if(IsOnFloor()) {    
            if(Input.IsActionPressed("player_down")) currentState = PlayerState.Crouched;
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
                rayDirection = 1.0f;
                wallDetection.Scale = new Vector2(rayDirection, wallDetection.Scale.Y);
            } else {
                rayDirection = -1.0f;
                wallDetection.Scale = new Vector2(rayDirection, wallDetection.Scale.Y);
            }
        }
    }
 
    //Both in-air and on the ground.
    private void characterMovement(float endingSpeed, float incrementValue, float decrementValue) {
        if(horizontalDirection != 0.0f) {
            if(IsOnFloorOnly() && Input.IsActionPressed("player_run")) {
                characterVelocity.X = Mathf.MoveToward(characterVelocity.X, maximumSpeed * horizontalDirection, acceleration * 2.0f); //Run-movement code;
            } else characterVelocity.X = Mathf.MoveToward(characterVelocity.X, endingSpeed * horizontalDirection, incrementValue); //Walk-movement code.
        } else characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, decrementValue); //Player-friction code.
    }

    private void characterJump() {
        if(Input.IsActionJustPressed("player_jump")) {
            if(IsOnFloor() || numberOfJumps < maximumJumps) {
                characterVelocity.X = horizontalJumpVelocity * horizontalDirection;
                characterVelocity.Y = jumpVelocity;
                numberOfJumps += 1;
            } else if(wallDetection.IsColliding()) {
                numberOfJumps = 0;
                characterVelocity.X = -(wallPushback * horizontalDirection);
                characterVelocity.Y = wallJumpHeight;
            }
        }
    }
 
    private void characterCrouch() {
        //Change collision to new size.
        //((RectangleShape2D)playerCollisionShape.Shape);
        if(characterVelocity.X != maximumSpeed) {
            characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, friction);
            if(Input.IsActionJustReleased("player_down")) {
                //Change collision back to normal.
            }
        } else characterVelocity.X = Mathf.MoveToward(characterVelocity.X, 0.0f, groundSlideSpeed);
    }
}

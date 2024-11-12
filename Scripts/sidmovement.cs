using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SidMovement : MonoBehaviour {
    //[SerializeField] allows you to directly edit a numerical value into Unity.
    [Header("Movement Variables")]
    private float currentSpeed;
    private float currentHeight;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float friction;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float jumpingSpeed;
    private Vector2 rayCastSize = new Vector2(0.3f, 0.2f); //Creates the size of the BoxCast
    private float rayCastDistance = 0.3f; //Determines how far the RayCast detects the ground.
    
    [Header("Jump Variables")]
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBufferTime;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private int jumpsLeft;
    private int maximumJumps = 2;
    
    [Header("Input-System References")]
    private float horizontalDirection;
    private bool runningAction;
    private bool jumpingAction;
    private bool wallJumpingAction;
    private bool crouchingAction;
    
    [Header("Reference Variables")]
    [SerializeField] private PhysicsMaterial2D physicsMaterial;
    [SerializeField] private Rigidbody2D sidMarshallRigidBody; //References the RigidBody2D class as a variable.
    [SerializeField] private LayerMask tileSetLayer; //Choose Layers in Unity.
    private Animator sidAnimations; //References the Animator class as a variable.
    private BoxCollider2D boxCastCollision; //References the BoxCollider2D class as a variable.

    private void Awake() { //Everytime the script starts, the code runs with the Awake method.
        //Grabs references for the nodes in Unity.
        sidMarshallRigidBody = GetComponent<Rigidbody2D>(); //Initializes the RigidBody component
        sidAnimations = GetComponent<Animator>(); //Initializes the Animator component
        boxCastCollision = GetComponent<BoxCollider2D>(); //Initializes the BoxCollider2D component.
        jumpsLeft = maximumJumps; //On Startup, Sid has 2 jumps available to use.
    }

    //Method for handling code that runs every frame, such as Animation Logic.
    private void Update() {
        //Flips the character sprite.
        if(horizontalDirection > 0) {
            transform.localScale = Vector3.one; //Keeps the vector the same.
        } else if(horizontalDirection < 0) {
            transform.localScale = new Vector3(-1, 1, 1); //Only changes the X-value of vector 3 (flips the character)
        }
        
        currentSpeed = sidMarshallRigidBody.velocity.x;
        currentHeight = sidMarshallRigidBody.velocity.y;
        Vector2 currentPosition = new Vector2(sidMarshallRigidBody.velocity.x, sidMarshallRigidBody.velocity.y);
        //Basic Movement: Walking and Running (With animation-logic)
        if(groundChecker()) {
            sidAnimations.SetBool("Grounded", true);
            if(horizontalDirection == 0) {
                sidAnimations.SetFloat("Horizontal Velocity", 0.0f); //Idle Animation Logic
            } //This if-else statement is basically calculating the velocity which we can then substitute in our RigidBody2D component.
            if(horizontalDirection != 0) {
                currentSpeed = walkingSpeed * horizontalDirection;
                sidAnimations.SetFloat("Horizontal Velocity", 1.0f); //Walking Animation Logic
                if(runningAction && currentSpeed < runningSpeed) {
                    currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, runningSpeed * horizontalDirection, acceleration * Time.deltaTime * 10.0f);
                    sidAnimations.SetFloat("Horizontal Velocity", 2.0f); //Running Animation Logic
                }
            } else {
                currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, 0.0f, friction * Time.deltaTime * 10.0f);
                if(!runningAction && horizontalDirection != 0) {
                    currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, walkingSpeed, friction * Time.deltaTime * 10.0f);
                }
            } 
            currentPosition = new Vector2(currentSpeed, currentHeight);
            sidMarshallRigidBody.velocity = currentPosition; //After all the calculations, the velocity is applied to the RigidBody2D
            slopeAdjustment(currentPosition);
        }

        //Jumping, Double-Jumping, and Wall-Jumping Logic (with animation logic)
        if(jumpingAction) {
            jumpBufferCounter = jumpBufferTime; //Resets the jump-buffer.
        } else {
            jumpBufferCounter -= Time.deltaTime;
        } if(groundChecker()) {
            coyoteCounter = coyoteTime; //Resets the CoyoteCounter.
            jumpsLeft = maximumJumps; //Resets the amount of jumps.
        } else if(!groundChecker()) {
            coyoteCounter -= Time.deltaTime;
            if(currentHeight < 0) {
                sidAnimations.SetBool("Grounded", false);
                sidAnimations.SetFloat("Vertical Velocity", -1.0f); //Falling Logic
            }
        }

        if(coyoteCounter > 0 && jumpBufferCounter > 0) {
            performJump();
        } if(!jumpingAction && currentHeight > 0) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, currentHeight * 0.5f); //Variable Jump Height by length of button press.
            coyoteCounter = 0.0f;
        } 
    }

    //Basic Movement Input Method.
    public void Movement(InputAction.CallbackContext movement) {
        horizontalDirection = movement.ReadValue<Vector2>().x; //Reads on the x-values
    }

    //Basic Running Input Method
    public void Run(InputAction.CallbackContext running) {
        runningAction = running.ReadValueAsButton(); //Reads a single button value.
    }

    //Basic Jumping Input Method
    public void Jump(InputAction.CallbackContext jumping) {
        jumpingAction = jumping.ReadValueAsButton();
    }

    private void performJump() {
        if(coyoteCounter <= 0.0f && !wallChecker() && jumpsLeft <= 0) {
            return;
        } if(groundChecker() || coyoteCounter > 0) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, jumpingSpeed);
            sidAnimations.SetFloat("Vertical Velocity", 0.0f); //Single Jump Logic
            sidAnimations.SetBool("Grounded", false);
            jumpBufferCounter = 0.0f;
        } else if(jumpsLeft > 0) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, jumpingSpeed);
            sidAnimations.SetFloat("Vertical Velocity", 1.0f); //Double Jump Logic
            jumpsLeft -= 1;
        } coyoteCounter = 0.0f;
    }

    public bool groundChecker() {
        RaycastHit2D collisionDetection = Physics2D.BoxCast(boxCastCollision.bounds.center, boxCastCollision.bounds.size, 0.0f, Vector2.down, 0.1f, tileSetLayer);
        return collisionDetection.collider != null;
    }

    public bool wallChecker() {
        Vector2 wallDirection = new Vector2(transform.localScale.x, 0);
        RaycastHit2D collisionDetection = Physics2D.BoxCast(boxCastCollision.bounds.center, boxCastCollision.bounds.size, 0.0f, wallDirection, 0.1f, tileSetLayer);
        return collisionDetection.collider != null;
    }

    private Vector2 slopeAdjustment(Vector2 velocity) {
        var collisionRay = new Ray(transform.position, Vector2.down);
        if(Physics.Raycast(collisionRay, out RaycastHit hitInfo, 0.2f)) {
            var rotateToSlope = Quaternion.FromToRotation(Vector2.up, hitInfo.normal);
            var adjustedVelocity = rotateToSlope * velocity * Time.deltaTime;
            if(adjustedVelocity.y < 0) {
                return adjustedVelocity;
            }
        } return velocity;
    }
}

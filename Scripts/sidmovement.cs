using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SidMovement : MonoBehaviour {
    //[SerializeField] allows you to directly edit a numerical value into Unity.
    [Header("Movement Variables")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float friction;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float jumpingSpeed;
    private Vector2 rayCastSize = new Vector2(0.5f, 0.2f); //Creates the size of the BoxCast
    private float rayCastDistance = 0.3f; //Determines how far the RayCast detects the ground.
    
    [Header("Jump Variables")]
    private float jumpsLeft;
    private float maximumJumps = 2;
    private float coyoteTime;
    private float coyoteCounter;
    
    [Header("Input-System References")]
    private float horizontalDirection;
    private bool runningAction;
    private bool jumpingAction;
    private bool wallJumpingAction;
    private bool crouchingAction;
    
    [Header("Reference Variables")]
    [SerializeField] public LayerMask tileSetLayer; //Choose Layers in Unity.
    [SerializeField] private Rigidbody2D sidMarshallRigidBody; //References the RigidBody2D class as a variable.
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
        
        float currentSpeed = sidMarshallRigidBody.velocity.x;
        float currentHeight = sidMarshallRigidBody.velocity.y;
        Vector2 currentPosition = new Vector2(sidMarshallRigidBody.velocity.x, sidMarshallRigidBody.velocity.y);
        //Basic Movement: Walking and Running (With animation-logic)
        if(groundChecker()) {
            sidAnimations.SetBool("Grounded", true);
            jumpsLeft = maximumJumps;
            coyoteCounter = coyoteTime;
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
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, currentHeight); //After all the calculations, the velocity is applied to the RigidBody2D
            slopeAdjustment(currentPosition);
        }

        //Jumping Logic (with animation logic)
        if(jumpingAction && jumpsLeft > 0) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, jumpingSpeed);
            sidAnimations.SetFloat("Vertical Velocity", 0.0f); //Single Jump Logic
            sidAnimations.SetBool("Grounded", false);
            coyoteCounter -= Time.deltaTime;
            jumpsLeft -= 1;
            if(jumpsLeft == 0) {
                sidAnimations.SetFloat("Vertical Velocity", 1.0f); //Double Jump Logic
            } 
        } else if(!jumpingAction && currentHeight > 0) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, currentHeight * 0.5f);
        } else if(!groundChecker() && currentHeight < 0) {
            sidAnimations.SetBool("Grounded", false);
            sidAnimations.SetFloat("Vertical Velocity", -1.0f); //Falling Logic
        } 
    }

    //Basic Movement Method.
    public void Movement(InputAction.CallbackContext movement) {
        horizontalDirection = movement.ReadValue<Vector2>().x; //Reads on the x-values
    }

    //Acceleration and Friction Method
    public void Run(InputAction.CallbackContext running) {
        runningAction = running.ReadValueAsButton(); //Reads a single button value.
    }

    //Jump Method
    public void Jump(InputAction.CallbackContext jumping) {
        jumpingAction = jumping.ReadValueAsButton();
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

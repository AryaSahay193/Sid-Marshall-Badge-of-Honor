using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SidMovement : MonoBehaviour {
    //[SerializeField] allows you to directly edit a numerical value into Unity.
    [SerializeField] public LayerMask tileSetLayer; //Choose Layers in Unity.
    private float walkingSpeed = 3.5f;
    private float runningSpeed = 2.0f;
    private float jumpingSpeed = 10.0f;
    private Vector2 rayCastSize = new Vector2(0.5f, 0.2f); //Creates the size of the BoxCast
    private float rayCastDistance = 0.3f; //Determines how far the RayCast detects the ground.
    private float horizontalMovement;
    private float jumpsLeft;
    private float maximumJumps = 2;
    private bool runningAction;
    private bool jumpingAction;
    private bool crouchingAction;
    
    //Reference variables
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
        float verticalMovement = sidMarshallRigidBody.velocity.y;
        //Animation Logic - Horizontal Movement
        if(horizontalMovement == 0 && groundChecker() || verticalMovement == 0 && groundChecker()) {
            sidAnimations.SetBool("Grounded", true);
            sidAnimations.SetFloat("Horizontal Velocity", 0.0f); //Idle Logic
            } if(horizontalMovement != 0) {
                sidAnimations.SetFloat("Horizontal Velocity", 1.0f); //Walking Logic
                if(runningAction) {
                    sidAnimations.SetFloat("Horizontal Velocity", 2.0f); //Running Logic
                }
            }

        //Animation Logic - Vertical Movement
        if(verticalMovement > 0 && jumpingAction) {
            sidAnimations.SetBool("Grounded", false);
            sidAnimations.SetFloat("Vertical Velocity", 0.0f); //Single Jump Logic
            if(jumpsLeft == 0) {
                sidAnimations.SetFloat("Vertical Velocity", 1.0f); //Double Jump Logic
            } 
        } else if(verticalMovement < 0) {
            sidAnimations.SetFloat("Vertical Velocity", -1.0f); //Falling Logic
        }
        
        //Flips the character sprite.
        if(horizontalMovement > 0) {
            transform.localScale = Vector3.one; //Keeps the vector the same.
        } else if(horizontalMovement < 0) {
            transform.localScale = new Vector3(-1, 1, 1); //Only changes the X-value of vector 3 (flips the character)
        }
    }
    
    //Method best used for Physcis-Related code, such as Movement Logic.
    private void FixedUpdate() {
        //Basic Movement: Walking and Running
        sidMarshallRigidBody.velocity = new Vector2(horizontalMovement * walkingSpeed, sidMarshallRigidBody.velocity.y); //You can change the velocity of X, but Y stays the same.
        if(runningAction && horizontalMovement != 0) {
            sidMarshallRigidBody.velocity = new Vector2(sidMarshallRigidBody.velocity.x * runningSpeed, sidMarshallRigidBody.velocity.y);
        }

        //Resets the amount of jumps.
        if(groundChecker() && sidMarshallRigidBody.velocity.y <= 0) {
            jumpsLeft = maximumJumps;
        }
    }

    //Basic Movement Method.
    public void Movement(InputAction.CallbackContext movement) {
        horizontalMovement = movement.ReadValue<Vector2>().x; //Reads on the x-values
    }

    //Acceleration and Friction Method
    public void Run(InputAction.CallbackContext acceleration) {
        runningAction = acceleration.ReadValueAsButton(); //Reads a single button value.
    }

    //Jump Method
    public void Jump(InputAction.CallbackContext jump) {
        jumpingAction = jump.ReadValueAsButton();
        if(jump.performed && jumpsLeft > 0) {
            sidMarshallRigidBody.velocity = new Vector2(sidMarshallRigidBody.velocity.x, jumpingSpeed);
            jumpsLeft -= 1;
        } if(jump.canceled && sidMarshallRigidBody.velocity.y > 0.0f) {
            sidMarshallRigidBody.velocity = new Vector2(sidMarshallRigidBody.velocity.x, sidMarshallRigidBody.velocity.y * 0.5f);
        }
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
}

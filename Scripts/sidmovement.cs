using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SidMovement : MonoBehaviour {
    //[SerializeField] allows you to directly edit a numerical value into Unity.
    [Header("Reference Variables")]
    [SerializeField] private Rigidbody2D sidMarshallRigidBody; //References the RigidBody2D class as a variable.
    [SerializeField] private PhysicsMaterial2D physicsMaterial;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private Transform groundCollider;
    [SerializeField] private Transform wallCollider;
    [SerializeField] private LayerMask tileSetLayer; //Choose Layers in Unity.
    private Animator sidAnimations; //References the Animator class as a variable.
    private BoxCollider2D boxCastCollider; //References the BoxCollider2D class as a variable.
    private Vector2 boxCastSize;
    
    [Header("Ground Detection")]
    private Vector2 rayCastSize = new Vector2(0.3f, 0.2f); //Creates the size of the BoxCast
    private Vector2 slopeNormalPerpendicularAngle;
    private float rayCastDistance = 0.3f; //Determines how far the RayCast detects the ground.
    private float slopeCurrentAngle;
    private float slopePreviousAngle;
    private bool isOnTheSlope;
    
    [Header("Movement Variables")]
    private float currentSpeed;
    private float currentHeight;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float friction;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float jumpingSpeed;
    [SerializeField] private float wallSlidingSpeed;
    [SerializeField] private Vector2 wallJumpSpeed;
    
    [Header("Jump Variables")]
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBufferTime;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float wallJumpTime;
    private float wallJumpCounter;
    private float wallJumpDirection;
    private float wallJumpDuration;
    private int jumpsLeft;
    private int maximumJumps = 2;
    private bool doubleJumping;
    private bool wallSliding;
    private bool wallJumping;
    
    [Header("Input-System References")]
    private float horizontalDirection;
    private bool runningAction;
    private bool jumpingAction;
    private bool attackingAction;
    private bool crouchingAction;

    private void Awake() { //Everytime the script starts, the code runs with the Awake method.
        //Grabs references for the nodes in Unity.
        sidMarshallRigidBody = GetComponent<Rigidbody2D>(); //Initializes the RigidBody component
        sidAnimations = GetComponent<Animator>(); //Initializes the Animator component
        boxCastCollider = GetComponent<BoxCollider2D>(); //Initializes the BoxCollider2D component.
        boxCastSize = boxCastCollider.size;
    }

    //Method for handling code that runs every frame, such as Animation Logic.
    private void Update() {
        currentSpeed = sidMarshallRigidBody.velocity.x;
        currentHeight = sidMarshallRigidBody.velocity.y;
        Vector2 currentPosition = new Vector2(sidMarshallRigidBody.velocity.x, sidMarshallRigidBody.velocity.y);
        //Basic Movement: Walking and Running (With animation-logic)        
        if(!wallJumping) {
            flipCharacter();
            if(groundChecker()) {
                sidAnimations.SetBool("Grounded", true);
                coyoteCounter = coyoteTime; //Resets the CoyoteCounter. 
                performMovement();
                currentPosition = new Vector2(currentSpeed, currentHeight);
                sidMarshallRigidBody.velocity = currentPosition; //After all the calculations, the velocity is applied to the RigidBody2D
                slopeCheck();
            } else {
                coyoteCounter -= Time.deltaTime;
                if(currentHeight < 0) {
                    sidAnimations.SetBool("Grounded", false);
                    sidAnimations.SetFloat("Vertical Velocity", -1.0f); //Falling Logic
                }
            }
        }

        if(wallChecker()) {
            performWallJump();
        }

        if(jumpingAction) {
            jumpBufferCounter = jumpBufferTime; //Resets the jump-buffer.
        } else {
            jumpBufferCounter -= Time.deltaTime;
        } if(coyoteCounter > 0 && jumpBufferCounter >= 0) {
            performJump();
            jumpBufferCounter = 0.0f;
        } if(!jumpingAction && currentHeight > 0) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, currentHeight * 0.5f); //Variable Jump Height by length of button press.
            coyoteCounter = 0.0f;
        } 
    }

    //Assigns the movement buttons to the movement mechanic.
    public void Movement(InputAction.CallbackContext movement) {
        horizontalDirection = movement.ReadValue<Vector2>().x; //Reads on the x-values
    }

    //Assigns the run button to the acceleration mechanic.
    public void Run(InputAction.CallbackContext running) {
        runningAction = running.ReadValueAsButton(); //Reads a single button value.
    }

    //Assigns the jump button to the jump mechanic.
    public void Jump(InputAction.CallbackContext jumping) {
        jumpingAction = jumping.ReadValueAsButton();
    }

    //Method that deals with movement code (including walking, acceleration and friction).
    private void flipCharacter() {
        //Flips the character sprite.
        if(horizontalDirection > 0) {
            transform.localScale = Vector3.one; //Keeps the vector the same.
        } else if(horizontalDirection < 0) {
            transform.localScale = new Vector3(-1, 1, 1); //Only changes the X-value of vector 3 (flips the character)
        } dustParticles.Play();
    }
    
    private void performMovement() {
        if(horizontalDirection == 0) {
            sidAnimations.SetFloat("Horizontal Velocity", 0.0f); //Idle Animation Logic
        } if(horizontalDirection != 0) { //This if-else statement is basically calculating the velocity which we can then substitute in our RigidBody2D component.
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

        if(groundChecker() && isOnTheSlope) {
            float slopeVelocity = currentSpeed * -horizontalDirection;
            sidMarshallRigidBody.velocity = new Vector2(slopeVelocity * slopeNormalPerpendicularAngle.x, slopeVelocity * slopeNormalPerpendicularAngle.y);
        }
    }
    
    //Method that deals with jumping code (including Coyote Time, Double Jump and Jump Buffer).
    private void performJump() {
        if(coyoteCounter <= 0.0f && !wallChecker() && doubleJumping == false) {
            return;
        } if(groundChecker() || coyoteCounter > 0.0f) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, jumpingSpeed);
            sidAnimations.SetFloat("Vertical Velocity", 0.0f); //Single Jump Logic
            sidAnimations.SetBool("Grounded", false);
        } else if(doubleJumping) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, jumpingSpeed);
            sidAnimations.SetFloat("Vertical Velocity", 1.0f); //Double Jump Logic
            doubleJumping = false;
        }
    }

    private void performWallJump() {
        if(!groundChecker() && horizontalDirection != 0) {
            wallSliding = true;
        } else {
            wallSliding = false;
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, Mathf.Clamp(currentHeight, -wallSlidingSpeed, float.MaxValue));
        }

        if(wallSliding) {
            wallJumping = false;
            wallJumpDirection = -transform.localScale.x; //Flips direction when the jump button is pressed on a wall.
            wallJumpCounter = wallJumpTime;
            CancelInvoke(nameof(wallJumpCancel));
        } else {
            wallJumpCounter -= Time.deltaTime;
        } 
        
        if(jumpingAction && wallJumpCounter > 0.0f) {
            wallJumping = true;
            sidMarshallRigidBody.velocity = new Vector2(wallJumpDirection * wallJumpSpeed.x, wallJumpSpeed.y);
            wallJumpCounter = 0.0f;
            if(transform.localScale.x != wallJumpDirection) {
                Vector3 directionFacing = transform.localScale;
                directionFacing.x *= -1;
                transform.localScale = directionFacing;
            } Invoke(nameof(wallJumpCancel), wallJumpDuration);
        }
    } void wallJumpCancel() {
        wallJumping = false;
    }

    //Assigns the crouch button to the crouch mechanic.
    public void Crouch(InputAction.CallbackContext crouching) {
        crouchingAction = crouching.ReadValueAsButton();
    }

    //Assigns the attack button to the kicking mechanics.
    public void Attack(InputAction.CallbackContext attacking) {
        attackingAction = attacking.ReadValueAsButton();
    }

    //Method that checks for the ground.
    public bool groundChecker() {
        RaycastHit2D collisionDetection = Physics2D.BoxCast(groundCollider.position, rayCastSize, 0.0f, Vector2.down, 0.1f, tileSetLayer);
        return collisionDetection.collider != null;
    }

    public void slopeCheck() {
        Vector2 playerPosition = transform.position - new Vector3(0.0f, boxCastSize.y / 2);
        RaycastHit2D slopeVerticalDetection = Physics2D.Raycast(playerPosition, Vector2.down, 0.1f, tileSetLayer);
        if(slopeVerticalDetection) {
            Debug.DrawRay(slopeVerticalDetection.point, slopeVerticalDetection.normal, Color.blue);
            slopeNormalPerpendicularAngle = Vector2.Perpendicular(slopeVerticalDetection.normal).normalized;
            slopeCurrentAngle = Vector2.Angle(slopeVerticalDetection.normal, Vector2.up);
            if(slopeCurrentAngle != slopePreviousAngle) {
                isOnTheSlope = true;
            } slopePreviousAngle = slopeCurrentAngle;
        }
    }
    
    //Method that checks for the wall.
    public bool wallChecker() {
        Vector2 wallDirection = new Vector2(transform.localScale.x, 0);
        RaycastHit2D wallCollision = Physics2D.Raycast(wallCollider.position, wallDirection);
        return wallCollision.collider != null;
    }
}

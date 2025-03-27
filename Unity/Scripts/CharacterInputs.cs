using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SidMovement : MonoBehaviour, PlayerController.IPlayerActions {
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
    private float rayCastDistance = 0.15f, slopeCurrentAngle, slopePreviousAngle;
    private bool isOnTheSlope;
    
    [Header("Movement Variables")]
    private Vector2 motion2D;
    private float currentSpeed, currentHeight;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float friction;
    [SerializeField] private float wallAcceleration;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float jumpingSpeed;
    [SerializeField] private float wallSlidingSpeed;
    [SerializeField] private Vector2 wallJumpSpeed;
    
    [Header("Jump Variables")]
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float wallJumpTime;
    private float coyoteCounter, jumpBufferCounter, wallJumpCounter, wallJumpDirection, wallJumpDuration;
    int jumpsLeft, maximumJumps = 2;
    private bool doubleJumping, wallSliding, wallJumping;
    
    [Header("Input-System References")]
    private PlayerController playerControls;
    private bool attackingAction, crouchingAction, doorEnterAction;
    private float directionalInput, runningAction, jumpingAction;

    private void Awake() { //Everytime the script starts, the code runs with the Awake method.
        //Grabs references for the nodes in Unity.
        playerControls = new PlayerController();
        sidMarshallRigidBody = GetComponent<Rigidbody2D>(); //Initializes the RigidBody component
        sidAnimations = GetComponent<Animator>(); //Initializes the Animator component
        boxCastCollider = GetComponent<BoxCollider2D>(); //Initializes the BoxCollider2D component.
        boxCastSize = boxCastCollider.size; //Sets the size for the BoxCollider2D for slope-checks.
        jumpsLeft = maximumJumps; //Resets the amount of jumps at the start of the game.
        playerControls.Player.SetCallbacks(this); //Initializes all control schemes and phases.

    }
    
    //Handles all inputs that subscribe to events when a button press is activated or performed.
    private void OnEnable() {
        playerControls.Enable();
    }

    //Handles all inputs that unsubscribe to events when a button press is deactivated or canceled.
    private void OnDisable() {
        playerControls.Disable();
    }

    //MAIN METHOD: Handles code that runs every frame, such as Animation Logic.
    private void Update() {
        currentSpeed = sidMarshallRigidBody.velocity.x;
        currentHeight = sidMarshallRigidBody.velocity.y;
        //Basic Movement: Walking and Running (With animation-logic)        
        if(!wallJumping) {
            sidAnimations.SetBool("Wall", false);
            flipCharacter();
            if(groundChecker()) {
                coyoteCounter = coyoteTime; //Resets the CoyoteCounter. 
                sidAnimations.SetBool("Grounded", true);
                performMovement();
                slopeCheck();
            } else {
                coyoteCounter -= Time.deltaTime;
                if(currentHeight < 0) {
                    sidAnimations.SetBool("Grounded", false);
                    sidAnimations.SetFloat("Vertical Velocity", -1.0f); //Falling Logic
                }
            }

            if(jumpingAction > 0) {
                jumpBufferCounter = jumpBufferTime; //Resets the jump-buffer.
            } else {
                jumpBufferCounter -= Time.deltaTime;
            } if(coyoteCounter > 0 && jumpBufferCounter >= 0) {
                jumpBufferCounter = 0.0f;
                performJump();
            } if(jumpingAction == 0.0f && currentHeight > 0) {
                sidMarshallRigidBody.velocity = new Vector2(currentSpeed, currentHeight * 0.5f); //Variable Jump Height by length of button press.
                coyoteCounter = 0.0f;
            } 
        }

        if(wallChecker() && !groundChecker()) {
            sidAnimations.SetBool("Wall", true);
            sidAnimations.SetFloat("Vertical Velocity", 3.0f); //Wall-Contact Animation
            performWallJump();
        } else {
            sidAnimations.SetBool("Wall", false);
        }
    }

    //Assigns the movement buttons to the movement mechanic.
    public void OnMovement(InputAction.CallbackContext movement) {
        directionalInput = movement.ReadValue<Vector2>().x; //Reads on the x-values
        motion2D = new Vector2(directionalInput, 0.0f);
    }

    //Assigns the run button to the acceleration mechanic.
    public void OnRun(InputAction.CallbackContext running) {
        runningAction = running.ReadValue<float>(); //Reads a single button value.
    }

    //Assigns the jump button to the jump mechanic.
    public void OnJump(InputAction.CallbackContext jumping) {
        jumpingAction = jumping.ReadValue<float>();
    }

    //Assigns the jump button to the jump mechanic.
    public void OnCrouch(InputAction.CallbackContext crouching) {
        crouchingAction = crouching.ReadValue<bool>();
    }

    //Assigns the attack button to the jump mechanic.
    public void OnAttack(InputAction.CallbackContext attacking) {
        attackingAction = attacking.ReadValue<bool>();
    }

    //Assigns the door-entering button to scene-change scenarios.
    public void OnDoorEnter(InputAction.CallbackContext doorEntering) {
        doorEnterAction = doorEntering.ReadValue<bool>();
    }

    //Method that deals with movement code (including walking, acceleration and friction).
    private void flipCharacter() {
        //Flips the character sprite.
        if(directionalInput > 0) {
            transform.localScale = Vector3.one; //Keeps the vector the same.
        } else if(directionalInput < 0) {
            transform.localScale = new Vector3(-1, 1, 1); //Only changes the X-value of vector 3 (flips the character)
        } dustParticles.Play();
    }
    
    private void performMovement() {
        if(directionalInput == 0) {
            sidAnimations.SetFloat("Horizontal Velocity", 0.0f); //Idle Animation Logic
        } if(directionalInput != 0) { //This if-else statement is basically calculating the velocity which we can then substitute in our RigidBody2D component.
            currentSpeed = walkingSpeed * directionalInput;
            sidAnimations.SetFloat("Horizontal Velocity", 1.0f); //Walking Animation Logic
            if(runningAction != 0 && currentSpeed < runningSpeed) {
                currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, runningSpeed * directionalInput, acceleration * Time.deltaTime * 10.0f);
                sidAnimations.SetFloat("Horizontal Velocity", 2.0f); //Running Animation Logic
            }
        } else {
            currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, 0.0f, friction * Time.deltaTime * 10.0f);
            float frictionalValue = currentSpeed;
            float currentDirection = directionalInput;
            if(frictionalValue != 0 && directionalInput != currentDirection) {
                sidAnimations.Play("Skid");
            }
        }
        Vector2 currentPosition = new Vector2(currentSpeed, currentHeight);
        sidMarshallRigidBody.velocity = currentPosition; //After all the calculations, the velocity is applied to the RigidBody2D

        if(groundChecker() && isOnTheSlope) {
            float slopeVelocity = currentSpeed * -directionalInput;
            sidMarshallRigidBody.velocity = new Vector2(slopeVelocity * slopeNormalPerpendicularAngle.x, slopeVelocity * slopeNormalPerpendicularAngle.y);
        }
    }
    
    //Method that deals with jumping code (including Coyote Time, Double Jump and Jump Buffer).
    private void performJump() {
        if(coyoteCounter <= 0.0f && !wallChecker() && doubleJumping == false) {
            return;
        } if(groundChecker() || coyoteCounter > 0.0f || doubleJumping) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, jumpingSpeed);
            jumpsLeft -= 1;
            sidAnimations.SetBool("Grounded", false);
            doubleJumping = !doubleJumping;
        } if(jumpsLeft == 1) {
            sidAnimations.SetFloat("Vertical Velocity", 0.0f); //Single Jump Logic
        } else if(jumpsLeft == 0) {
            sidAnimations.SetFloat("Vertical Velocity", 1.0f); //Double Jump Logic
        }
    }

    private void performWallJump() {
        if(wallChecker() && !groundChecker() && directionalInput != 0) {
            wallSliding = true;
            sidAnimations.SetFloat("Vertical Velocity", 4.0f); //Wall-Sliding Animation
            float wallFriction = Mathf.MoveTowards(0.0f, -wallSlidingSpeed * Time.deltaTime * 10.0f, -wallAcceleration * Time.deltaTime * 10.0f);
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, Mathf.Max(currentHeight, -wallFriction));
        } else {
            wallSliding = false;
        }

        if(wallSliding) {
            wallJumping = false;
            wallJumpDirection = -transform.localScale.x; //Flips direction when the jump button is pressed on a wall.
            wallJumpCounter = wallJumpTime;
            CancelInvoke(nameof(wallJumpCancel));
        } else {
            wallJumpCounter -= Time.deltaTime;
        } 
        
        if(jumpingAction > 0.0f && wallJumpCounter > 0.0f) {
            wallJumping = true;
            sidMarshallRigidBody.velocity = new Vector2(wallJumpDirection * wallJumpSpeed.x, wallJumpSpeed.y);
            sidAnimations.SetFloat("Vertical Velocity", 5.0f); //Wall-Kick Animation
            wallJumpCounter = 0.0f;
            if(transform.localScale.x != wallJumpDirection) {
                flipCharacter();
            } Invoke(nameof(wallJumpCancel), wallJumpDuration);
        }
    } 
    
    //Method for used for Wall-Jump Invoking.
    private void wallJumpCancel() {
        wallJumping = false;
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
        RaycastHit2D wallCollisionLeft = Physics2D.Raycast(wallCollider.position, -Vector2.right, rayCastDistance, tileSetLayer);
        RaycastHit2D wallCollisionRight = Physics2D.Raycast(wallCollider.position, Vector2.right, rayCastDistance, tileSetLayer);
        return wallCollisionLeft.collider || wallCollisionRight.collider != null;
    }
}

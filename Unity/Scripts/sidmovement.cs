using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Player_Controller : MonoBehaviour, PlayerController.IPlayerActions {
    [Header("Reference Variables")]
    [SerializeField] private Rigidbody2D sidMarshallRigidBody; //References the RigidBody2D class as a variable.
    [SerializeField] private PhysicsMaterial2D physicsMaterial;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private Transform groundCollider;
    [SerializeField] private Transform wallCollider;
    [SerializeField] private Transform ceilingCollider;
    [SerializeField] private LayerMask tileSetLayer; //Choose Layers in Unity.
    private Animator sidAnimations; //References the Animator class as a variable.
    private BoxCollider2D boxCastCollider; //References the BoxCollider2D class as a variable.
    private Vector2 boxCastSize; //Creates a Vector2 for a Box Cast Size.
    private bool faceDirectionRight = true; //By default
    
    [Header("Ground Detection")]
    RaycastHit2D collisionDetection;
    private Vector2 groundRaySize, slopeNormalPerpendicularAngle;
    private float rayCastDistance = 0.25f, slopeCurrentAngle, slopePreviousAngle;
    private bool isOnTheSlope;
    
    [Header("Movement Variables")]
    private float currentSpeed, currentHeight;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float friction;
    [SerializeField] private float wallAcceleration;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float jumpingSpeed;
    [SerializeField] private float slidingSpeed;
    [SerializeField] private float wallSlidingSpeed;
    [SerializeField] private Vector2 wallJumpSpeed;
    
    [Header("Movement Attributes")]
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float diveTime;
    [SerializeField] private float wallJumpTime;
    private float coyoteCounter, jumpBufferCounter, wallJumpCounter, wallJumpDirection, wallJumpDuration, divePower, diveCooldown;
    private bool groundSliding, wallSliding, wallJumping, previouslyGrounded, canDive = true, isDiving;
    private int jumpsLeft, maximumJumps = 2;
    
    [Header("Input-System References")]
    private PlayerController playerControls;
    private bool doublePress, attackingAction, crouchingAction, doorEnterAction, confirmButton, cancelButton;
    private float directionalInput, runningAction, jumpingAction, tapDelay, pressPoint;
    private int tapAmount = 2;

    private void Awake() { //Everytime the script starts, the code runs with the Awake method.
        //Grabs references for the nodes in Unity.
        playerControls = new PlayerController();
        playerControls.Player.SetCallbacks(this); //Initializes all control schemes and phases.
        sidMarshallRigidBody = GetComponent<Rigidbody2D>(); //Initializes the RigidBody component
        sidAnimations = GetComponent<Animator>(); //Initializes the Animator component
        boxCastCollider = GetComponent<BoxCollider2D>(); //Initializes the BoxCollider2D component.
        boxCastSize = boxCastCollider.size; //Sets the size for the BoxCollider2D for slope-checks.
        jumpsLeft = maximumJumps; //Resets the amount of jumps at the start of the game.
    }
    
    //Handles all inputs that subscribe to events when a button press is activated or performed.
    private void OnEnable() {
        playerControls.Enable();
    }

    //Handles all inputs that unsubscribe to events when a button press is deactivated or canceled.
    private void OnDisable() {
        playerControls.Disable();
    }

    //Assigns the movement buttons to the movement mechanic.
    public void OnMovement(InputAction.CallbackContext movement) {
        directionalInput = movement.ReadValue<Vector2>().x; //Reads X-values
    }

    //Assigns the run button to the acceleration mechanic.
    public void OnRun(InputAction.CallbackContext running) {
        runningAction = running.ReadValue<float>(); //Reads the velocity, as a button.
    }

    //Assigns the jump button to the jump mechanic.
    public void OnJump(InputAction.CallbackContext jumping) {
        jumpingAction = jumping.ReadValue<float>(); //Reads the height, as a button.
    }

    //Assigns the jump button to the jump mechanic.
    public void OnCrouch(InputAction.CallbackContext crouching) {
        crouchingAction = crouching.ReadValue<bool>(); //Single button.
    }

    //Assigns the attack button to the jump mechanic.
    public void OnAttack(InputAction.CallbackContext attacking) {
        attackingAction = attacking.ReadValue<bool>(); //Single button.
    }

    //Assigns the door-entering button to scene-change scenarios.
    public void OnDoorEnter(InputAction.CallbackContext doorEntering) {
        doorEnterAction = doorEntering.ReadValue<bool>(); //Single button.
    }

    //Assigns a button-value to confirm something in-game.
    public void OnConfirm(InputAction.CallbackContext confirm) {
        confirmButton = confirm.ReadValue<bool>(); //Single button.
    }
    
    //Assigns a button-value to cancel something in-game.
    public void OnCancel(InputAction.CallbackContext cancel) {
        cancelButton = cancel.ReadValue<bool>(); //Single button.
    }

    //MAIN METHOD: Handles code that runs every frame, such as Animation Logic.
    private void Update() {
        currentSpeed = sidMarshallRigidBody.velocity.x;
        currentHeight = sidMarshallRigidBody.velocity.y;
        previouslyGrounded = characterOnGround();
        /*var ButtonCooldown = 0.5f;
        var ButtonCount = 0;
        var tapAmount = 3;
        //Multi-Tap Detection
        if(Keyboard.current.anyKey.IsPressed()) {
            if(ButtonCooldown > 0 && ButtonCount == 1/(tapAmount - 1)) {
                return;
            } else {
                ButtonCooldown = 0.5f;
                ButtonCount += 1;
            }
        } 
        if(ButtonCooldown > 0) ButtonCooldown -= 1 * Time.deltaTime;
        else ButtonCount = 0;*/
        
        //Basic Movement: Walking and Running (With animation-logic)        
        flipCharacter();
        if(isDiving) return;
        if(!wallJumping) {
            if(characterOnGround()) {
                coyoteCounter = coyoteTime; //Resets the CoyoteCounter. 
                sidAnimations.SetBool("Walled", false);
                sidAnimations.SetBool("Grounded", true);
                performMovement(); //Performs basic movements.
                performCrouchSlide(); //Performs crouch/slide movements.
                characterOnSlope(groundRaySize); //Performs slope movements.
                jumpsLeft = maximumJumps; //Resets the number of jumps.
            } else {
                coyoteCounter -= Time.deltaTime;
                if(currentHeight >= jumpingSpeed || currentHeight < 0) {
                    sidAnimations.SetBool("Grounded", false);
                    sidAnimations.SetFloat("Vertical Velocity", -1.0f); //Falling Logic
                }
            }
        }

        /*if(characterOnGround() && double-tap) {
            StartCoroutine(Dive());
        }*/
        
        if(!characterOnGround() && characterOnwall()) {
            sidAnimations.SetBool("Walled", true);
            sidAnimations.SetFloat("Vertical Velocity", 2.0f); //Wall-Contact Animation
            performWallJump();
        } else {
            sidAnimations.SetBool("Walled", false);
        }

        if(jumpingAction > 0.0f) {
            jumpBufferCounter = jumpBufferTime; //Resets the jump-buffer.
        } else {
            jumpBufferCounter -= Time.deltaTime;
            Debug.Log("Jump Buffer: " + jumpBufferCounter);
        } if(coyoteCounter > 0.0f && jumpBufferCounter >= 0) {
            jumpBufferCounter = 0.0f;
            jumpsLeft -= 1;
            performJump();
        } if(jumpingAction == 0.0f && currentHeight > 0) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, currentHeight * 0.5f); //Variable Jump Height by length of button press.
            coyoteCounter = 0.0f;
        }
    }

    //Method that deals with movement code (including walking, acceleration and friction).
    private void flipCharacter() {
        //Flips the character sprite.
        if(directionalInput > 0) {
            transform.localScale = Vector3.one; //Keeps the vector the same.
        } else if(directionalInput < 0) {
            transform.localScale = new Vector3(-1, 1, 1); //Only changes the X-value of vector 3 (flips the character)
            faceDirectionRight = false;
        }

        //Directional-Changes (Skiddding and Dust Trails).
        float previousDirection = directionalInput;
        if((characterOnGround() || previouslyGrounded) && previousDirection != Mathf.Sign(directionalInput) && (currentSpeed > walkingSpeed || currentSpeed < -walkingSpeed)) {
            sidAnimations.Play("Skid");
            dustParticles.Play();
        }
    }

    //Coroutine Method for the Dive ability.
    private IEnumerator Dive() {
        canDive = false;
        isDiving = true;
        float originalGravity = sidMarshallRigidBody.gravityScale;
        sidMarshallRigidBody.gravityScale = 0.0f; //Gravity is non-existent while diving.
        sidMarshallRigidBody.velocity = new Vector2(transform.localScale.x * divePower, 0.0f);
        sidAnimations.Play("Dive Roll");
        yield return new WaitForSeconds(diveTime);
        sidMarshallRigidBody.gravityScale = originalGravity; //Resets gravity.
        isDiving = false;
        yield return new WaitForSeconds(diveCooldown);
        canDive = true;
    }
    
    private void performMovement() {
        if(directionalInput == 0) {
            sidAnimations.SetFloat("Horizontal Velocity", 0.0f); //Idle Animation Logic
        } if(directionalInput != 0) { //This if-else statement is basically calculating the velocity which we can then substitute in our RigidBody2D component.
            currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, walkingSpeed * directionalInput, acceleration * Time.deltaTime * 10.0f);
            sidAnimations.SetFloat("Horizontal Velocity", 1.0f); //Walking Animation Logic
            if(runningAction != 0 && currentSpeed < runningSpeed) {
                currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, runningSpeed * directionalInput, acceleration * Time.deltaTime * 10.0f);
                sidAnimations.SetFloat("Horizontal Velocity", 2.0f); //Running Animation Logic
            }
        } else if(directionalInput == 0 || (directionalInput == 0 && runningAction == 0)) {
            currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, 0.0f, friction * Time.deltaTime * 10.0f);
        }
        Vector2 currentPosition = new Vector2(currentSpeed, currentHeight);
        sidMarshallRigidBody.velocity = currentPosition; //After all the calculations, the velocity is applied to the RigidBody2D
        if(characterOnGround() && isOnTheSlope) {
            float slopeVelocity = currentSpeed * -directionalInput;
            sidMarshallRigidBody.velocity = new Vector2(slopeVelocity * slopeNormalPerpendicularAngle.x, slopeVelocity * slopeNormalPerpendicularAngle.y);
        }
    }
    
    //Method that deals with jumping code (including Coyote Time, Double Jump and Jump Buffer).
    private void performJump() {
        if(coyoteCounter <= 0.0f && !characterOnwall() && jumpsLeft == 0) return;
        if(characterOnGround() || coyoteCounter > 0.0f || jumpsLeft >= 0) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, jumpingSpeed);
            sidAnimations.SetBool("Grounded", false);

            if(jumpsLeft == 1) sidAnimations.SetFloat("Vertical Velocity", 0.0f); //Single Jump Logic
            else if(jumpsLeft == 0) sidAnimations.SetFloat("Vertical Velocity", 1.0f); //Double Jump Logic
        } if(!characterOnGround() && sidMarshallRigidBody.velocity.y < 0.0f && directionalInput != 0) {
            float midairCourseCorrection = walkingSpeed * directionalInput;
            sidMarshallRigidBody.velocity = new Vector2(midairCourseCorrection, sidMarshallRigidBody.gravityScale);
        }
    }

    //Method that detects if a key press is double-pressed or not
    private void doubleKeyPress(Keyboard buttonInput) {
        /*if(/*buttonInput pressed twice) {
        }*/
    }

    //Method that deals with wall-sliding and wall-jumping.
    private void performWallJump() {
        if(characterOnwall() && !characterOnGround() && directionalInput != 0) {
            sidMarshallRigidBody.velocity = new Vector2(sidMarshallRigidBody.velocity.x, Mathf.Clamp(sidMarshallRigidBody.velocity.y, -wallSlidingSpeed, float.MaxValue));
            sidAnimations.SetFloat("Vertical Velocity", 3.0f); //Wall-Sliding Animation
            wallSliding = true;
        } else {
            wallSliding = false;
        }

        if(wallSliding) {
            wallJumpDirection = -transform.localScale.x; //Flips direction when the jump button is pressed on a wall.
            CancelInvoke(nameof(wallJumpCancel));
            wallJumpCounter = wallJumpTime;
            wallJumping = false;
        } else {
            wallJumpCounter -= Time.deltaTime;
        } 
        
        if(jumpingAction > 0.0f && wallJumpCounter > 0.0f) {
            sidMarshallRigidBody.velocity = new Vector2(wallJumpDirection * wallJumpSpeed.x, wallJumpSpeed.y);
            sidAnimations.SetFloat("Vertical Velocity", 4.0f); //Wall-Kick Animation
            wallJumpCounter = 0.0f;
            wallJumping = true;
            if(transform.localScale.x != wallJumpDirection) {
                faceDirectionRight = !faceDirectionRight;
                Vector3 localScale = transform.localScale;
                transform.localScale = new Vector3(-1, 1, 1);
                transform.localScale = localScale;
            } Invoke(nameof(wallJumpCancel), wallJumpDuration);
        }
    } 
    
    //Method used for Wall-Jump Invoking.
    private void wallJumpCancel() {
        wallJumping = false;
    }

    //Method that deals with crouching and sliding mechanisms.
    private void performCrouchSlide() {
        Vector2 originalBoxCastSize = boxCastSize;
        if(crouchingAction && characterOnGround()) {
            boxCastSize = new Vector2(boxCastCollider.size.x, boxCastCollider.size.y / 2);
            sidAnimations.Play("Crouch");
            if(currentSpeed == runningSpeed) {
                groundSliding = true;
                currentSpeed = Mathf.MoveTowards(slidingSpeed, 0.0f, friction * Time.deltaTime * 10.0f);
                boxCastSize = new Vector2(3.5f, 1.5f);
                sidAnimations.Play("Slide");
            } if(groundSliding && currentSpeed >= 0.01f && jumpingAction > 0) {
                groundSliding = false;
                sidAnimations.Play("Slide Recover");
            }
        } else if(!crouchingAction) {
            groundSliding = false;
            boxCastSize = originalBoxCastSize;
            sidAnimations.Play("Crouch Recover");
        }
    }

    //Method that deals with attacks.
    private void performAttack() {
        if(characterOnGround() && attackingAction && (currentSpeed <= walkingSpeed || currentSpeed >= -walkingSpeed)) {
            sidAnimations.Play("Kick (Stationary)");
            sidMarshallRigidBody.velocity = new Vector2(0.0f, 0.0f);
        }
    }
    
    public void characterOnSlope(Vector2 raySize) {
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

    //Method that checks for the ground.
    public bool characterOnGround() {
        groundRaySize = new Vector2(0.3f, 0.1f);
        collisionDetection = Physics2D.BoxCast(groundCollider.position, groundRaySize, 0.0f, Vector2.down, 0.1f, tileSetLayer);
        return collisionDetection.collider != null;
    }
    
    //Method that checks for the wall.
    public bool characterOnwall() {
        Vector2 collisionDirection;
        if(faceDirectionRight) collisionDirection = Vector2.right;
        else collisionDirection = -Vector2.right;
        
        RaycastHit2D wallCollision = Physics2D.Raycast(wallCollider.position, collisionDirection, rayCastDistance, tileSetLayer);
        return wallCollision.collider != null;
    }
}

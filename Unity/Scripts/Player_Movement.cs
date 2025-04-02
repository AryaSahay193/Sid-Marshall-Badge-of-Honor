using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Player_Movement : MonoBehaviour, PlayerController.IPlayerActions {
    //All public variables can be accessed to other scripts. Private variables are only able to be changed within this script.
    [Header("Reference Variables")]
    [SerializeField] private Rigidbody2D sidMarshallRigidBody; //References the RigidBody2D class as a variable.
    [SerializeField] private PhysicsMaterial2D physicsMaterial;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private Transform groundCollider;
    [SerializeField] private Transform wallCollider;
    [SerializeField] private LayerMask tileSetLayer; //Choose Layers in Unity.
    private BoxCollider2D boxCastCollider; //References the BoxCollider2D class as a variable.
    private Vector2 boxCastSize; //Creates a Vector2 for a Box Cast Size.
    private bool faceDirectionRight = true; //By default
    
    [Header("Ground Detection")]
    RaycastHit2D collisionDetection;
    private Vector2 groundRaySize, slopeNormalPerpendicularAngle;
    private float rayCastDistance = 0.25f, slopeCurrentAngle, slopePreviousAngle;
    private bool isOnTheSlope;
    
    [Header("Movement Variables")]
    public float currentSpeed, currentHeight;
    [SerializeField] private float acceleration;
    [SerializeField] private float friction;
    [SerializeField] public float walkingSpeed;
    [SerializeField] public float runningSpeed;
    [SerializeField] private float wallAcceleration;
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
    public int jumpsLeft, maximumJumps = 2;
    
    [Header("Input-System References")]
    private PlayerController playerControls;
    public bool doublePress, attackingAction, crouchingAction, divingAction, doorEnterAction, confirmButton, cancelButton;
    public float directionalInput, runningAction, jumpingAction;

    private void Awake() { //Everytime the script starts, the code runs with the Awake method.
        //Grabs references for the nodes in Unity.
        playerControls = new PlayerController();
        playerControls.Player.SetCallbacks(this); //Initializes all control schemes and phases.
        sidMarshallRigidBody = GetComponent<Rigidbody2D>(); //Initializes the RigidBody component
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

    public void OnDive(InputAction.CallbackContext diving) {
        divingAction = diving.ReadValue<bool>(); //Single button.
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

    public void Process(ref InputInteractionContext multiTap) {
        var controls = multiTap.control;
        if(inputControls != null) {
            if(inputControls != controls) return;
            var isStillActuated = multiTap.ControlIsActuated(0.75f);
            var actuationTime = multiTap.time - multiTap.startTime;
            if(!isStillActuated) {
                if(actuationTime >= 1) multiTap.Performed();
                else multiTap.Canceled();
            }
        } else {
            var isActuated = multiTap.ControlIsActuated(0.75f);
            if(isActuated) {
                inputControls = multiTap.control;
                multiTap.Started();
            }
        }
    }

    InputControl inputControls;
    public void Reset() {
        inputControls = null;
    }

    //MAIN METHOD: Handles code that runs every frame, such as Animation Logic.
    public void Update() {
        currentSpeed = sidMarshallRigidBody.velocity.x;
        currentHeight = sidMarshallRigidBody.velocity.y;
        previouslyGrounded = characterOnGround();
        
        //Basic Movement: Walking and Running (With animation-logic)        
        flipCharacter();
        if(isDiving) return;
        if(!wallJumping) {
            if(characterOnGround()) {
                performMovement(); //Performs basic movements.
                performCrouchSlide(); //Performs crouch/slide movements.
                characterOnSlope(groundRaySize); //Performs slope movements.
                coyoteCounter = coyoteTime; //Resets the CoyoteCounter. 
                jumpsLeft = maximumJumps; //Resets the number of jumps.
            } else coyoteCounter -= Time.deltaTime;
        } if(divingAction) StartCoroutine(Dive());

        //Jump-Buffer and Jump-Conditions
        if(jumpingAction > 0.0f) {
            jumpBufferCounter = jumpBufferTime; //Resets the jump-buffer.
        } else {
            jumpBufferCounter -= Time.deltaTime;
        } if(coyoteCounter > 0.0f && jumpBufferCounter >= 0) {
            jumpBufferCounter = 0.0f;
            jumpsLeft -= 1;
            performJump();
        } if(jumpingAction == 0.0f && currentHeight > 0) {
            sidMarshallRigidBody.velocity = new Vector2(currentSpeed, currentHeight * 0.5f); //Variable Jump Height by length of button press.
            coyoteCounter = 0.0f;
        }

        //Course-correction while falling.
        float horizontalFallSpeed = 5.0f;
        float pointOfFallAdjustment = -3.0f;
        float midairCourseCorrection = horizontalFallSpeed * directionalInput;
        if(!characterOnGround() && currentHeight <= pointOfFallAdjustment && currentSpeed <= runningSpeed) {
            if(directionalInput != 0.0f) {
                float midairSpeed = Mathf.MoveTowards(currentSpeed, midairCourseCorrection, acceleration * Time.deltaTime * 10.0f);
                sidMarshallRigidBody.velocity = new Vector2(midairSpeed, currentHeight);
            } else if(directionalInput == 0.0f || directionalInput != Mathf.Abs(directionalInput)) {
                float zeroFallMovement = Mathf.MoveTowards(currentSpeed, 0.0f, (friction * Time.deltaTime * 2.5f));
                sidMarshallRigidBody.velocity = new Vector2(zeroFallMovement, currentHeight);
            }
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
        yield return new WaitForSeconds(diveTime);
        sidMarshallRigidBody.gravityScale = originalGravity; //Resets gravity.
        isDiving = false;
        yield return new WaitForSeconds(diveCooldown);
        canDive = true;
    }
    
    private void performMovement() {
        if(directionalInput != 0) { //This if-else statement is basically calculating the velocity which we can then substitute in our RigidBody2D component.
            currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, walkingSpeed * directionalInput, acceleration * Time.deltaTime * 10.0f);
            if(runningAction != 0 && currentSpeed < runningSpeed) {
                currentSpeed = Mathf.MoveTowards(sidMarshallRigidBody.velocity.x, runningSpeed * directionalInput, acceleration * Time.deltaTime * 10.0f);
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
        } if(!characterOnGround() && sidMarshallRigidBody.velocity.y < 0.0f && directionalInput != 0) {
            float midairCourseCorrection = walkingSpeed * directionalInput;
            sidMarshallRigidBody.velocity = new Vector2(midairCourseCorrection, sidMarshallRigidBody.gravityScale);
        }
    }

    //Method that deals with wall-sliding and wall-jumping.
    private void performWallJump() {
        if(characterOnwall() && !characterOnGround() && directionalInput != 0) {
            sidMarshallRigidBody.velocity = new Vector2(sidMarshallRigidBody.velocity.x, Mathf.Clamp(sidMarshallRigidBody.velocity.y, -wallSlidingSpeed, float.MaxValue));
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
            if(currentSpeed == runningSpeed) {
                groundSliding = true;
                currentSpeed = Mathf.MoveTowards(slidingSpeed, 0.0f, friction * Time.deltaTime * 10.0f);
                boxCastSize = new Vector2(3.5f, 1.5f);
            } if(groundSliding && currentSpeed >= 0.01f && jumpingAction > 0) {
                groundSliding = false;
            }
        } else if(!crouchingAction) {
            groundSliding = false;
            boxCastSize = originalBoxCastSize;
        }
    }

    //Method that deals with attacks.
    private void performAttack() {
        if(characterOnGround() && attackingAction && (currentSpeed <= walkingSpeed || currentSpeed >= -walkingSpeed)) {
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

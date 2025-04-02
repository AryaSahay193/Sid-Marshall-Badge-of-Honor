using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animations : MonoBehaviour {
    
    [Header("Basic References")]
    private Player_Movement playerMovementScript;
    private PlayerController playerControls;
    private Rigidbody2D sidMarshallRigidBody;
    private Animator sidAnimations;
    
    [Header("Animation Attributes")]
    private float inputDirections, currentSpeed, currentHeight, animationDuration, isJumping, isRunning, walkingSpeed, runningSpeed;
    private bool isCharacterGrounded, isCharacterWalled, isDiving, isAttacking, isCrouching, isSliding;
    private string currentAnimationState;
    
    void Start() {
        //Basic setting up.
        playerMovementScript = GetComponent<Player_Movement>();
        playerControls = GetComponent<PlayerController>();
        isCharacterGrounded = playerMovementScript.characterOnGround();
        isCharacterWalled = playerMovementScript.characterOnwall();
        sidMarshallRigidBody = GetComponent<Rigidbody2D>();
        sidAnimations = GetComponent<Animator>();
        animationDuration = sidAnimations.GetCurrentAnimatorStateInfo(0).length;
        
        //Booleans that help with playing animations.
        inputDirections = playerMovementScript.directionalInput; //Float
        isAttacking = playerMovementScript.attackingAction; //Boolean
        isCrouching = playerMovementScript.crouchingAction; //Boolean
        isDiving = playerMovementScript.divingAction; //Boolean
        isJumping = playerMovementScript.jumpingAction; //Float
        isRunning = playerMovementScript.runningAction; //Float

        //Values that can help with animation type.
        walkingSpeed = playerMovementScript.walkingSpeed;
        runningSpeed = playerMovementScript.runningSpeed;
        currentSpeed = playerMovementScript.currentSpeed;
        currentHeight = playerMovementScript.currentHeight;
    }

    void Update() {
        if(isCharacterGrounded) {
            if(inputDirections != 0.0f || inputDirections == 0.0f) {
                if(currentSpeed == 0.0f) playAnimations("Idle");
                else if(currentSpeed != 0.0f) playAnimations("Walk");
                else if(isRunning != 0.0f && currentSpeed <= runningSpeed) playAnimations("Run");
            }

            if(currentSpeed <= walkingSpeed || currentSpeed >= -walkingSpeed) {
                if(isCrouching) playAnimations("Crouch");
                else if(!isCrouching) playAnimations("Crouch Recover");
            }
        } 
        
        if(!isCharacterGrounded) {
            float highFallHeight = -50.0f;
            if(currentHeight < 0.0f && currentHeight >= highFallHeight) playAnimations("Fall"); 
            else playAnimations("Fall (High)");
        } 
        
        if(isCharacterWalled) {
        }
    }

    //Method that handles animations
    private void playAnimations(string nameOfAnimation) {
        if(currentAnimationState == nameOfAnimation) return;
        sidAnimations.Play(nameOfAnimation);
        currentAnimationState = nameOfAnimation;
    }
}

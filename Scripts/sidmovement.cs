using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SidMovement : MonoBehaviour {
    //[SerializeField] allows you to directly edit a numerical value into Unity.
    [SerializeField] private float walkingSpeed; //Edit walk speed in Unity.
    [SerializeField] private float runningSpeed; //Edit run speed in Unity.
    [SerializeField] private float jumpingSpeed; //Edit jump height in Unity.
    [SerializeField] public LayerMask tileSetLayer; //Choose Layers in Unity.
    private Vector2 rayCastSize = new Vector2(0.5f, 0.2f); //Creates the size of the BoxCast
    private float rayCastDistance = 0.3f; //Determines how far the RayCast detects the ground.
    private float horizontalMovement;
    private bool grounded;
    
    //Reference variables
    [SerializeField] private Rigidbody2D sidMarshallRigidBody; //References the RigidBody2D class as a variable.
    private Animator sidAnimations; //References the Animator class as a variable.
    private BoxCollider2D boxCastCollision; //References the BoxCollider2D class as a variable.

    private void Awake() { //Everytime the script starts, the code runs with the Awake method.
        //Grabs references for the nodes in Unity.
        sidMarshallRigidBody = GetComponent<Rigidbody2D>();
        sidAnimations = GetComponent<Animator>();
        boxCastCollision = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate() {
        sidMarshallRigidBody.velocity = new Vector2(horizontalMovement * walkingSpeed, sidMarshallRigidBody.velocity.y); //You can change the velocity of X, but Y stays the same.
        sidAnimations.SetFloat("Horizontal Velocity", Math.Abs(sidMarshallRigidBody.velocity.x));
        sidAnimations.SetFloat("Vertical Velocity", sidMarshallRigidBody.velocity.y);
        //Flips the character sprite.
        if(horizontalMovement > 0) {
            transform.localScale = Vector3.one; //Keeps the vector the same.
        } else if(horizontalMovement < 0) {
            transform.localScale = new Vector3(-1, 1, 1); //Only changes the X-value of vector 3 (flips the character)
        }
    }

    public void Jump(InputAction.CallbackContext jump) {
        sidAnimations.SetBool("Jump", !grounded);
        if(jump.performed && groundChecker()) {
            sidAnimations.SetTrigger("Jump");
            sidMarshallRigidBody.velocity = new Vector2(sidMarshallRigidBody.velocity.x, jumpingSpeed);
        } if(jump.canceled && sidMarshallRigidBody.velocity.y > 0.0f) {
            sidMarshallRigidBody.velocity = new Vector2(sidMarshallRigidBody.velocity.x, sidMarshallRigidBody.velocity.y * 0.5f);
        }
    }

    public void Movement(InputAction.CallbackContext movement) {
        horizontalMovement = movement.ReadValue<Vector2>().x; //Reads on the x-values
    }

    public void Accelerate(InputAction.CallbackContext accelerate) {
        if(accelerate.performed && horizontalMovement != 0) {
            sidMarshallRigidBody.velocity = new Vector2(runningSpeed, sidMarshallRigidBody.velocity.y);
            sidAnimations.SetBool("Run", horizontalMovement != 0 && accelerate.performed);
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

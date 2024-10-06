using UnityEngine;
using UnityEngine.InputSystem;

public class SidMovement : MonoBehaviour {
    //[SerializeField] allows you to directly edit a numerical value into Unity.
    private float walkingSpeed = 3.5f;
    private float runningSpeed = 7.0f;
    private float jumpingSpeed = 20.0f;
    private float numberOfJumps; //Number of jumps in the current state.
    private float maximumJumps = 2; //The maximum number of jumps allowed.
    private bool ground;
    private bool doubleJump;
    
    //Reference variables
    private Rigidbody2D sidMarshallRigidBody;
    private Animator sidAnimations;
    private InputActionReference sidButtonInputs;

    private void Awake() { //Everytime the script starts, the code runs with the Awake method.
        //Grabs references for the nodes in Unity.
        sidMarshallRigidBody = GetComponent<Rigidbody2D>();
        sidAnimations = GetComponent<Animator>();
    }

    private void Update() { //Code exacutes on every frame instead of starting immediately on awake.
        float horizontalaxis = Input.GetAxis("Horizontal");
        float verticalaxis = Input.GetAxis("Vertical");
        sidMarshallRigidBody.velocity = new Vector2(horizontalaxis * walkingSpeed, sidMarshallRigidBody.velocity.y); //You can change the velocity of X, but Y stays the same.
        if(Input.GetButton("Jump") && numberOfJumps > 0) {
            Jump(); //Calls the method for jumping.
            if(ground && numberOfJumps == 0) {
                numberOfJumps = maximumJumps; //Resets the jump-counter back to 2.
            }
        }

        if(horizontalaxis !=0 && Input.GetKey(KeyCode.LeftShift)) {
            sidMarshallRigidBody.velocity = new Vector2(horizontalaxis * runningSpeed, sidMarshallRigidBody.velocity.y);
        }

        //Flips the character sprite.
        if(horizontalaxis > 0) {
            transform.localScale = Vector3.one;
        } else if(horizontalaxis < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //Animation Logic
        sidAnimations.SetBool("Idle", horizontalaxis == 0);
        sidAnimations.SetBool("Walk", horizontalaxis != 0);
        sidAnimations.SetBool("Run", horizontalaxis != 0 && Input.GetKey(KeyCode.LeftShift));
        sidAnimations.SetBool("Ground", ground); //Sets boolean for jumping.
    }

    private void Jump() {
        sidMarshallRigidBody.velocity = new Vector2(sidMarshallRigidBody.velocity.x, jumpingSpeed);
        sidAnimations.SetTrigger("Jump");
        ground = false; //When the player jumps, they are not on the ground anymore, which is why now it's false.
        numberOfJumps -= 1;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Ground") {
            ground = true; //Ensures that the player is on the ground.
        }
    }
}

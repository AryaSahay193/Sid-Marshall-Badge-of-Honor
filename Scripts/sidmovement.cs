using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidMovement : MonoBehaviour {
    [SerializeField] private float walkingSpeed; //Allows you to change Walking Speed directly into Unity.
    [SerializeField] private float jumpingSpeed; //Allows you to change Jumping Speed directly into Unity.
    private Rigidbody2D sidMarshallRigidBody; //Creates a variable that references RigidBody2D

    private void Awake() {
        //Everytime the script starts, the code runs with the Awake method.
        sidMarshallRigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update() { //Code exacutes on every frame instead of starting immediately on awake.
        float horizontalaxis = Input.GetAxis("Horizontal");
        float verticalaxis = Input.GetAxis("Vertical");
        sidMarshallRigidBody.velocity = new Vector2(horizontalaxis * walkingSpeed, sidMarshallRigidBody.velocity.y); //You can change the velocity of X, but Y stays the same.
        if(Input.GetKey(KeyCode.Space)) {
            sidMarshallRigidBody.velocity = new Vector2(sidMarshallRigidBody.velocity.x, jumpingSpeed);
        }

        //Flips the character sprite.
        if(horizontalaxis > 0) {
            transform.localScale = Vector3.one;
        } else if(horizontalaxis < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}

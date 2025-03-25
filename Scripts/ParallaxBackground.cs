using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour {
    private float length, startingPosition;
    public float parallaxEffect;
    public GameObject gameCamera;

    // Start is called before the first frame update
    void Start() {
        startingPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate() {
        float cameraMovement = (gameCamera.transform.position.x * (1 - parallaxEffect));
        float backgroundDistance = (gameCamera.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startingPosition + backgroundDistance, transform.position.y, transform.position.z);

        if(cameraMovement > startingPosition + length) startingPosition += length;
        else if(cameraMovement < startingPosition - length) startingPosition -= length; 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFloating : MonoBehaviour {

    float distFromGround = 1.5f;
    Vector3 groundHeight;
    float minDist = 0.5f;
    float maxDist = 0.5f;
    float speed = 1f;
    Vector3 minPosition;
    Vector3 maxPosition;
    Vector3 vel;


    bool up = true;

    bool falling = true;

    Vector3 ground;

    private void Awake () {
        falling = true;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
            groundHeight = hit.point;
            groundHeight.y += distFromGround;
        } else {
            groundHeight = transform.position;
            groundHeight.y = 0 + distFromGround;
        }

        minPosition = groundHeight - new Vector3(0, minDist, 0);
        maxPosition = groundHeight + new Vector3(0, maxDist, 0);
        vel = new Vector3(0, speed, 0);
    }


    private void Update () {
        if (falling) {
            transform.position = Vector3.Lerp(transform.position, groundHeight, 0.1f);

            if (transform.position.y < groundHeight.y + maxDist) {
                falling = false;
            }

        } else {

            if (up) {
                transform.position += vel * Time.deltaTime;
                up = transform.position.y <= maxPosition.y;
            } else {
                transform.position -= vel * Time.deltaTime;
                up = transform.position.y <= minPosition.y;
            }
            

        }


    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
    
    private float tumbleSpeed;

    void Start() {
        tumbleSpeed = Random.Range(0.075f, 0.25f);
        gameObject.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumbleSpeed;
    }
}

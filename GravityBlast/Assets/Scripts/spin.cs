using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    Rigidbody rb;
	float rotationAngle = 0.0f;
	
	// Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rotationAngle += 180.0f * Time.fixedDeltaTime;
		rb.AddTorque(Vector3.up * rotationAngle * rb.mass);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : MonoBehaviour
{
	[SerializeField] private float orbitSpeed = 15.0f;
	[SerializeField] private Vector3 orbitDirection = Vector3.right;
	
	[SerializeField] private float spinSpeed = 0.02f;
	[SerializeField] private Vector3 spinDirection = Vector3.up;

    // Update is called once per frame
	void Update()
    {
        //Vector3 orbitCenter = transform.TransformDirection(transform.parent.position);
		
		//transform.InverseTransformDirection(transform.position);
		
		//transform.TransformDirection(transform.position);
		
		//Vector3 orbitCenter = transform.parent.position - transform.InverseTransformPoint(transform.position);
		
		//transform.TransformPoint(Vector3.zero);
		
		//Vector3 orbitCenter = transform.parent.position - transform.localPosition;
		
		//Vector3 orbitCenter = transform.parent.position - transform.position;
		
		
		
		
		
		Vector3 orbitCenter = transform.parent.position;
		
		//orbitDirection = transform.TransformDirection(orbitDirection);
		
		//Vector3 orbitRotation = transform.parent.InverseTransformDirection(orbitDirection) - transform.localEulerAngles;
		
		
		
		
		//Vector3 orbitRotation = transform.parent.transform.InverseTransformPoint(transform.position);
		
		
		//orbitDirection = transform.parent.right;
		
		
		Vector3 orbitRotation = orbitDirection;
		
		transform.RotateAround(orbitCenter, orbitRotation, orbitSpeed * Time.deltaTime);
		
		transform.Rotate(spinDirection * spinSpeed, Space.Self);
		//transform.RotateAround(transform.parent.position, orbitDirection, orbitSpeed * Time.deltaTime);
		//transform.RotateAround(orbitCenter, orbitDirection.normalized, orbitSpeed * Time.deltaTime);
		
		
    }
}

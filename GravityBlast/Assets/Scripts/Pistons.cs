using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistons : MonoBehaviour
{
	[SerializeField] private Transform[] pistons;
	
	private Vector3 upperEuler;
	private Vector3 lowerEuler;
	
	private Quaternion upperLookDirection;
	private Quaternion lowerLookDirection;
	
	[SerializeField] private Vector3 upperLookOffset;
	[SerializeField] private Vector3 lowerLookOffset;

/*
	void Update()
    {		
		// Upper Piston		
		upperEuler = pistons[1].position - pistons[0].position;
		upperEuler.x = 0;
		upperLookDirection = Quaternion.LookRotation(upperEuler);
		upperLookDirection = Quaternion.Euler(upperLookDirection.eulerAngles + upperLookOffset);
		
		pistons[0].rotation = Quaternion.Slerp(pistons[0].rotation, upperLookDirection, 1.0f);
		
		// Lower Piston
		lowerEuler = pistons[0].position - pistons[1].position;
		//lowerEuler = pistons[1].InverseTransformDirection(lowerEuler); // Convert to local space
		lowerEuler.x = 0;
		lowerLookDirection = Quaternion.LookRotation(lowerEuler);
		lowerLookDirection = Quaternion.Euler(lowerLookDirection.eulerAngles + lowerLookOffset);
		
		pistons[1].rotation = Quaternion.Slerp(pistons[1].rotation, lowerLookDirection, 1.0f);
    }
*/

    void Update()
    {		
		// Upper Piston		
		upperEuler = new Vector3(pistons[0].position.x, pistons[1].position.y, pistons[1].position.z);
		//pistons[0].LookAt(upperEuler);
		
		upperLookDirection = Quaternion.LookRotation(upperEuler);
		upperLookDirection = Quaternion.Euler(upperLookDirection.eulerAngles + upperLookOffset);
		pistons[0].rotation = Quaternion.Slerp(pistons[0].rotation, upperLookDirection, 1.0f);
		
		// Lower Piston		
		lowerEuler = new Vector3(pistons[1].position.x, pistons[0].position.y, pistons[0].position.z);
		//pistons[1].LookAt(lowerEuler);
		
		lowerLookDirection = Quaternion.LookRotation(lowerEuler);
		lowerLookDirection = Quaternion.Euler(lowerLookDirection.eulerAngles + lowerLookOffset);
		pistons[1].rotation = Quaternion.Slerp(pistons[1].rotation, lowerLookDirection, 1.0f);
    }
}

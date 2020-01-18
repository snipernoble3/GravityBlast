using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		other.attachedRigidbody.AddForce((-transform.forward * 100) * other.attachedRigidbody.mass, ForceMode.Impulse);
		/*
		if (enter)
        {
            
        }
		*/		
	}
}

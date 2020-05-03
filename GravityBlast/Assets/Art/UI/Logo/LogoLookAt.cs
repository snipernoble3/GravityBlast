using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoLookAt : MonoBehaviour
{
    public Transform logoCamera;
	Quaternion lookAtCamera = Quaternion.identity;

    // Update is called once per frame
    void Update()
    {
		transform.rotation = Quaternion.LookRotation(logoCamera.position - transform.position);
    }
}

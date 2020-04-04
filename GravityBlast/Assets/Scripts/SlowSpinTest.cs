using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowSpinTest : MonoBehaviour
{
    [SerializeField] Vector3 rotationVector;
	
    void Update()
    {
          transform.Rotate(rotationVector * Time.deltaTime, Space.Self);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeSpin : MonoBehaviour
{
    public float rotationSpeed;
	public float precessionSpeed;
	public float precessionAmount;
	
	//private Vector3 originalRotation;
	//private Quaternion originalRotation;
	
	private Vector3 newRotation = Vector3.zero;
	
	//private Transform offset;
	
	
    void Awake()
	{
		//offset = new GameObject().transform;
		//offset.position = transform.position;
		//offset.rotation = transform.rotation;
		//offset.localScale = transform.localScale;
		//offset.parent = null;
		
		//originalRotation = transform.localEulerAngles;
		//originalRotation = transform.eulerAngles;
		
		//originalRotation = transform.localRotation;
	}
	
	void Update()
    {
		newRotation.x = Mathf.Sin(precessionSpeed * Time.time) * precessionAmount;
		newRotation.y = Mathf.Repeat(newRotation.y + rotationSpeed * Time.deltaTime, 360.0f);
		newRotation.z = Mathf.Cos(precessionSpeed * Time.time) * precessionAmount;
		
		//transform.localRotation = Quaternion.Euler(new Vector3(originalRotation.x, rotationAngle, originalRotation.y));
		
		//transform.localRotation = Quaternion.Euler(originalRotation + new Vector3(0.0f, rotationAngle, 0.0f));
		
		//transform.localRotation = Quaternion.Euler(originalRotation + newRotation);
		
		//newRotation += transform.TransformDirection(originalRotation);
		
		//newRotation = transform.InverseTransformDirection(newRotation);
		
		//newRotation += originalRotation;
		
		//transform.localRotation = Quaternion.Euler(newRotation);
		
		//transform.rotation = Quaternion.Euler(newRotation) * originalRotation;
		
		//transform.localRotation = Quaternion.Euler(newRotation) * originalRotation;
		
		//newRotation = offset.InverseTransformDirection(newRotation);
		
		//newRotation = offset.InverseTransformVector(newRotation);
		
		transform.localRotation = Quaternion.Euler(newRotation);
    }
}

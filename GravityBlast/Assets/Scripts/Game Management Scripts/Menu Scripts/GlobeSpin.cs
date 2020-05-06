using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeSpin : MonoBehaviour
{
    public float rotationSpeed = 45.0f;
	public float precessionSpeed = 2.0f;
	public float precessionAmount = 4.0f;
	
	private Quaternion originalRotation;
	private Vector3 angles;
	
    void Awake()
	{
		UpdateAlignment();
	}
	
	public void UpdateAlignment()
	{
		originalRotation = transform.rotation;
	}
	
	void Update()
    {
		angles.x = Mathf.Sin(precessionSpeed * Time.time) * precessionAmount;
		angles.y = Mathf.Repeat(angles.y + rotationSpeed * Time.deltaTime, 360.0f);
		angles.z = Mathf.Cos(precessionSpeed * Time.time) * precessionAmount;

		transform.rotation = originalRotation;
		transform.rotation *= Quaternion.Euler(new Vector3(angles.x, 0.0f, angles.z));
		transform.Rotate(0.0f, angles.y, 0.0f, Space.Self);
    }
}
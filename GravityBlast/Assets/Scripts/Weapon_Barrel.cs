using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Barrel : MonoBehaviour
{
    [SerializeField] private Vector3 spinAxis = Vector3.forward;
	[SerializeField] private bool spinClockwise = true;
	
	private Quaternion originalRotation;
	private Quaternion targetRotation;
	private float spinTarget;
	
	
	public float rotationAmount = 1000.0f;
	
	public float windUpRate = 3.0f;
	public float windDownRate = -1.0f;
	private float WindUpDownMultiplier = 0.0f;
	
	public float fireRateMultiplier = 1.0f;
	
	bool shouldWindDown = true;
	
    void Start()
    {
        originalRotation = transform.rotation;
		spinTarget = originalRotation.y;
    }

    void Update()
    {
        // Wind down the barrel
		if (shouldWindDown && WindUpDownMultiplier > 0.0f)
		{
			WindUpDownMultiplier = Mathf.Clamp(WindUpDownMultiplier + windDownRate * Time.deltaTime, 0.0f, 1.0f);
			SetTargetRotation();
		}
		else shouldWindDown = true;
		
		// Spin the barrel
		if (transform.rotation != targetRotation)
		{
			transform.localRotation = Quaternion.Slerp(transform.rotation, targetRotation, 1.0f);
		}
    }
	
	public void Spin()
	{
		shouldWindDown = false;	
		if (WindUpDownMultiplier < 1.0f) WindUpDownMultiplier = Mathf.Clamp(WindUpDownMultiplier + windUpRate * Time.deltaTime, 0.0f, 1.0f);
		SetTargetRotation();
	}
	
	private void SetTargetRotation()
	{	
		spinTarget += rotationAmount * fireRateMultiplier * WindUpDownMultiplier * Time.deltaTime * (spinClockwise ? -1.0f : 1.0f);
		if (spinTarget < 0.0f) spinTarget += 360.0f;
		if (spinTarget >= 360.0f) spinTarget -= 360.0f;
		
		targetRotation = Quaternion.Euler(spinAxis * spinTarget);
		//targetRotation = Quaternion.Euler(0.0f, spinTarget, 0.0f);
	}
}

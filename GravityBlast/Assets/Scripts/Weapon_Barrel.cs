using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Barrel : MonoBehaviour
{
    [SerializeField] private Vector3 spinAxis = -Vector3.forward;
	[SerializeField] private int numOfBarrels = 3;
	
	private Quaternion targetRotation;
	private float spinAngle = 0.0f;
	private float angleOfNearestBarrel = 0.0f;
	private bool stoppingPointCalculated = false;
	
	public float rotationAmount = 1000.0f;
	private float remainingAngle = 0.0f;
	private float barrelResetSpeed = 1.0f;
	
	public float windUpRate = 3.0f;
	public float windDownRate = -1.0f;
	private float WindUpDownMultiplier = 0.0f;
	
	private float MinWindDownSpeed = 0.2f;
	
	public float fireRateMultiplier = 1.0f;
	
	private bool isWindingDown = true;
	

    void Update()
    {
		// Wind down the barrel
		if (isWindingDown && WindUpDownMultiplier > 0.0f)
		{
			// Subtract from the WindUpDownMutliplier over time until it hits the min spin down speed.
			WindUpDownMultiplier = Mathf.Clamp(WindUpDownMultiplier + windDownRate * Time.deltaTime, MinWindDownSpeed, 1.0f);
			SetTargetRotation();
		}
		else isWindingDown = true;
		
		// Spin the barrel to match the targetRotation if its not already there.
		if (transform.rotation != targetRotation) transform.localRotation = targetRotation;
    }
	
	public void Spin()
	{
		stoppingPointCalculated = false;
		isWindingDown = false;
		WindUpDownMultiplier = Mathf.Clamp(WindUpDownMultiplier + windUpRate * Time.deltaTime, 0.0f, 1.0f);
		SetTargetRotation();
	}
	
	private void SetTargetRotation()
	{	
		float angleOfChange = rotationAmount * fireRateMultiplier * WindUpDownMultiplier * Time.deltaTime;
		spinAngle += angleOfChange;
		if (spinAngle < 0.0f) spinAngle += 360.0f;
		if (spinAngle >= 360.0f) spinAngle -= 360.0f;
		
		if (isWindingDown && WindUpDownMultiplier == MinWindDownSpeed)
		{
			if (!stoppingPointCalculated)
			{
				angleOfNearestBarrel = 0.0f;
				while (angleOfNearestBarrel < 360.0f)
				{
					if (angleOfNearestBarrel > spinAngle) break;
					angleOfNearestBarrel += (360.0f / numOfBarrels);
				}
				remainingAngle = angleOfNearestBarrel - spinAngle;
				if (angleOfNearestBarrel > 360.0f) angleOfNearestBarrel -= 360.0f; // Fix the wrap around issue.
				stoppingPointCalculated = true;
			}
			else
			{
				if (remainingAngle - angleOfChange > 0.0f)
				{
					remainingAngle -= angleOfChange;
				}
				else
				{
					spinAngle = angleOfNearestBarrel;
					WindUpDownMultiplier = 0.0f;
				}
			}
		}

		targetRotation = Quaternion.Euler(spinAxis * spinAngle); // Build the target rotation.
	}
}
	
/*
	private void resetToNearestBarrel()
	{
		if (!stoppingPointCalculated)
		{
			//angleOfNearestBarrel = ((int)spinAngle / (360 / numOfBarrels)) * (360 / numOfBarrels);
			angleOfNearestBarrel = (Mathf.Floor(spinAngle) / (360 / numOfBarrels)) * (360 / numOfBarrels);
			stoppingPointCalculated = true;
		}	

		//spinAngle = Mathf.Lerp(spinAngle, angleOfNearestBarrel, 1.0f - WindUpDownMultiplier);
		spinAngle = Mathf.Lerp(spinAngle, angleOfNearestBarrel, barrelResetSpeed * Time.deltaTime);
		targetRotation = Quaternion.Euler(spinAxis * spinAngle);
		
		//targetRotation = Quaternion.Slerp(targetRotation, Quaternion.Euler(spinAxis * angleOfNearestBarrel), barrelResetSpeed * Time.deltaTime);
	}
*/
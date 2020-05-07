using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Barrel : MonoBehaviour
{
    [SerializeField] private Vector3 spinAxis = -Vector3.forward; // Use negative or positive to spin clockwise / counter clockwise respectively.
	[SerializeField] private int numOfBarrels = 3; // The number of barrels on the gun, used for figuring out the stopping position.
	public float rotationAmount = 1000.0f; // The base speed multiplier for the barrel spin.
	public float fireRateMultiplier = 1.0f;
	
	private Quaternion targetRotation; // The quaternion rotation that will be built each frame.
	private float spinAngle = 0.0f; // The angle that the barrel will be set to each frame.
	private float angleOfNearestBarrel = 0.0f; // This will be calculated depending on the number of barrels (default 3 = 120 degrees)
	
	
	private float MinWindDownSpeed = 0.2f;
	private bool stoppingPointCalculated = false; // set to false when we have yet to determine which barrel the spinning will stop on.
	private float remainingAngle = 0.0f; // How many degrees are left before the barrel has rotated to a valid stopping point.
	
	private bool isWindingDown = true;
	public float windUpRate = 3.0f;
	public float windDownRate = -1.0f;
	private float WindUpDownMultiplier = 0.0f;
	
	public Material barrelMat;
	private float overheat = 0.0f;
	private float overheatRate = 0.2f;
	private float coolOffRate = 1.2f;

    void Start()
	{
		//barrelMat = GetComponent<Renderer>().material;
	}
	
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
		
		if (WindUpDownMultiplier == 0.0f) overheat = Mathf.Clamp (overheat - coolOffRate * Time.deltaTime, 0.0f, 1.0f);
		barrelMat.SetFloat("_OverheatBlend", overheat);
    }
	
	public void Spin() // Called from the shooting script.
	{
		stoppingPointCalculated = false;
		isWindingDown = false;
		WindUpDownMultiplier = Mathf.Clamp(WindUpDownMultiplier + windUpRate * Time.deltaTime, 0.0f, 1.0f);
		SetTargetRotation();
		
		overheat = Mathf.Clamp (overheat + overheatRate * Time.deltaTime, 0.0f, 1.0f);
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
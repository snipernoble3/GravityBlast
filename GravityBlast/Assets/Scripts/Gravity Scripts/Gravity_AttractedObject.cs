using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Gravity_AttractedObject : MonoBehaviour
{
	[SerializeField] private Gravity_Source gravitySource;
	//public float blendToNewSource = 1.0f;
	//private float blendSpeed = 0.025f;
	public bool rotateToGravitySource = true; // Keep this on for important objects like characters, off for more preformance (for things like bullets).
	
	public bool isChangingSource = false;
	private const float sourceChangeDuration = 5.0f;
	private float timeSinceSourceChange;

    [HideInInspector] public bool paused;

    void Awake()
    {
		if (gravitySource == null && Gravity_Source.DefaultGravitySource != null) gravitySource = Gravity_Source.DefaultGravitySource;
		
		// If we are sourcing gravity from a plant object, then don't apply Unity's default gravity.
		if (gravitySource != null) GetComponent<Rigidbody>().useGravity = false;
		else GetComponent<Rigidbody>().useGravity = true;
		timeSinceSourceChange = sourceChangeDuration; // Set the timer to complete.
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (timeSinceSourceChange != sourceChangeDuration) timeSinceSourceChange = Mathf.Clamp(timeSinceSourceChange + Time.fixedDeltaTime, 0.0f, sourceChangeDuration);
		
		//if (blendToNewSource != 1.0f) blendToNewSource = Mathf.Clamp(blendToNewSource + (blendSpeed * Time.fixedDeltaTime), 0.0f, 1.0f);
        //if (gravitySource != null) gravitySource.AttractObject(transform, blendToNewSource, rotateToGravitySource);
		
		// Convert the timer to a 0-1 value.
		float timeBasedBlend = Mathf.InverseLerp(0.0f, sourceChangeDuration, timeSinceSourceChange);
		
		if (gravitySource != null && !paused) gravitySource.AttractObject(transform, timeBasedBlend, rotateToGravitySource, isChangingSource);
    }
	
	public void SetGravitySource(Gravity_Source gravitySource)
	{
		isChangingSource = true;
		timeSinceSourceChange = 0.0f;
		
		this.gravitySource = gravitySource;
		GetComponent<Rigidbody>().useGravity = false;
	}

    public Gravity_Source GetGravitySource()
	{
		return gravitySource;
	}
}

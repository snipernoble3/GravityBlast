using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static God; 

[RequireComponent (typeof (Rigidbody))]
public class Gravity_AttractedObject : MonoBehaviour
{
	[SerializeField] private Gravity_Source _currentGravitySource;
	public Gravity_Source CurrentGravitySource
	{
		get	{ return _currentGravitySource; }
		set
		{
			// These are used to make transitions smoother.
			isChangingSource = true;
			timeSinceSourceChange = 0.0f;
			
			// Assign the gravity source.
			_currentGravitySource = value;
			// If we are sourcing gravity from a gravity source, then don't apply Unity's gravity system.
			rb.useGravity = false;
		}
	}
	
	//public float blendToNewSource = 1.0f;
	//private float blendSpeed = 0.025f;
	public bool rotateToGravitySource = true; // Keep this on for important objects like characters, off for more preformance (for things like bullets).
	
	[HideInInspector] public bool isChangingSource = false;
	private const float sourceChangeDuration = 5.0f;
	private float timeSinceSourceChange;
	
	private Rigidbody rb; // A reference to the rigidbody on this game object.

    [HideInInspector] public bool blastOff;

    void Awake()
    {
		rb = GetComponent<Rigidbody>();
		
		if (CurrentGravitySource == null && Gravity_Source.DefaultGravitySource != null) CurrentGravitySource = Gravity_Source.DefaultGravitySource;
		else rb.useGravity = true;
		
		timeSinceSourceChange = sourceChangeDuration; // Set the timer to complete.
    }
	
	void Start()
	{
		// If the game begins with no gravity sources, then disabled this componenet to avoid null reference exceptions.
		if (Gravity_Source.DefaultGravitySource == null)
		{
			this.enabled = false;
		}
	}
	

    // Update is called once per frame
    void FixedUpdate()
    {
		if (timeSinceSourceChange != sourceChangeDuration) timeSinceSourceChange = Mathf.Clamp(timeSinceSourceChange + Time.fixedDeltaTime, 0.0f, sourceChangeDuration);
		
		//if (blendToNewSource != 1.0f) blendToNewSource = Mathf.Clamp(blendToNewSource + (blendSpeed * Time.fixedDeltaTime), 0.0f, 1.0f);
        //if (CurrentGravitySource != null) CurrentGravitySource.AttractObject(transform, blendToNewSource, rotateToGravitySource);
		
		// Convert the timer to a 0-1 value.
		float timeBasedBlend = Mathf.InverseLerp(0.0f, sourceChangeDuration, timeSinceSourceChange);
		
		if (CurrentGravitySource != null && !paused && !blastOff) CurrentGravitySource.AttractObject(transform, timeBasedBlend, rotateToGravitySource, isChangingSource);
    }
}

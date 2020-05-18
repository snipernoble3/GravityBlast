using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static God; 

[RequireComponent (typeof (Rigidbody))]
public class Gravity_AttractedObject : MonoBehaviour
{
	[SerializeField] private Gravity_Source _CurrentGravitySource;
	public Gravity_Source CurrentGravitySource
	{
		get	{ return _CurrentGravitySource; }
		set
		{
			// If we are sourcing gravity from a gravity source, then don't apply Unity's gravity system.
			if (value != null && rb != null) rb.useGravity = false;
			
			// If we are already sourcing gravity from this gravity source, then don't do anything else.
			if (_CurrentGravitySource == value) return;
			
			// These are used to make transitions from one gravity source to another.
			isChangingSource = true;
			timeLerpValue = 0.0f; // Reset the timer on the transition.
			
			// Assign the new gravity source.
			_CurrentGravitySource = value;
		}
	}

	public bool rotateToGravitySource = true; // Keep this on for important objects like characters, off for more preformance (for things like bullets).
	[HideInInspector] public bool blastOff;
	private Rigidbody rb; // A reference to the rigidbody on this game object.
	
	// Smooth Transition between gravity sources.
	[HideInInspector] public bool isChangingSource = false; // True while transitioning, if false the rotation toward the gravity source will always be instantaneous.
	
	private bool _InitialInfoAcquired = false; // Used to check if the initial angle and distance have been recorded yet.
	public bool InitialInfoAcquired
	{
		get	{ return _InitialInfoAcquired; }
		set
		{
			// If the bool is being set to false, reset the appropriate variables.
			if (value == false)
			{
				angleMultiplier = 1.0f; // Set the angleMultiplier to 1.0f so that the 100% of the transition time is used by default.
				initialDistance = 0.0f; // Reset the initial distance.
			}
			
			_InitialInfoAcquired = value;
		}
	}
	
	[HideInInspector] public float timeLerpValue {get; private set;} = 1.0f;  // At 0.0f the transition is begining, at 1.0f the transition is complete.
	[HideInInspector] public float angleMultiplier = 1.0f; // A value from 0-1 to represent an initial angle difference of 0-180 degrees between this object and the new gravity source uppon entering its gravity trigger.
	[HideInInspector] public float initialDistance = 0.0f; // The initial distance from the gravity source's surface uppon entering its gravity trigger.

    void Awake()
    {
		rb = GetComponent<Rigidbody>();
		if (CurrentGravitySource == null && Gravity_Source.DefaultGravitySource != null) CurrentGravitySource = Gravity_Source.DefaultGravitySource;
		else rb.useGravity = true;
    }
	
	void Start()
	{
		// If the game begins with no gravity sources, then disabled this componenet to avoid null reference exceptions.
		if (Gravity_Source.DefaultGravitySource == null)
		{
			this.enabled = false;
		}
	}
	
    void FixedUpdate()
    {
		// Count how long it has been since the source change.
		if (timeLerpValue != 1.0f)
		{
			float transitionDuration = 2.50f; // How long does the transition between gravity sources take in seconds.
			transitionDuration *= angleMultiplier; // Use the full amount of time for an initial angle difference of 180 degrees and less time for smaller angles, down to an instant transition at 0 degrees.
			// Use the if statement to avoid dividing by 0.
			// Add an incremental amount to the timeLerpValue based on the number of seconds specified by the transistionDuration.
			if (transitionDuration > 0.0f) timeLerpValue = Mathf.Clamp(timeLerpValue + Time.fixedDeltaTime / transitionDuration, 0.0f, 1.0f);
			else timeLerpValue = 1.0f;
		}
		
		if (CurrentGravitySource != null && !paused && !blastOff) CurrentGravitySource.AttractObject(this);
    }
}

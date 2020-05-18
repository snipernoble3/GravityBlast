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
			// If we are sourcing gravity from a gravity source, then don't apply Unity's gravity system.
			if (value != null) rb.useGravity = false;
			
			// If we are already sourcing gravity from this gravity source, then don't do anything else.
			if (_currentGravitySource == value) return;
			
			// These are used to make transitions from one gravity source to another.
			isChangingSource = true;
			timeLerpValue = 0.0f; // Reset the timer on the transition.
			
			// Assign the new gravity source.
			_currentGravitySource = value;
		}
	}

	public bool rotateToGravitySource = true; // Keep this on for important objects like characters, off for more preformance (for things like bullets).
	
	// Smooth Transition between gravity sources.
	public float timeLerpValue {get; private set;} = 1.0f;  // At 0.0f the transition is begining, at 1.0f the transition is complete.
	private const float transitionDuration = 1.0f; // How long does the transition between gravity sources take in seconds.
	
	[HideInInspector] public bool isChangingSource = false;
	[HideInInspector] public float initialDistance = 0.0f; // The initial distance from the gravity source's surface uppon entering its gravity trigger.
	
	private Rigidbody rb; // A reference to the rigidbody on this game object.

    [HideInInspector] public bool blastOff;

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
			// Avoid dividing by 0.
			if (transitionDuration > 0.0f) timeLerpValue = Mathf.Clamp((timeLerpValue + Time.fixedDeltaTime) / transitionDuration, 0.0f, 1.0f);
			else timeLerpValue = 1.0f;
		}
		
		if (CurrentGravitySource != null && !paused && !blastOff) CurrentGravitySource.AttractObject(this);
    }
}

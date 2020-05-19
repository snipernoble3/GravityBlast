using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static God; 

[RequireComponent (typeof (Rigidbody))]
public class Gravity_AttractedObject : MonoBehaviour
{
	public List<Gravity_Source> gravitySources = new List<Gravity_Source>();
	
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
			InitialInfoAcquired = false; // Reset the initial info in preperation for the transition.
			timeLerpValue = 0.0f; // Reset the timer on the transition.
			
			// Assign the new gravity source.
			_CurrentGravitySource = value;
			
			// Set this object to be a child of the new gravity source's surface.
			transform.SetParent(CurrentGravitySource.GetSurface(), true);
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
	
	private Coroutine returnToDefaultCo;

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
		
		// If there is more than one gravity source in the list, determine which one is closest and switch to it.
		if (gravitySources.Count > 1) CheckClosest();
		
		if (CurrentGravitySource != null && !paused && !blastOff) CurrentGravitySource.AttractObject(this);
    }
	
	public void AddGravitySource(Gravity_Source sourceToAdd)
	{
		if (!gravitySources.Contains(sourceToAdd)) gravitySources.Add(sourceToAdd);
		
		// If this was the first gravity source being added to the list, set the current gravity source to this.
		if (gravitySources.Count == 1) CurrentGravitySource = sourceToAdd;
		if (returnToDefaultCo != null) StopCoroutine(returnToDefaultCo);
	}
	
	public void RemoveGravitySource(Gravity_Source sourceToRemove)
	{
		// Remove the gravity source from the list.
		if (gravitySources.Contains(sourceToRemove)) gravitySources.RemoveAt(gravitySources.IndexOf(sourceToRemove));
		// If the list is empty, return to the default gravity source.
		if (gravitySources.Count == 0) returnToDefaultCo = StartCoroutine(DelayedSwitchToDefaultGravitySource());
		else if (!gravitySources.Contains(CurrentGravitySource))
		{
			// If the current gravity source is no longer in the list, switch to the next gravity source in the list.
			CurrentGravitySource = gravitySources.Last();
		}
	}
	
	private void CheckClosest()
	{
		Gravity_Source closestGravitySource = null;
		float closestDistance = float.MaxValue; // Start of at the max possible value;
		
		foreach (Gravity_Source gravitySource in gravitySources)
		{
			float distanceToCenter = Vector3.Distance(transform.position, gravitySource.GetSurface().position);
			
			Vector3 rayStartPoint = transform.position + transform.up * 0.25f; // Give a slight relative vertical offset so that the ray doesn't start below the surface while standing on it.
			RaycastHit[] surfaceHits = Physics.RaycastAll(rayStartPoint, gravitySource.GetGravityVector(transform), distanceToCenter);
			
			for (int i = 0; i < surfaceHits.Length; i++)
			{
				if (surfaceHits[i].transform == gravitySource.GetSurface())
				{
					float distance = (transform.position - surfaceHits[i].point).sqrMagnitude; // Use this to avoid a square root calculation.
					//float distance = Vector3.Distance(transform.position, surfaceHits[i].point);
					if (distance < closestDistance)
					{
						closestDistance = distance;
						closestGravitySource = gravitySource;
					}
					break; // Once we've found the ray that hit the gravity source's surface's collider, stop checking through the loop.
				} 
			}
		}
		
		if (closestGravitySource == null || CurrentGravitySource == closestGravitySource) return;
		CurrentGravitySource = closestGravitySource;
	}
	
	public IEnumerator DelayedSwitchToDefaultGravitySource()
	{
		// Wait for the specified number of seconds after leaving gravity sources before transtioning back to the default gravity source.
		yield return new WaitForSeconds(0.75f);
		
		// If the gravitySource list is still empty after the delay, return to the default gravity source.
		if (gravitySources.Count == 0) CurrentGravitySource = Gravity_Source.DefaultGravitySource;
	}
}

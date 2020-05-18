﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity_Source : MonoBehaviour
{
	public static Gravity_Source DefaultGravitySource {get; private set;}
	[SerializeField] private bool isDefaultGravitySource = false; // Only one object per scene should have this checked.
	[SerializeField] private Transform surface; // The surface of the planet or object that.
	
	[SerializeField] private float gravityStrength = -9.81f; // This default is equivalent to gravity on Earth.
	[SerializeField] private bool isRadial = true; // This should be true for planetoids and moons.
	[SerializeField] private Vector3 nonRadialDirection = Vector3.up; // Set this in the inspector, it will be transformed to the object's local space.
	
	// Test stuff
	public static bool useTestSpheres = false; // Set this to false when finished testing!!!
	private static GameObject testSphere;

    void Awake()
    {
		// If marked as such in the inspector, use this gravity source as the default gravity source so that it can be referenced from anywhere.
		if (isDefaultGravitySource) DefaultGravitySource = this;
    }
	
	void Start()
    {
		// If we don't have a default gravity source when the scene starts...
		//...then create a gameObject that can be used as the default gravity source.
		if (DefaultGravitySource == null) 
		{
			GameObject defaultGravityGameObject = new GameObject("Default Gravity Source");
			DefaultGravitySource = defaultGravityGameObject.AddComponent<Gravity_Source>();
			DefaultGravitySource.isDefaultGravitySource = true;
			
			DefaultGravitySource.gravityStrength = -Physics.gravity.magnitude;
			DefaultGravitySource.isRadial = false;
			DefaultGravitySource.nonRadialDirection = -Physics.gravity.normalized;
		}
		
		// If no surface is specified for this gravity source, use its own transform as the surface.
		if (surface == null) surface = transform;
	}
	
	private void InitializeTestSphere()
	{
		// Set up a testSphere which can be instantiated as needed.
		testSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Destroy(testSphere.GetComponent<SphereCollider>());
		testSphere.layer = LayerMask.NameToLayer("GravityTrigger");
		
		Renderer sphereRend = testSphere.GetComponent<Renderer>();
		Material newMat = new Material(sphereRend.material);
		newMat.color = Color.magenta;
		sphereRend.material = newMat;
			
		testSphere.SetActive(false);
	}
	
	private void OnTriggerEnter(Collider triggeredObject)
    {
        Gravity_AttractedObject attractedObject = triggeredObject.transform.GetComponent<Gravity_AttractedObject>();
		
		// If the object that entered the trigger has the Gravity_AttractedObject component...
		//assign this gravity source to it's CurrentGravitySource.
		if (attractedObject != null)
		{
			attractedObject.initialDistance = 0.0f; // Set the initial distance to 0.0f as a sentinel value in preperation for the transition.
			attractedObject.CurrentGravitySource = this;
			triggeredObject.transform.SetParent(surface, true);
		}
		
		Player_Movement player = triggeredObject.transform.GetComponent<Player_Movement>();
		if (player != null) player.gravitySource = this;
    }
	
	private void OnTriggerExit(Collider triggeredObject)
    {
        Gravity_AttractedObject exitingObject = triggeredObject.transform.GetComponent<Gravity_AttractedObject>();
		if (exitingObject != null)
		{	
			exitingObject.CurrentGravitySource = DefaultGravitySource;
			triggeredObject.transform.SetParent(DefaultGravitySource.transform, true);
		}
		
		// DELETE THIS ONCE THE PLAYER MOVEMENT CODE HAS BEEN CLEANED UP:
		Player_Movement player = triggeredObject.transform.GetComponent<Player_Movement>();
		if (player != null) player.gravitySource = DefaultGravitySource;
    }
	
	public Vector3 GetGravityVector(Transform attractedObject)
	{
		if (isRadial) return (attractedObject.position - transform.position).normalized * gravityStrength;
		else return transform.TransformDirection(nonRadialDirection * gravityStrength);
	}
	
	public void AttractObject(Gravity_AttractedObject attractedObject)
	{		
		// Store the transform of the attractedObject so it can be more easily referenced.
		Transform attractedTransform = attractedObject.transform;
		
		// Find the direction of gravity.
		Vector3 gravityVector = GetGravityVector(attractedTransform);
		
		// Add force to the attracted object to simulate gravity toward the gravity source.
		attractedTransform.GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);
		
		// Rotate the attracted object so that its downward direction faces the gravity source.
		if (attractedObject.rotateToGravitySource)
		{			
			// Default to an instantaneous rotation, calculate an alternative value if the attractedObject is transitioning between gravity sources.
			float rotationLerpValue = 1.0f;
			
			if (attractedObject.isChangingSource)
			{
				// Get the distance from the attracted object to the CENTER of the gravity source...
				// This value will be longer than the distnace to the SURFACE, but it gives us a ray length that we can use to check for the surface point.
				float distanceToSurface = Vector3.Distance(attractedTransform.position, surface.position);
				
				// Use a RaycastAll in case somthing is in the way between the attracted object and the gravity source.
				RaycastHit[] surfaceHits = Physics.RaycastAll(attractedTransform.position, gravityVector, distanceToSurface);
				
				for (int i = 0; i < surfaceHits.Length; i++)
				{
					// Check if the object hit was this gravity source's surface.
					if (surfaceHits[i].transform == surface)
					{
						// Update the distanceToSurface to be the distance from the attracted object to the hit point of the raycast.
						distanceToSurface = Vector3.Distance(attractedTransform.position, surfaceHits[i].point);
						
						// Create test sphere to visualize the surface point that was hit.
						if (useTestSpheres)
						{
							if (testSphere == null) InitializeTestSphere();
							GameObject newSphere = Instantiate(testSphere, surfaceHits[i].point, Quaternion.identity);
							newSphere.transform.SetParent(surface, true);
							newSphere.SetActive(true);
							Destroy(newSphere, 5.0f);
						}
						break; // Once we've found the ray that hit the gravity source's surface's collider, stop checking through the loop.
					} 
				}
				
				// Use the sentinel value of 0.0f to determine if the initial distance has been set yet, if not set it to the current distance.
				if (attractedObject.initialDistance == 0.0f) attractedObject.initialDistance = distanceToSurface;
				
				float minDistance = 0.05f; // Add a tiny amount of padding.
				float distanceLerp = Mathf.InverseLerp(attractedObject.initialDistance, minDistance, distanceToSurface);
				distanceLerp = Mathf.Clamp(distanceLerp, 0.0f, 1.0f); // Clamp in case the attracted object goes below the surface, or is pulled beyond the initial distance.
				
				float timeLerp = attractedObject.timeLerpValue;
				
				// DELETE THESE AFTER IMPLEMENTATION IS FINISHED.
				distanceLerp = 0.0f; // Override the distance lerp so that it is not being used.
				// timeLerp = 0.0f; // Override the time lerp so that it is not being used.
					
				// Use the greater lerp value: the time it's been since the transition started, or the distance from the planet surface.
				rotationLerpValue = (distanceLerp >= timeLerp) ? distanceLerp : timeLerp;
				
				// If the transition is complete reset the appropriate variables.
				if (rotationLerpValue == 1.0f)
				{
					attractedObject.isChangingSource = false; // We're done transitioning to the new gravity source.
					attractedObject.initialDistance = 0.0f; // Reset the initial distance value just in case.
				}
			}
				
			// Set the target rotation (aim at gravity source).
			Quaternion targetRotation = Quaternion.FromToRotation(-attractedTransform.up, gravityVector.normalized) * attractedTransform.rotation;
			// Rotate the attracted object based on the lerp value.
			attractedTransform.rotation = Quaternion.Slerp(attractedTransform.rotation, targetRotation, rotationLerpValue);
		}
	}
}

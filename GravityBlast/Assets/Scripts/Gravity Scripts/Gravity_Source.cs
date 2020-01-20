using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity_Source : MonoBehaviour
{
	public static Gravity_Source DefaultGravitySource {get; private set;}
	[SerializeField] private bool isDefaultGravitySource = false;
	
	[SerializeField] private float gravityStrength = -9.81f;
	[SerializeField] private bool isRadial = true;
	private Vector3 nonRadialDirection;
	
	//private Transform defaultGravitySource;
	
	// Test stuff
	[SerializeField] private bool useTestSpheres = false;

	
	// Start is called before the first frame update
    void Awake()
    {
        nonRadialDirection = transform.up;	
		if (isDefaultGravitySource) DefaultGravitySource = this;
    }
	
	private void OnTriggerEnter(Collider triggeredObject)
    {
        Gravity_AttractedObject attractedObject = triggeredObject.transform.GetComponent<Gravity_AttractedObject>();
		
		if (attractedObject != null)
		{
			//attractedObject.timeSinceSourceChange = 0.0f; // Reset the timer on the attractedObject.
			//attractedObject.blendToNewSource = 0.0f;
			
			attractedObject.SetGravitySource(this);
			triggeredObject.transform.SetParent(transform.parent);
		}
		
		Player_Movement player = triggeredObject.transform.GetComponent<Player_Movement>();
		if (player != null) player.gravitySource = this;
    }
	
	private void OnTriggerExit(Collider triggeredObject)
    {
        Gravity_AttractedObject exitingObject = triggeredObject.transform.GetComponent<Gravity_AttractedObject>();
		
		if (exitingObject != null)
		{
			if (DefaultGravitySource != null)
			{	
				//exitingObject.timeSinceSourceChange = 0.0f; // Reset the timer on the attractedObject.
				//exitingObject.blendToNewSource = 0.0f;
				//exitingObject.gravitySource = DefaultGravitySource;
			
				exitingObject.SetGravitySource(DefaultGravitySource);
				triggeredObject.transform.SetParent(DefaultGravitySource.transform);
			}
		}
		
		Player_Movement player = triggeredObject.transform.GetComponent<Player_Movement>();
		
		if (player != null) player.gravitySource = DefaultGravitySource;
    }
	
	public Vector3 GetGravityVector(Transform attractedObject)
	{
		if (isRadial) return (attractedObject.position - transform.position).normalized * gravityStrength;
		else return nonRadialDirection * gravityStrength;
	}
	
	public void AttractObject(Transform attractedObject, float timeBasedBlend, bool rotateToGravitySource, bool isChangingSource)
	{		
		// Find the direction of gravity
		Vector3 gravityVector = GetGravityVector(attractedObject);
		
		// Add force to the attracted object to simulate gravity toward the gravity source.
		attractedObject.gameObject.GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);
		
		if (rotateToGravitySource) // Rotate the attracted object so that its downward direction faces the gravity source
		{			
			float rotationStep = 0.1f;
			
			if (isChangingSource)
			{
				// Start by getting the distance from the attracted object to the CENTER of the gravity source (this will be longer than the distnace to the SURFACE, but it gives us a range to check).
				float distanceToSurface = Vector3.Distance(attractedObject.position, transform.position);
				
				RaycastHit[] surfaceHits; 
				// Use a RaycastAll in case somthing is in the way between the attracted object and the gravity source.
				surfaceHits = Physics.RaycastAll(attractedObject.position, gravityVector, distanceToSurface);
				
				for (int i = 0; i < surfaceHits.Length; i++)
				{
					// Check if the object hit was this gravity source.
					if (surfaceHits[i].transform == transform)
					{
						// Update the distanceToSurface to be the distance from the attracted object to the hit point of the raycast.
						distanceToSurface = Vector3.Distance(attractedObject.position, surfaceHits[i].point);
						
						// Test sphere
						if (useTestSpheres)
						{
							GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							sphere.transform.position = surfaceHits[i].point;
							sphere.transform.SetParent(transform);
							Destroy(sphere.GetComponent<SphereCollider>());
							Renderer sphereRend = sphere.GetComponent<Renderer>();
							sphereRend.material = new Material(Shader.Find("Standard"));
							sphereRend.material.color = Color.magenta;
							Destroy(sphere, 5.0f);
						}
						break; // Once we've found the ray that hit the gravity source's collider, stop checking through the loop.
					} 
				}
				
				// Convert the distance to a 0-1 mapping for use in the rotation slerp.
				float distanceBasedBlend = Mathf.InverseLerp(50.0f, 0.0f, distanceToSurface); 
				
				// What is the slowest the player is allowed to rotate (useful for when the object is SUPER far away from the gravity source).
				float maxRotationStep = 0.025f;
				
				distanceBasedBlend = Mathf.Clamp(distanceBasedBlend, 0, maxRotationStep);
				
				//  = 0.025f
				
				// Calculate how far the object has to rotate.
				//float rotationDegrees = Vector3.Angle(-attractedObject.up, gravityVector.normalized);
				
				//float rotationStep = Mathf.InverseLerp(0.0f, 180.0f, rotationDegrees); // Convert the angles 0-180 to a 0-1 mapping.
				
				// 0.0f doesn't rotate at all, 1.0f rotates instantly.
				// 0.03f is fast, for things will small angle diferences, 0.0025f is slow for things with large angle differences.
				distanceBasedBlend = Mathf.Lerp(0.03f, 0.0025f, distanceBasedBlend); // Convert the angles 0-180 to a 0-1 mapping.
				
				/*
				rotationStep = Vector3.Distance(attractedObject.position, transform.position);
				rotationStep = Mathf.InverseLerp(0.0f, 50.0f, rotationStep); // Convert the angles 0-180 to a 0-1 mapping.
				rotationStep = Mathf.Lerp(0.03f, 0.0025f, rotationStep); // Convert the angles 0-180 to a 0-1 mapping.
				*/
				
				// Rotate towards the target.
				//attractedObject.rotation = Quaternion.Slerp(attractedObject.rotation, targetRotation, );
				
				//rotationStep = Mathf.Max(timeBasedBlend, distanceBasedBlend);
				
				//if (rotationStep == 1.0f) attractedObject.GetComponent<Gravity_AttractedObject>().isChangingSource = false;
			}
			//else rotationStep = 1.0f;
			
			
			
			//rotationStep = 35.0f; // If we aren't transitioning to a new planet keep the rotation step high.
			
			// Make sure the speeds are framerate independant but also clamped to a 0-1 range.
			//rotationStep = Mathf.Clamp(rotationStep * Time.fixedDeltaTime, 0.0f, 1.0f);
			
			rotationStep = 2.5f * Time.fixedDeltaTime;
			
			//Debug.Log(rotationStep);
			
			// Set the target rotation (aim at gravity source)
			Quaternion targetRotation = Quaternion.FromToRotation(-attractedObject.up, gravityVector.normalized) * attractedObject.rotation;
			
			attractedObject.rotation = Quaternion.Slerp(attractedObject.rotation, targetRotation, rotationStep);
		}
	}
}

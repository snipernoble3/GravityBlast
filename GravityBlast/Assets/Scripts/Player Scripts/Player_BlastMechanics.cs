using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles Rocket Jumping, Ground Pounding, and Object Blasting.
[RequireComponent(typeof(Player_Movement))]
public class Player_BlastMechanics : MonoBehaviour
{
	private bool blastEnabled = true;
	
	// User Preferences
	public bool enableCameraShake_gpLanding = true;
	public bool autoJumpBeforeGroundedRocketJump = true;
	public bool blastDealsDamage = false;
	public bool rj_trainingHitMarker = false;
	
	// Object References
	
	private Player_Movement playerMovement;
	public GameObject groundPoundParticles; // gets reference to the particles to spawn
	private GameObject gpParticles_GameObject; // stores the instance of the particles
	private Transform firstPersonCamera;
	private GameObject camOffset;
	
	public LayerMask raycastMask;
	private Rigidbody playerRB;
	public Animator firstPersonArms_Animator;
	public GameObject armTube;
	private Material armTube_Material;
	public GameObject blastWarp;
	private Material blastWarp_Material;
	private Coroutine blastWarpCo;
	private Coroutine armTubeCo;
	
	
	// Rocket Jumping Variables
	private int rjBlast_NumSinceGrounded = 0;
	[SerializeField] private int rjBlast_MidAirLimit = 1;
	private bool rjBlast_DidHitSurface = false;
	
	private float rjBlast_CoolDownTime = 0.25f; // seconds between rocket jumps
	private float rjBlast_TimeSinceLastJump;
	
	private const float rjBlast_Range = 4.0f;
	private const float rjBlast_Power = 850.0f;
	private Vector3 rjBlast_Epicenter; // The origin of the rocket jump blast radius.
	private const float rjBlast_Radius = 5.0f;
	private const float rjBlast_UpwardForce = 0.5f;
	private const float minRocketJumpCameraAngle = 45.0f;
	
	private float impactVelocity = 0.0f;
	public float minGroundPoundVelocity = 8.0f;
	public float groundPound_Multiplier = 25.0f;

    [HideInInspector] public bool paused;

    void Awake()
    {		
		// Set up references
		armTube_Material = armTube.GetComponent<Renderer>().material;
		blastWarp_Material = blastWarp.GetComponent<Renderer>().material;
		playerMovement = GetComponent<Player_Movement>();
		firstPersonCamera = transform.Find("Camera Position Offset/First Person Camera");
		camOffset = transform.Find("Camera Position Offset").gameObject;
        playerRB = GetComponent<Rigidbody>();
		rjBlast_TimeSinceLastJump = rjBlast_CoolDownTime;
    }

    void Update()
    {
		if (rjBlast_TimeSinceLastJump < rjBlast_CoolDownTime) rjBlast_TimeSinceLastJump = Mathf.Clamp(rjBlast_TimeSinceLastJump += 1.0f * Time.deltaTime, 0.0f, rjBlast_CoolDownTime);
		
		// Inputs
		if (blastEnabled)
		{
			if (Input.GetButtonDown("Fire2") && rjBlast_TimeSinceLastJump == rjBlast_CoolDownTime && !paused) RocketJumpCheck();
		}		
		if (Input.GetButton("Crouch") && !playerMovement.GetIsGrounded() && !paused) AccelerateDown();
    }
	
	public void EnableBlast(bool blastState)
	{
		blastEnabled = blastState;
	}
	
	void FixedUpdate()
	{	
		GroundPoundCheck();
		if (playerMovement.GetIsGrounded()) rjBlast_NumSinceGrounded = 0;
	}
	
	// Called via player input, Checks if the conditions to be able to rocket jump are met,
	// if so, calls the rocket jump method that performs the rocket jump.
	private void RocketJumpCheck()
	{
		RaycastHit rjBlast_Hit;
			
		//Determine if the the rocket jump blast hit an object or if it was a mid-air rocket jump, and therefore determine where its center point should be.
		if (Physics.Raycast(firstPersonCamera.position, firstPersonCamera.forward, out rjBlast_Hit, rjBlast_Range, raycastMask, QueryTriggerInteraction.Ignore))
		{
			//if (rjBlast_Hit.collider != null)		
			rjBlast_DidHitSurface = true;
			rjBlast_Epicenter = rjBlast_Hit.point;
		}
		else
		{
			rjBlast_DidHitSurface = false;
			rjBlast_Epicenter = firstPersonCamera.position + (firstPersonCamera.forward * rjBlast_Range);
		}
		
		firstPersonArms_Animator.Play("Blast", 1, 0.25f); // Play the blast animation.
		
		if (armTubeCo != null) StopCoroutine(armTubeCo); // Stop the coroutine so that it isn't overlapping the previous coroutine.
		if (blastWarpCo != null) StopCoroutine(blastWarpCo); // Stop the coroutine so that it isn't overlapping the previous coroutine.
		armTubeCo = StartCoroutine(ArmTubePulsate()); // make the tube pulsate.
		blastWarpCo = StartCoroutine(BlastWarpEffect()); // create visual warping effect.
		
		
		BlastForce(rjBlast_Power, rjBlast_Epicenter, rjBlast_Radius, rjBlast_UpwardForce); // Add the blast force to affect other objects.
			
		if (playerMovement.GetIsGrounded())
		{
			if (playerMovement.GetVerticalCameraAngle() <= -45.0f && playerMovement.GetVerticalCameraAngle() >= -90.0f)
			{
				// Force the player into the air (as if he jumped) before applying the rocket jump to get a compounding force.
				if (autoJumpBeforeGroundedRocketJump) StartCoroutine(JumpThenRocketJump());
				else RocketJump(); // Rocket jump wihtout jumping first.
			}
		}
		else RocketJump();
		
		
	}

	// Called via the Rocket Jump Check method, this actually performs the rocket jump.
	private void RocketJump()
	{
		rjBlast_TimeSinceLastJump = 0.0f;
		
		// Test sphere
		if (rj_trainingHitMarker)
		{
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = rjBlast_Epicenter;
			Destroy(sphere.GetComponent<SphereCollider>());
			Renderer sphereRend = sphere.GetComponent<Renderer>();
			sphereRend.material = new Material(Shader.Find("Standard"));
			if (rjBlast_DidHitSurface) sphereRend.material.color = Color.green;
			else if (rjBlast_NumSinceGrounded < rjBlast_MidAirLimit) sphereRend.material.color = Color.yellow;
			else sphereRend.material.color = Color.red;
			Destroy(sphere, 5.0f);
		}
		
		// For testing reset number of mid air jumps if you hit a surface even before touching the ground.
		if (rjBlast_DidHitSurface) rjBlast_NumSinceGrounded = 0;
		
		if (rjBlast_NumSinceGrounded < rjBlast_MidAirLimit) 
		{
			// Cancel the downward velocity
			playerRB.AddRelativeForce(new Vector3(0.0f, playerMovement.GetDownwardVelocity() * playerRB.mass, 0.0f), ForceMode.Impulse);
			
			// Add the rocket jump force
			playerRB.AddExplosionForce(rjBlast_Power, rjBlast_Epicenter, rjBlast_Radius, 0.0f, ForceMode.Impulse);
			if (!playerMovement.GetIsGrounded() && !rjBlast_DidHitSurface) rjBlast_NumSinceGrounded += 1;
		}
	}
	
	private void AccelerateDown()
	{
		// If the player is currently accelerating upward, instantly canncel upward velocity, then apply downward force.
		//if (playerRB.velocity.y > 0.0) playerRB.velocity = new Vector3 (playerRB.velocity.x, 0.0f, playerRB.velocity.z);
		//playerRB.AddForce(playerMovement.GetGravity().normalized * groundPound_Multiplier, ForceMode.Acceleration);
		//playerRB.AddForce(playerMovement.GetGravity().normalized * groundPound_Multiplier, ForceMode.Impulse);
		
		playerRB.AddRelativeForce(Vector3.down * groundPound_Multiplier, ForceMode.Impulse);
		
		//playerRB.AddForce(playerMovement.GetGravity() * groundPound_Multiplier / playerMovement.GetGravity().magnitude, ForceMode.Impulse);
		//playerMovement.TerminalVelocity();
	}
	
	private void GroundPoundCheck()
	{
		float downwardVelocity = playerMovement.GetDownwardVelocity();
		
		if(downwardVelocity != 0.0f) impactVelocity = downwardVelocity; // Set the "previous velocity" at this physics step so it can be compared during the next physics step.
		if (impactVelocity != 0.0f && downwardVelocity == 0.0f && impactVelocity >= minGroundPoundVelocity && playerMovement.GetIsGrounded())
		{
			float gpBlast_Power = 65.0f * impactVelocity;
			float gpBlast_Radius = impactVelocity * 0.3f; // 5.0f;
			float gpBlast_UpwardForce = 0.1f * impactVelocity;
			
			float camShake_Amplitude = Mathf.Clamp(Mathf.Pow(impactVelocity * 0.04f, 3.0f), 0.5f, 20.0f);
			float camShake_Frequency = Mathf.Clamp(impactVelocity * 2.0f, 15.0f, 25.0f);
			float camShake_Duration = Mathf.Clamp((impactVelocity * 0.02f), 0.0f, 0.7f);
			
			// Camera Shake
			StartCoroutine(CameraShake(camShake_Amplitude, camShake_Frequency, camShake_Duration, camShake_Duration * 0.2f, camShake_Duration * 0.5f));
			
			// Blast Force
			BlastForce(gpBlast_Power, playerRB.position, gpBlast_Radius, gpBlast_UpwardForce); // Apply a blast around the landing
			
			// Particle System
			if (gpParticles_GameObject != null) Destroy(gpParticles_GameObject); // to prevent multiple particle systems from spawning when clipping the corner of rounded objects.
	
			float gpParticle_Duration = camShake_Duration * 3.0f;			
			
			gpParticles_GameObject = Instantiate(groundPoundParticles, playerRB.position, Quaternion.Euler(transform.up)) as GameObject;
			
			//gpParticles_GameObject = Instantiate(groundPoundParticles, playerRB.position, Quaternion.Euler(playerMovement.gravitySource.GetGravityVector(transform).normalized)) as GameObject;
			//gpParticles_GameObject = Instantiate(groundPoundParticles, playerRB.position, Quaternion.LookRotation(playerRB.transform.forward, (transform.position - playerMovement.gravitySource.transform.position).normalized)) as GameObject;
			//gpParticles_GameObject = Instantiate(groundPoundParticles, playerRB.position, Quaternion.Euler((transform.position - playerMovement.gravitySource.transform.position).normalized)) as GameObject;
			
			
			
			ParticleSystem.MainModule gpParticles_MainModule = gpParticles_GameObject.GetComponent<ParticleSystem>().main;
			
			gpParticles_MainModule.startSpeed = Mathf.Clamp(impactVelocity, 5.0f, 50.0f);
			gpParticles_MainModule.startLifetime = gpParticle_Duration;
			gpParticles_MainModule.duration = gpParticle_Duration;
			gpParticles_GameObject.GetComponent<ParticleSystem>().Play();
			Destroy(gpParticles_GameObject, gpParticle_Duration);
			
			// Reset downward velocity.
			impactVelocity = 0.0f;
		}
	}
	
	private void BlastForce(float blast_Power, Vector3 blast_Epicenter, float blast_Radius, float blast_UpwardForce)
	{
		// Check all objects within the blast radius.
		Collider[] colliders = Physics.OverlapSphere(blast_Epicenter, blast_Radius);
		foreach (Collider objectToBlast in colliders)
		{
			Rigidbody rb = objectToBlast.GetComponent<Rigidbody>();
			// If the object has a rigidbody component and it is not the player, add the blast force!
			if (rb != null && rb != playerRB) rb.AddExplosionForce(blast_Power, blast_Epicenter, blast_Radius, blast_UpwardForce, ForceMode.Impulse);

            if (blastDealsDamage)
			{
				Health h = objectToBlast.GetComponent<Health>();
				// If the object is not the player or an ally to the player, deal damage based on the distance from the blast
				if (h != null && (h.GetTag() == Health.AlignmentTag.Enemy || h.GetTag() == Health.AlignmentTag.Neutral))
				{
					h.TakeDamage((int)(blast_Radius / (0.75 * Vector3.Magnitude(h.gameObject.transform.position - blast_Epicenter))));
				}
			}
        }
	}
	
	// This method includes a tiny delay to ensure that the counter for mid-air rocket jumps isn't thrown off
	// AND so that the two forces aren't applied inhumanly fast resulting in a higher jump than is otherwise humanly reflexivly possible.
	public IEnumerator JumpThenRocketJump()
	{
		playerMovement.Jump(); // Comunicate with the Player_Movement script to force the player to jump.
		yield return new WaitForSeconds(0.1f);
		RocketJump();
	}
	
	public IEnumerator CameraShake(float shake_amplitude, float shake_frequency, float shake_Duration, float fade_Duration_In, float fade_Duration_Out)
	{
		if (enableCameraShake_gpLanding) // If the player has chosen to disable camera shake, then do nothing.
		{
			if (fade_Duration_In + fade_Duration_Out < shake_Duration) // If there won't be enough time to complete the fade in and the fade out, then don't shake at all.
			{
				float fade_Min = 0.0f;
				float fade_Max = 1.0f;

				float fade_Multiplier = fade_Min; // Start with the shake faded out completely.
				float fade_Rate = fade_Max / fade_Duration_In; // Set the fade rate so that the shake will fade in over time.
				
				bool fade_isFadingIn = true;
				bool fade_isFadingOut = false;
				
				float shake_timeElapsed = 0.0f;
				float shake_Angle_Pitch = 0.0f;
				float shake_Angle_Roll = 0.0f;
				
				while (shake_timeElapsed < shake_Duration)
				{
					shake_Angle_Pitch = Mathf.Sin(Time.time * shake_frequency) * shake_amplitude * fade_Multiplier;
					shake_Angle_Roll = Mathf.Cos(Time.time * shake_frequency) * shake_amplitude * fade_Multiplier * 0.5f;
					
					// Increase or decrease the fade multiplier
					if (fade_isFadingIn || fade_isFadingOut)
					{
						fade_Multiplier = Mathf.Clamp(fade_Multiplier + (fade_Rate * Time.deltaTime), fade_Min, fade_Max);
						if (fade_Multiplier == fade_Max) fade_isFadingIn = false;
					}
					
					// Check if it's time to start fading out
					if (!fade_isFadingIn && !fade_isFadingOut && shake_timeElapsed >= shake_Duration - fade_Duration_Out)
					{
						fade_Rate = fade_Max / -fade_Duration_Out;
						fade_isFadingOut = true;
					}
					
					// Apply the shake to the camera
					camOffset.transform.localEulerAngles = new Vector3(shake_Angle_Pitch, 0.0f, shake_Angle_Roll);	
					
					shake_timeElapsed += Time.deltaTime;

					yield return null;
				}
				
				// Ensure that the camera is back to zero when the shake is done.
				camOffset.transform.localRotation = Quaternion.identity;
			}
			else Debug.Log("Fade duration ("+ (fade_Duration_In + fade_Duration_Out) +" seconds) can not be longer than the shake duration (" + shake_Duration + " seconds). The camera shake was not performed.");
		}
	}
	
	public IEnumerator ArmTubePulsate()
	{
		//float phaseStart = 0.15f; // Beginning of the shoulder
		float phaseStart = 0.5f; // Beginning of the elbow tube pivot point.
		float phaseEnd = 0.85f;
		float phaseStep = 0.015f;
		
		float phase = phaseStart;
		
		while (phase <= phaseEnd)
		{
			//phase = Mathf.Clamp(phase + phaseStep * Time.deltaTime, phaseStart, phaseEnd);
			phase += phaseStep;
			armTube_Material.SetFloat("_PulsePhase", phase);
			
			yield return null;
		}
	}
	
	public IEnumerator BlastWarpEffect()
	{
		blastWarp.SetActive(true);
		
		float intensityMin = 0.0f;
		float intensityMax = 0.85f;
		
		float intensitySpeed = 6.0f;
		
		float intensity = 0.5f;
		
		while (intensity <= Mathf.PI)
		{
			intensity += Time.deltaTime * intensitySpeed;
			float warpIntensity = Mathf.Lerp(intensityMin, intensityMax, ((Mathf.Sin(intensity) + 1.0f) / 2.0f));
			
			blastWarp_Material.SetFloat("_WarpIntensity", warpIntensity);
			
			yield return null;
		}
		
		blastWarp.SetActive(false);
	}
}

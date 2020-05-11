using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class MovementParameters
{
	public float accelerationRate; // The ammount of acceleration the player is requesting to be added to his movement each frame.
	public float accelerationLimit; // The maximum ammount of force that can be added per physics step.
	public float targetSpeed; // The speed the player is trying to accelerate to.
	
	// Speed caps for different situations
	public readonly float groundedMax = 14.0f; // Cap the walking speed. // Quake set this to 320 ups (approximately 10 player's thickness per second OR a velocity of 10 in Unity)
	public readonly float bHopMax = 20.0f; // Cap the bunny hopping speed.
	
	public readonly float lateralMax = 50.0f; // NEVER let the player move faster than this lateraly.	
	public readonly float verticalMax = 75.0f; // NEVER let the player move faster than this verticaly.	
	
	[SerializeField] public ReductionMultiplier reduction = new ReductionMultiplier();
	public MovementVectors vector = new MovementVectors();
}

public class MovementVectors
{
	public Vector3 input; // Raw input from the player.
	public Vector3 request; // Input from the player multiplied by the rampUp and speedReduction values.
	public Vector3 projected; // The most important one for Quake style movement!!!
	public Vector3 output; // The final vector that will be used to apply force to the player.
	
	public Vector3 localVelocity;
	public Vector3 localVelocity_Lateral;
}

[System.Serializable]
public class ReductionMultiplier
{
	private float multiplier; // The current multiplier.
	
	private const float regular = 1.0f; // Set to 1.0f so there is no reduction while grounded.
	[SerializeField] [Range(0.0f, 1.0f)] private float walk = 0.5f;
	[SerializeField] [Range(0.0f, 1.0f)] private float crouch = 0.5f;
	[SerializeField] [Range(0.0f, 1.0f)] private float air = 0.5f;
	[SerializeField] [Range(0.0f, 1.0f)] private float water = 0.75f;
	
	// Setter with specific predefined values, other multipliers can be used by assigning the value directly.
	public void SetMultiplier(string newMultiplier)
	{
        switch(newMultiplier) 
        {
			case "regular": 
                multiplier = regular;
                break; 
            case "walk": 
                multiplier = walk;
                break; 
			case "crouch": 
                multiplier = crouch;
                break; 
			case "air": 
                multiplier = air;
                break; 
            case "water": 
                multiplier = water;
                break; 
            default:
                Debug.Log("ReductionMultiplier.SetMultiplier() method was called with an invalid argument, multiplier was not changed.");
				break;
        } 
	}
	
	public float GetMultiplier()
	{
		return multiplier;
	}
}

[RequireComponent(typeof(Rigidbody))]
public class Player_Movement : MonoBehaviour
{
	private bool lookEnabled = true;
	private bool moveEnabled = true;
	
	// Object References
	public Gravity_Source gravitySource;
	private Transform firstPersonCamera;
	private Transform firstPersonObjects;
	private Rigidbody playerRB;
	public Animator firstPersonArms_Animator;
	
	// HUD
	private GameObject hud;
	private List<VectorVisualizer> radarLines = new List<VectorVisualizer>();
	private TextMeshProUGUI hud_LateralVelocity;
	private TextMeshProUGUI hud_VerticalVelocity;
	
	[SerializeField] private MovementParameters movement = new MovementParameters();
	private Vector3 gravity; // Use this instead of Physics.gravity in case we want to replace gravity with attraction to a gravity sorce (like a tiny planet).
	private bool isGrounded = false; // Initialize as false, since player may spawn in mid-air
	
	// Jump
	//public bool holdJumpToKeepJumping = false;
	[SerializeField] private float jumpForceMultiplier =  3.0f;
	private const float jumpCoolDownTime = 0.2f;
	private float timeSinceLastJump;
	private bool jumpQueue_isQueued = false;
	private const float jumpQueue_Expiration = 0.3f; // How long will the jump stay queued.
	private float jumpQueue_TimeSinceQueued = 0.0f; // How long has it been since the jump was queued.
	private const float jumpQueue_bHopGracePeriod = 0.3f; // How long before friction starts being applied to the player.
	private float jumpQueue_timeSinceGrounded = 0.0f; // How long has it been since the player became grounded.
	
	// Mouse Input
	[SerializeField] private float mouseSensitivity_X = 3.0f;
	[SerializeField] private float mouseSensitivity_Y = 1.0f;
	[SerializeField] private bool matchXYSensitivity = true;
	[SerializeField] private bool useRawMouseInput = true;
	[SerializeField] private bool invertVerticalInput = false;
	private float rotation_vertical = 0.0f;
	private float rotation_horizontal = 0.0f;
	private float verticalAngle = 0.0f;
	private float horizontalAngle = 0.0f;

    void Awake()
    {
		// Hide the mouse cursor
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
		
		// Set up references
		firstPersonCamera = transform.Find("Camera Position Offset/First Person Camera");
		firstPersonObjects = firstPersonCamera.Find("First Person Objects");
		
		playerRB = GetComponent<Rigidbody>();
		playerRB.constraints = RigidbodyConstraints.FreezeRotation;
		gravity = GetGravity();
		
		// Set limits for Lateral Movement
		movement.reduction.SetMultiplier("regular");
		
		// Make connections to HUD
		//hud = GameObject.Find("Canvas_HUD");
		if (hud != null)
		{
			hud.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
			hud.GetComponent<Canvas>().worldCamera = firstPersonCamera.GetComponent<Camera>();
			
			hud.transform.GetComponentsInChildren<VectorVisualizer>(false, radarLines);	
			hud_LateralVelocity = hud.transform.Find("Current Lateral Velocity").gameObject.GetComponent<TextMeshProUGUI>();
			hud_VerticalVelocity = hud.transform.Find("Current Vertical Velocity").gameObject.GetComponent<TextMeshProUGUI>();
		}

		timeSinceLastJump = jumpCoolDownTime;
    }

    void Update()
    {		
		// Count how long it's been since the player jumped.
		if (timeSinceLastJump < jumpCoolDownTime) timeSinceLastJump = Mathf.Clamp(timeSinceLastJump += 1.0f * Time.deltaTime, 0.0f, jumpCoolDownTime);
		// Count how long it's been since the player queued the next jump.
		if (jumpQueue_TimeSinceQueued < jumpQueue_Expiration) jumpQueue_TimeSinceQueued = Mathf.Clamp(jumpQueue_TimeSinceQueued += 1.0f * Time.deltaTime, 0.0f, jumpQueue_Expiration);
		if (jumpQueue_TimeSinceQueued == jumpQueue_Expiration) jumpQueue_isQueued = false;
		// Count how long it's been since the player became grounded.
		if (jumpQueue_timeSinceGrounded < jumpQueue_bHopGracePeriod) jumpQueue_timeSinceGrounded = Mathf.Clamp(jumpQueue_timeSinceGrounded += 1.0f * Time.deltaTime, 0.0f, jumpQueue_bHopGracePeriod);

		// Rotate arms to follow screen.
		//firstPersonObjects.rotation = Quaternion.RotateTowards(firstPersonObjects.rotation, Quaternion.Euler(firstPersonCamera.forward), 50.0f);
		
		//firstPersonObjects.rotation = Quaternion.RotateTowards(firstPersonObjects.rotation, Quaternion.Euler(firstPersonCamera.TransformDirection(Vector3.forward)), 50.0f);
		
		//firstPersonObjects.rotation = Quaternion.Euler(firstPersonCamera.forward);
		
		//firstPersonObjects.rotation = Quaternion.Euler(firstPersonCamera.TransformDirection(Vector3.forward));
		
		//firstPersonObjects.rotation = firstPersonCamera.rotation;
		
		firstPersonObjects.rotation = Quaternion.RotateTowards(firstPersonObjects.rotation, firstPersonCamera.rotation, 1.0f);
		
		
		/*
		// Inputs
		if (lookEnabled && !paused)
		{
			GetInput_Mouse();
			MouseLook();
		}
		
		if (moveEnabled && !paused)
		{
			GetInput_LateralMovement();
			
			if (holdJumpToKeepJumping && Input.GetButton("Jump"))
			{
				jumpQueue_isQueued = true;
				jumpQueue_TimeSinceQueued = 0.0f;
			}
				
			if (Input.GetButtonDown("Jump"))
			{
				if (isGrounded) Jump();
				else
				{
					jumpQueue_isQueued = true;
					jumpQueue_TimeSinceQueued = 0.0f;
				}
			}
		}
		*/
    }
	/*
	public void EnableLook(bool lookState)
	{
		lookEnabled = lookState;
	}
	
	public void EnableMove(bool moveState)
	{
		moveEnabled = moveState;
	}
	*/
	
	void FixedUpdate()
	{		
		// Convert the velocity vector from world space to local space.
		movement.vector.localVelocity = transform.InverseTransformDirection(playerRB.velocity);
		// In many cases we are only concerned with the lateral part of the local space velocity vector, so the vertical axis is zeroed out.
		movement.vector.localVelocity_Lateral = new Vector3(movement.vector.localVelocity.x, 0.0f, movement.vector.localVelocity.z);
		
		gravity = GetGravity();

		LateralMovement(); // Move the player based on the lateral movement input.
		
		// Slow the player down with "friction" if he is grounded and not trying to move.
		if (isGrounded && jumpQueue_timeSinceGrounded == jumpQueue_bHopGracePeriod) SimulateFriction();
		
		TerminalVelocity();
		
		if (hud != null) UpdateHUD(); // Update HUD elements.
	}
	
	void UpdateHUD()
	{
		hud_LateralVelocity.text = "Lateral Velocity: " + movement.vector.localVelocity_Lateral.magnitude.ToString("F2");
		hud_VerticalVelocity.text = "Vertical Velocity: " + movement.vector.localVelocity.y.ToString("F2");
			
		radarLines[0].SetVector(new Vector3((movement.vector.input.normalized * movement.targetSpeed).x, (movement.vector.input.normalized * movement.targetSpeed).z, -2.0f));
		radarLines[1].SetVector(new Vector3(movement.vector.localVelocity_Lateral.x, movement.vector.localVelocity_Lateral.z, -1.0f));
	}
	
	public void GetInput_LateralMovement()
	{
		// Get input for Movement from input manager and build a Vector3 to store the two inputs
		movement.vector.input = new Vector3 (Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
		// Limit the magnitude of the vector so that horizontal and vertical input doesn't stack to excede the indended maximum move speed
		movement.vector.input = Vector3.ClampMagnitude(movement.vector.input, 1.0f);
	}

	// Emulate Quake's "vector limiting" air strafe code to enable Bunny Hopping.
	private void LateralMovement()
	{
		// Calculate the target speed the player is trying to acclerate to (The player can move faster than this if he is bunny hopping, rocket jumping, or affected by outside sources of force).
		movement.targetSpeed = movement.groundedMax;
		
		// Reduce the target speed based on player input. (If the input is not maxed out, the player will walk instead of run (primarily for analogue input)).
		movement.targetSpeed *= movement.vector.input.magnitude;
		
		// Reduce the target speed based on the environment (no reduction when grounded (1.0f), but reduced in the air or in water.
		movement.targetSpeed *= movement.reduction.GetMultiplier();
		
		movement.accelerationRate = 140.0f;
		//if (!isGrounded) movement.accelerationRate = 20.0f;
		
		movement.accelerationRate *= movement.reduction.GetMultiplier();
		
		// Start by Projecting the player's current velocity vector onto the input acceleration direction (modified to have a length of 1.0f).
		// The Quake approach uses a dot product calculation as a roundabout way of getting the number we care about (magnitude) instead of doing a proper (expensive) vector projection.
		// The result gets the same number as Vector3.Project(movement.localVelocity_Lateral, movement.inputVector).magnitude which is the only part of the projection we care about.
		float projectedVelocity = Vector3.Dot(movement.vector.localVelocity_Lateral, movement.vector.input.normalized); // Vector projection of current velocity onto a unit vector input direction.
		
		float velocityToAdd = movement.accelerationRate * Time.fixedDeltaTime; // Accelerated velocity in direction of movment

		// Although the magnitude of the projected vector (same number as the result of the dot product calculation above) isn't representitive of the actual velocity, it can still be used for the speed comparison:
		// Calculate the difference between the speed player is asking to go, and the result of the "projected" vector (the dot product calculation).
		
		// If necessary, truncate the accelerated velocity so the vector projection does not exceed the target speed.
		if(projectedVelocity + velocityToAdd > movement.targetSpeed) velocityToAdd = movement.targetSpeed - projectedVelocity;
		
		
		// Prevent the player from going past the maximum allowed bHop speed.
		if (movement.vector.localVelocity_Lateral.magnitude + velocityToAdd > movement.bHopMax)
		{
			velocityToAdd = movement.bHopMax - movement.vector.localVelocity_Lateral.magnitude;
			// If it's negative make it possitive to prevent the infamous "Accelerated Back Hopping" glitch.
			//if (velocityToAdd < 0.0f) velocityToAdd = 0.0f - velocityToAdd;
			if (velocityToAdd < 0.0f) velocityToAdd = 0.0f;
		}
		
		// Apply the calculated force to the player in the requested direction in local space
		playerRB.AddRelativeForce(movement.vector.input.normalized * velocityToAdd, ForceMode.VelocityChange);	
	}
	
	private void SimulateFriction()
	{
		float rampDownMultiplier = 10.0f;
		rampDownMultiplier *= Time.fixedDeltaTime; // Multiply by Time.fixedDeltaTime so that friction speed is not bound to inconsitencies in the physics time step.
		
		Vector3 frictionForceToAdd = -movement.vector.localVelocity; // Start with a force in the opposite direction of the player's velocity.
		frictionForceToAdd -= transform.InverseTransformDirection(gravity);  // Counter gravity (in local space) so that the player won't slip off of edges.
		
		// Decide if vertical friction should be applied:
		//float verticalFrictionMultiplier = 1.0f; // We need some vertical counter force so the player doesn't slip off of edges.
		//if (frictionForceToAdd.y <= 0.02f) verticalFrictionMultiplier = 0.0f; // Don't slow down the player's force if he is trying to jump.
		float verticalFrictionMultiplier = 0.0f;
		
		// Multiply the rampDownMultiplier with the x and z components so that speed will ramp down for lateral movment, but the vertical friction will be instantanious if enabled.
		frictionForceToAdd = Vector3.Scale(frictionForceToAdd, new Vector3(rampDownMultiplier, verticalFrictionMultiplier, rampDownMultiplier));
		
		playerRB.AddRelativeForce(frictionForceToAdd, ForceMode.VelocityChange); // Apply the friction to the player in local space.
	}
		
	public void GetInput_Mouse()
	{
		if (matchXYSensitivity) mouseSensitivity_Y = mouseSensitivity_X;
		
		rotation_horizontal = (useRawMouseInput ? Input.GetAxisRaw("Mouse X") : Input.GetAxis("Mouse X")) * mouseSensitivity_X;
		rotation_vertical = (useRawMouseInput ? Input.GetAxisRaw("Mouse Y") : Input.GetAxis("Mouse Y")) * mouseSensitivity_Y * (invertVerticalInput ? -1.0f : 1.0f);
	}
	
	public void MouseLook()
	{
		float deltaTimeCompensation = 100.0f;
		float verticalAngle_Min = -90.0f;
		float verticalAngle_Max = 90.0f;
		
		// Up Down
		verticalAngle += rotation_vertical * Time.deltaTime * deltaTimeCompensation;
		verticalAngle = Mathf.Clamp(verticalAngle, verticalAngle_Min, verticalAngle_Max);
		firstPersonCamera.localRotation = Quaternion.Euler(Vector3.left * verticalAngle);
		
		// Left Right
		horizontalAngle = rotation_horizontal * Time.deltaTime * deltaTimeCompensation;
		transform.rotation *= Quaternion.Euler(new Vector3(0.0f, horizontalAngle, 0.0f));
	}
	
	public void QueueJump()
	{
		jumpQueue_isQueued = true;
		jumpQueue_TimeSinceQueued = 0.0f;
	}
	
	public void Jump()
	{
		if (timeSinceLastJump == jumpCoolDownTime)
		{
			timeSinceLastJump = 0.0f;
			playerRB.AddRelativeForce(Vector3.up * (jumpForceMultiplier + GetDownwardVelocity()) * playerRB.mass, ForceMode.Impulse);
			//SetIsGrounded(false);
		}
	}
	
	public float GetVerticalCameraAngle()
	{
		return verticalAngle;
	}
	
	public void SetIsGrounded(bool groundedState)
	{
		if (groundedState) // Check if the player just became grounded.
		{
			if (jumpQueue_isQueued) // If the player has a jump queued, jump without switching to ground movement.
			{
				Jump();
				jumpQueue_isQueued = false;
			}	
			else // If the player didn't have a jump queued, switch to ground movement and become grounded.
			{
				movement.reduction.SetMultiplier("regular");
				isGrounded = true;
			}
		}
		else
		{
			movement.reduction.SetMultiplier("air");
			isGrounded = false;
		}
	}
	
	public bool GetIsGrounded()
	{
		return isGrounded;
	}
	
	public Vector3 GetGravity()
	{
		if (gravitySource == null)
		{
			playerRB.useGravity = true;
			return Physics.gravity;
		}			
		else return gravitySource.GetGravityVector(transform);
	}
		
	public float GetDownwardVelocity()
	{
		if (isGrounded) return 0.0f; // If the player is grounded, then there is no downward velocity.
		
		// Calculate how fast the player is moving along his local vertical axis.
		Vector3 downwardVelocity = transform.InverseTransformDirection(playerRB.velocity); // Convert the vector to local space
		
		if (downwardVelocity.y > 0.0f || Mathf.Approximately(downwardVelocity.y, 0.0f)) return 0.0f; // If the player isn't moving vertically or the player is going up, then there is no downward velocity.
		else return Mathf.Abs(downwardVelocity.y); // If the player is falling, update the downward velocity to match.
	}
	
	public void TerminalVelocity()
	{
		float currentVertical = Mathf.Abs(movement.vector.localVelocity.y);
		float currentLateral = movement.vector.localVelocity_Lateral.magnitude;
		
		float cancelForce_Vertical = 0.0f;
		float cancelForce_Lateral = 0.0f;
		
		if (currentVertical > movement.verticalMax) cancelForce_Vertical = currentVertical - movement.verticalMax;
		if (currentLateral > movement.lateralMax) cancelForce_Lateral = currentLateral - movement.lateralMax;
		
		Vector3 cancelVector = -movement.vector.localVelocity_Lateral.normalized * cancelForce_Lateral;
		cancelVector += (Vector3.up * cancelForce_Vertical);
		
		if (cancelVector != Vector3.zero) playerRB.AddRelativeForce(cancelVector, ForceMode.VelocityChange);
	}
}
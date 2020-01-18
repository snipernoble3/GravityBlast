using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_GroundedCheck : MonoBehaviour
{
    public LayerMask groundedLayers;
	private Player_Movement playerMovement;
	private bool groundedState;
	
	void Awake()
	{
		playerMovement = transform.parent.GetComponent<Player_Movement>();
		groundedState = playerMovement.GetIsGrounded();
	}
	
	void OnTriggerEnter(Collider collision)
	{
		CheckGroundedState(collision, true);
	}
	
	// This check might not be necessary, it is a safety net incase the initial grounded check fails.
	void OnTriggerStay(Collider collision)
	{
		// If the player isn't already grounded, but there is still something colliding with the feet trigger, check if we need to set the bool to true.
		if (!groundedState) CheckGroundedState(collision, true);
	}
	
	void OnTriggerExit(Collider collision)
    {
		CheckGroundedState(collision, false);
    }
	
	void CheckGroundedState(Collider collision, bool newGroundedState)
	{
		// If the collision is on one of the groundedLayers, update the grounded state.
		// Check logic provided by https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
		if (groundedLayers == (groundedLayers | (1 << collision.gameObject.layer))) groundedState = newGroundedState;
	}
	
	void FixedUpdate()
	{
		// Keep the playerMovement script's isGrounded bool in sync with this script's groundedState bool.
		if (groundedState != playerMovement.GetIsGrounded()) playerMovement.SetIsGrounded(groundedState);
	}
}

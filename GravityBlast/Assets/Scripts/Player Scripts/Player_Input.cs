using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Input : MonoBehaviour
{
    [HideInInspector] public static Player_Input playerInput;
	[SerializeField] private Player_Movement movement;
	[SerializeField] private Player_BlastMechanics blastMechanics;
	
	private bool lookEnabled = true;
	private bool moveEnabled = true;
	private bool blastEnabled = true;
	private bool shootEnabled = true;
	
	[Header("Input Preferences")]
	public bool holdJumpToKeepJumping = false;
	
    void Awake()
    {
        // Hide the mouse cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
		
		if (playerInput == null) playerInput = this;
    }

    // Update is called once per frame
    void Update()
    {
	/*
		// Inputs
		if (lookEnabled)
		{
			movement.GetInput_Mouse();
			movement.MouseLook();
		}
		
		if (moveEnabled)
		{
			movement.GetInput_LateralMovement();
			
			if (holdJumpToKeepJumping && Input.GetButton("Jump"))
			{
				movement.QueueJump();
			}
				
			if (Input.GetButtonDown("Jump"))
			{
				if (movement.GetIsGrounded()) movement.Jump();
				else
				{
					movement.QueueJump();
				}
			}
			
			if (Input.GetButton("Crouch") && !movement.GetIsGrounded()) blastMechanics.AccelerateDown();
		}
		
		if (blastEnabled)
		{
			if (Input.GetButtonDown("Fire2") && blastMechanics.rjBlast_TimeSinceLastJump == blastMechanics.rjBlast_CoolDownTime) blastMechanics.RocketJumpCheck();
		}
	*/
    }
	
	public static void SetLookState(bool newState)
	{
		playerInput.lookEnabled = newState;
	}
	
	public static void SetMoveState(bool newState)
	{
		playerInput.moveEnabled = newState;
	}
	
	public static void SetBlastState(bool newState)
	{
		playerInput.blastEnabled = newState;
	}
	
	public static void SetShootState(bool newState)
	{
		playerInput.shootEnabled = newState;
	}
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Input : MonoBehaviour
{
    [HideInInspector] public static Player_Input playerInput;
	[SerializeField] private Player_Movement movement;
	[SerializeField] private Player_BlastMechanics blastMechanics;
	[SerializeField] private WeaponManager weaponManager;
	[SerializeField] private Animator firstPersonArms_Animator;
	
	private bool gameIsPaused = false;
	
	private bool lookEnabled = true;
	private bool moveEnabled = true;
	private bool blastEnabled = true;
	private bool shootEnabled = true;
    private bool devEnabled = true;
	
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
		if (gameIsPaused) return;
		
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
			
			if (Input.GetButton("Use"))
			{
				blastMechanics.Vacuum();
			}
			else if (Input.GetButtonUp("Use"))
			{
				blastMechanics.VacuumEnd();
			}				
		}
		
		if (shootEnabled)
		{
			if (Input.GetKeyDown(KeyCode.Alpha0)) weaponManager.SwitchWeapon(0);
			if (Input.GetKeyDown(KeyCode.Alpha1)) weaponManager.SwitchWeapon(0);
			if (Input.GetKeyDown(KeyCode.Alpha2)) weaponManager.SwitchWeapon(1);
			if (Input.GetKeyDown(KeyCode.Alpha3)) weaponManager.SwitchWeapon(2);

			if (Input.GetButton("Fire1"))
			{
				weaponManager.weapons[weaponManager.currentWeapon].GetComponent<IProjectileWeapon>().FireInput();
			}
			else
			{
				weaponManager.animator.SetBool("fire", false);
			}
			
			if (Input.GetButton("Reload"))
			{
				weaponManager.weapons[weaponManager.currentWeapon].GetComponent<IProjectileWeapon>().ReloadInput();
			}
		}

        if (devEnabled) {

            if (Input.GetKeyDown(KeyCode.G)) movement.gameObject.GetComponent<Player_Stats>().toggleGodMode();
            //if (Input.GetKeyDown(KeyCode.L)) { StartCoroutine(god.NextPlanet()); }

        }

    }
	
	public static void SetPauseState(bool newState) {
		playerInput.gameIsPaused = newState;
	}
	
	public static void SetLookState(bool newState) {
		playerInput.lookEnabled = newState;
	}
	
	public static void SetMoveState(bool newState) {
		playerInput.moveEnabled = newState;
	}
	
	public static void SetBlastState(bool newState)	{
		playerInput.blastEnabled = newState;
	}
	
	public static void SetShootState(bool newState)	{
		playerInput.shootEnabled = newState;
	}
}
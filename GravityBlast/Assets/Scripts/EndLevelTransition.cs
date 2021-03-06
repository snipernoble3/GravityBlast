﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelTransition : MonoBehaviour
{
	public Animator Jetpack_Animator;
	
	[SerializeField] private Canvas hud;
	[SerializeField] private GameObject stageCompletedMessage;
	
	[SerializeField] private Camera[] playerCameras;
	[SerializeField] private AudioListener playerListner;
	
	[SerializeField] private GameObject thirdPersonPlayer;
	[SerializeField] private GameObject player;

    //private Player_Stats playerStats;
	private bool jumpInitiated = false;
	
	public MusicManager musicManger;

    private Vector3 offset;
	
	void Start()
	{
		//playerStats = player.GetComponent<Player_Stats>();
        offset = player.transform.position - transform.position;
	}
	
	// Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B))
		if (Input.GetKeyDown(KeyCode.B) && GameManager.gm.playerStats.xpFull)
		{
			if (!jumpInitiated)
			{
				JumpToNextPlanet();
				musicManger.FadeOut(0.5f);
				StartCoroutine(musicManger.PlayTransition(musicManger.jetpack, false, false));
			}
        }
    }
	
	private void JumpToNextPlanet()
	{
		// Ensure that this method can't be called more than once.
		jumpInitiated = true;
		
		// Disable Player controls.
		Player_Input.SetLookState(false);
		Player_Input.SetMoveState(false);
		Player_Input.SetBlastState(false);
		Player_Input.SetShootState(false);
        GameManager.gm.playerStats.toggleGodMode();

		// Toggle Cameras
		for (int i = 0; i < playerCameras.Length; i++)
		{
			//cameras[i].SetActive(!cameras[i].activeInHierarchy);
			playerCameras[i].enabled = !playerCameras[i].enabled;
		}
		
		hud.enabled = false;
		
		thirdPersonPlayer.SetActive(true);
		thirdPersonPlayer.transform.SetParent(player.transform.parent);
		
		playerListner.enabled = false;
		
		Jetpack_Animator.Play("Open", 0, 0.0f);
		
		StartCoroutine(BlastOff());
        StartCoroutine(LoadNextLevel());
	}
	
	IEnumerator BlastOff() {
        Rigidbody playerRB = player.GetComponent<Rigidbody>();
        playerRB.velocity = Vector3.zero;
		yield return new WaitForSeconds(1.75f);
		playerRB.AddRelativeForce(Vector3.up * 50, ForceMode.VelocityChange);
		player.GetComponent<Gravity_AttractedObject>().blastOff = true;
		stageCompletedMessage.SetActive(true);
	}

    IEnumerator LoadNextLevel () {
        yield return new WaitForSeconds(4.5f);

        // Allow the method to be called again
        jumpInitiated = false;

        // Enable Player controls.
		Player_Input.SetLookState(true);
		Player_Input.SetMoveState(true);
		Player_Input.SetBlastState(true);
		Player_Input.SetShootState(true);
        GameManager.gm.playerStats.toggleGodMode();
        player.GetComponent<Gravity_AttractedObject>().blastOff = false;

        hud.enabled = true;
        stageCompletedMessage.SetActive(false);
        thirdPersonPlayer.SetActive(false);
        thirdPersonPlayer.transform.position = player.transform.position - offset;
        thirdPersonPlayer.transform.SetParent(player.transform);


        playerListner.enabled = true;

        Jetpack_Animator.StopPlayback();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.gm.StartCoroutine(GameManager.gm.NextPlanet());
    }

}

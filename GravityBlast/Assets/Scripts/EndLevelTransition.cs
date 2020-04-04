using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelTransition : MonoBehaviour
{
	public Animator Jetpack_Animator;
	
	[SerializeField] private Canvas hud;
	[SerializeField] private GameObject youWin;
	
	[SerializeField] private Camera[] playerCameras;
	[SerializeField] private AudioListener playerListner;
	
	[SerializeField] private GameObject thirdPersonPlayer;
	[SerializeField] private GameObject player;
	
	private PlayerResources resources;
	private bool jumpInitiated = false;
	
	void Start()
	{
		resources = player.GetComponent<PlayerResources>();
	}
	
	// Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B))
		if (Input.GetKeyDown(KeyCode.B) && resources.jumpReady)
		{
			if (!jumpInitiated) JumpToNextPlanet();
		}
    }
	
	private void JumpToNextPlanet()
	{
		// Ensure that this method can't be called more than once.
		jumpInitiated = true;
		
		// Disable Player controls.
		player.GetComponent<Player_Movement>().EnableLook(false);
		player.GetComponent<Player_Movement>().EnableMove(false);
		
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
	
	IEnumerator BlastOff()
	{
		yield return new WaitForSeconds(1.75f);
		player.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 50, ForceMode.VelocityChange);
		youWin.SetActive(true);
	}

    IEnumerator LoadNextLevel () {
        yield return new WaitForSeconds(6.75f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}

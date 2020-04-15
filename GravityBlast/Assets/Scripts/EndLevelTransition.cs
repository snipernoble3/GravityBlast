using System.Collections;
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

    [SerializeField] private God god;

    private Player_Stats playerStats;
	private bool jumpInitiated = false;
	
	void Start()
	{
		playerStats = player.GetComponent<Player_Stats>();
	}
	
	// Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B))
		if (Input.GetKeyDown(KeyCode.B) && playerStats.xpFull)
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
	
	IEnumerator BlastOff() {
		yield return new WaitForSeconds(1.75f);
		player.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 50, ForceMode.VelocityChange);
		stageCompletedMessage.SetActive(true);
	}

    IEnumerator LoadNextLevel () {
        yield return new WaitForSeconds(4.5f);

        // Allow the method to be called again
        jumpInitiated = false;

        // Enable Player controls.
        player.GetComponent<Player_Movement>().EnableLook(true);
        player.GetComponent<Player_Movement>().EnableMove(true);

        hud.enabled = true;
        stageCompletedMessage.SetActive(false);
        thirdPersonPlayer.SetActive(false);
        thirdPersonPlayer.transform.SetParent(player.transform);

        playerListner.enabled = true;

        Jetpack_Animator.StopPlayback();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        god.StartCoroutine(god.NextPlanet());
    }

}

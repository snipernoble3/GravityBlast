using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetHUD : MonoBehaviour
{
    [SerializeField] private GameObject planetHUDPrefab;
	private GameObject planetWireframe;
	private Player_Movement playerMovement;
	
	private Transform cameraPivot;
	private Transform camera;
	
	private float cameraDistance = 1.2f;
	
	// Start is called before the first frame update
    void Start()
    {
        if (Gravity_Source.DefaultGravitySource != null)
		{
			SetupPlanetHUD();
		}
    }
	
	private void SetupPlanetHUD()
	{
		planetWireframe = Instantiate(planetHUDPrefab, Gravity_Source.DefaultGravitySource.transform.position, Quaternion.identity) as GameObject;
		cameraPivot = planetWireframe.transform.Find("HUDPlanet_CameraPivot");
		camera = cameraPivot.transform.Find("HUDPlanet_Camera");
		
		cameraPivot.LookAt(transform, transform.forward); // Aim the pivot at the player.
		camera.localPosition = new Vector3(0.0f, 0.0f, cameraDistance); // Offset the camera to the appropriate distance to render the planet.
	}

    // Update is called once per frame
    void Update()
    {
        if (planetWireframe != null)
		{
			cameraPivot.LookAt(transform, transform.forward); // Aim the pivot at the player.
			camera.LookAt(cameraPivot, transform.forward); // Aim the pivot at the player.
		}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour {
    
    public static GameManager gm { get; private set; }

    [SerializeField] GameObject initialLoadingScreen;
    [SerializeField] GameObject difficultyButtons;
    [SerializeField] GameObject loading;
    //stat screen / level transition
    [SerializeField] GameObject statScreen;
    [SerializeField] TextMeshProUGUI completedStages;
    [SerializeField] TextMeshProUGUI stageTime;
    [SerializeField] TextMeshProUGUI stageKills;
    [SerializeField] TextMeshProUGUI survivalTime;
    [SerializeField] TextMeshProUGUI totalKills;
    //level tracker
    int completedPlanets = 0;
    int planetInSolarSystem = 0;
    int completedSolarSystems = 0;
    //player
    [SerializeField] GameObject playerPrefab;
    public GameObject player;
    public Player_Stats playerStats; 

    [SerializeField] public static int difficulty { get; private set; } = 1;//1, 2, 3
    //current solar system
    private PlanetInfo[] currSolarSystem;
    //current planet
    public static PlanetManager currPlanet { get; private set; }

	private MusicManager musicManger;

    float minPlanetScale = 0.45f;
    float maxPlanetScale = 0.75f;

    private bool nextPlanetReady = false;
    private bool playerReady = false;

    [SerializeField] MenuControl menu;
    PlanetTimer timer;

    public static bool paused { get; private set; } = false;
    private Vector3 pausedVelocity;

    private void Awake () {

        if (gm == null) gm = this;
        else Destroy(this);

        paused = false;
		
		timer = this.GetComponent<PlanetTimer>();

		musicManger = GetComponent<MusicManager>();
		player.GetComponent<EndLevelTransition>().musicManger = musicManger;

        StartCoroutine(StartGeneration());

    }

    private IEnumerator StartGeneration () {

        PauseGame(true);
        pausedVelocity = Vector3.zero;
        menu.gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.None, true);

        
        initialLoadingScreen.SetActive(true); //turn on loading screen
        difficultyButtons.SetActive(true);
        

        yield return new WaitUntil(() => playerReady);

        
        loading.SetActive(true);
        difficultyButtons.SetActive(false);
        

        PrefabManager.manager.StartCoroutine(PrefabManager.manager.GenerateObjectPools());

        yield return new WaitUntil(() => PrefabManager.poolsLoaded); // && difficulty selected

        GenerateSolarSystem();

        yield return null;

        FirstPlanet();

        yield return new WaitUntil(() => nextPlanetReady);

        
        initialLoadingScreen.SetActive(false); //remove loading screen
        
        menu.gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Locked, false);
        PauseGame(false);
        musicManger.enabled = true;
        nextPlanetReady = false;
        playerReady = false;

    }

    private void GenerateSolarSystem () {
        //generate x planets (info)
        int numPlanets = 3 + Random.Range(0, completedSolarSystems);
        currSolarSystem = new PlanetInfo[numPlanets];
        //Debug.Log("Planets in Solar System: " + currSolarSystem.Length);
        for (int i = 0; i < numPlanets; i++) {
            currSolarSystem[i] = GeneratePlanetInfo(completedPlanets + i);
        }

		// Set Skybox Color
        Material newSkybox = new Material(RenderSettings.skybox);
		float h, s, v;
		Color.RGBToHSV(newSkybox.GetColor("_Tint"), out h, out s, out v);
        h = Random.Range(0.0f, 1.0f);
		newSkybox.SetColor("_Tint", Color.HSVToRGB(h, s, v));
        RenderSettings.skybox = newSkybox;
        //DynamicGI.UpdateEnvironment(); // This refreshed the global lighting to match to color of the Skybox... Revisit this later.
        
    }

    private PlanetInfo GeneratePlanetInfo (int planetNumber) {

        int planetPrefab = Random.Range(0, PrefabManager.manager.planetPrefabs.Length);
        int stageNumber = planetNumber;
        int xpToAdvance = XPtoProceed(stageNumber);
        float size = Random.Range(minPlanetScale, maxPlanetScale);
        string sizeCategory = SizeClassOf(size);
        float lowestSurfacePoint;
        float highestSurfacePoint;
        CalculateSurfaceDistances(PrefabManager.manager.planetPrefabs[planetPrefab], out lowestSurfacePoint, out highestSurfacePoint);
        int moonMax = (int)(size + 0.5f) + (stageNumber / 5) + (difficulty - 1);
        int moons = (moonMax > 0) ? Random.Range(0, moonMax) : 0;
        PlanetInfo planet = new PlanetInfo(planetPrefab, stageNumber, xpToAdvance, size, sizeCategory, lowestSurfacePoint, highestSurfacePoint, moons);

        //Debug.Log("Planet " + planetNumber + " Info Generated");

        return planet;
    }
	
	private void CalculateSurfaceDistances(GameObject planet, out float lowest, out float highest)
	{
		Vector3[] positions = planet.GetComponent<MeshFilter>().sharedMesh.vertices;
		
		float[] distances = new float[positions.Length];
		
		for (int i = 0; i < positions.Length; i++)
		{
			distances[i] = (positions[i] - Vector3.zero).sqrMagnitude;
		}              
		
		System.Array.Sort(distances, positions);
		
		float sizeCompensation = 50.0f; // The result is 100 times too big, and we need radius instead of diameter so we use 50.
		lowest = distances[0] / sizeCompensation;
		highest = distances[distances.Length - 1] / sizeCompensation;
	}

    void FirstPlanet () {
        
        //create planet
        PlanetInfo planetToCreate = currSolarSystem[planetInSolarSystem];
        //player.transform.position = player.transform.position + (player.transform.up * 100);
        GameObject newPlanet = Instantiate(PrefabManager.manager.planetPrefabs[planetToCreate.planetPrefab], Vector3.zero, Quaternion.identity);
        currPlanet = newPlanet.GetComponent<PlanetManager>();
        currPlanet.SetInfo(planetToCreate);
        StartCoroutine(currPlanet.LoadPlanet());
        player.GetComponent<Gravity_AttractedObject>().CurrentGravitySource = currPlanet.GetComponentInChildren<Gravity_Source>();
        playerStats.SetXP(planetToCreate.xpToAdvance);
    }
	
	public void CheckRemainingEnemies()	{
		int enemiesRemaining = 0;
		int minEnemiesForActionMusic = 0;
        foreach (ObjectPool pool in PrefabManager.enemyPools) enemiesRemaining += pool.GetNumOfActiveEnemies();
		if (enemiesRemaining <= minEnemiesForActionMusic) //musicManger.CrossFade(0.75f);
		{
			musicManger.StartCoroutine(musicManger.PlayTransition(musicManger.outro, false, false));
			musicManger.CrossFade(0.0f);
			musicManger.FadeIn(musicManger.outro.length);
		}
	}

    public void NextPlanetReady () {

        RaycastHit hit = currPlanet.RandomSpawnPoint();
        player.transform.position = hit.point + (hit.normal * 75f);
        player.transform.rotation = Quaternion.LookRotation(hit.normal);
        player.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        nextPlanetReady = true;
        
    }

    public void PlayerReady () {
        playerReady = true;
    }

    public IEnumerator NextPlanet () {

        //PauseGameElements(true);
        PauseGame(true);
        pausedVelocity = Vector3.zero;

        //step forward in planet progression
        completedPlanets++;
        planetInSolarSystem++;

        

        //check to see if that was the end of the solar system
        if (planetInSolarSystem == currSolarSystem.Length) {
            completedSolarSystems++;
            GenerateSolarSystem();
            planetInSolarSystem = 0;
        }
        
        //destroy old planet
        if (currPlanet != null) {
            currPlanet.DestroyPlanet();
        }
        
        //create new planet
        PlanetInfo planetToCreate = currSolarSystem[planetInSolarSystem];
        
        GameObject newPlanet = Instantiate(PrefabManager.manager.planetPrefabs[planetToCreate.planetPrefab], Vector3.zero, Quaternion.identity);
        currPlanet = newPlanet.GetComponent<PlanetManager>();
        currPlanet.SetInfo(planetToCreate);
        StartCoroutine(currPlanet.LoadPlanet());
        player.GetComponent<Gravity_AttractedObject>().CurrentGravitySource = currPlanet.GetComponentInChildren<Gravity_Source>();
        playerStats.SetXP(planetToCreate.xpToAdvance);

        //overlay on
        statScreen.gameObject.SetActive(true);
        menu.gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.None, true);
        UpdateStatScreen();
        //player.transform.position = player.transform.position + (player.transform.up * 50);

        yield return new WaitUntil( () => nextPlanetReady && playerReady);
        //yield return new WaitForSeconds(5f);
		
		// Start music.
		musicManger.SwitchToAction();
		musicManger.PlayIntro();

        //turn off overlay
        statScreen.gameObject.SetActive(false);
        menu.gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Locked, false);
        timer.resetPlanetTime();
        //PauseGameElements(false);
        PauseGame(false);
        nextPlanetReady = false;
        playerReady = false;
        
    }

    private string SizeClassOf (float size) {

        //determine the class by how it compares to the size cutoffs for small med large
        if (size < 0.5) {
            return "small";
        } else if (size < 0.65) {
            return "medium";
        } else {
            return "large";
        }
    }

    private int XPtoProceed (int planetNumber) {
        int xp = 20;
        xp = (xp + (5 * planetNumber)) * difficulty;
        return xp;
    }

    private void UpdateStatScreen () {
        completedStages.text = "" + completedPlanets;
        stageTime.text = timer.getPlanetTime();
        //stage kills
        survivalTime.text = timer.getTotalTime();
        //total kills
    }

    /*
    public void PauseGameElements (bool isActive) {
        //send pause/unpause to all enemies
        currPlanet.GetComponent<PlanetManager>().PauseEnemies(isActive);
        //turn inputs off/on
        Player_Input.SetPauseState(isActive);
		//player.GetComponent<WeaponManager>().paused = isActive;
        //player.GetComponent<Player_BlastMechanics>().paused = isActive;
        //player.GetComponent<Player_Movement>().paused = isActive;
        player.GetComponent<Gravity_AttractedObject>().paused = isActive;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        timer.pauseTimer(isActive);
    }
    */
    
    public void PauseGame (bool pause) {
        paused = pause;
        if (paused) {
            //pausedVelocity = player.GetComponent<Rigidbody>().velocity;
            //player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //player.GetComponent<Player_Stats>().toggleGodMode();
            Time.timeScale = 0;
        } else {
            //player.GetComponent<Rigidbody>().velocity = pausedVelocity;
            //player.GetComponent<Player_Stats>().toggleGodMode();
            Time.timeScale = 1;
        }
    }
    
    public void PlayerDeath () {
        
        //PauseGameElements(false);
        //PauseGame(true); 

		// Disable Player controls.
		Player_Input.SetLookState(false);
		Player_Input.SetMoveState(false);
		Player_Input.SetBlastState(false);
		Player_Input.SetShootState(false);
		
		musicManger.StartCoroutine(musicManger.PlayTransition(musicManger.gameOver, true, false));
		StartCoroutine(ReturnToMenu(musicManger.gameOver.length));
        //EndScreen.SetActive(true);
    }
	
	public IEnumerator ReturnToMenu(float delay) {
        
        yield return new WaitForSeconds(delay);
        //PauseGame(false);
        menu.ReturnToMenu();
		//SceneManager.LoadScene("Menu");
	}

    public void SetDifficulty (int newDifficulty) {
        difficulty = newDifficulty;
        PlayerReady();
    }

}

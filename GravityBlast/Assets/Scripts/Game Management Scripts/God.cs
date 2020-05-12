using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class God : MonoBehaviour {
    
    public struct PlanetInfo {
        //planet model
        public GameObject prefab;
        //type?
        //difficulty/number of stage
        public int difficultySetting;
        public int stageNumber;
        public int xpToAdvance;
        //size category
        public float size;
        public string sizeCategory;
		public float lowestSurfacePoint;
		public float highestSurfacePoint;
		
        public GameObject[] staticEnvPrefabs;
        public GameObject[] clusterPrefabs;
        public GameObject[] plantPrefabs;
        public GameObject[] looseEnvPrefabs;
        public GameObject[] enemyPrefabs;
        //moon
        public bool hasMoon;
        public GameObject moonPrefab;
        //boss
        public bool hasBoss;
    }

    /*
    public struct EnemyInfo {
        public GameObject prefab;
        //float size; - if scale gets changed on different planets
        public int maxQuantity;
    }*/
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
    [SerializeField] public GameObject player;
    //
    [SerializeField] int difficulty = 1; //1, 2, 3
    //current solar system
    private PlanetInfo[] currSolarSystem;
    //current planet
    private GameObject currPlanet;
    //planet prefabs
    [SerializeField] private GameObject[] planetPrefabs;
    [SerializeField] private GameObject moonPrefab;
    //environment prefabs
    [SerializeField] private GameObject[] staticEnvPrefabs;
    [SerializeField] private GameObject[] looseEnvPrefabs;
    //plant prefabs
    [SerializeField] private GameObject[] clusterPrefabs;
    [SerializeField] private GameObject[] plantPrefabs;
    //enemy info
    [SerializeField] private GameObject[] enemyPrefabs;
    //[SerializeField] private int[] enemyMaxQuantities;
    //private EnemyInfo[] enemies;

    public ObjectPool[] enemyPools;

	private MusicManager musicManger;

    float minPlanetScale = 0.45f;
    float maxPlanetScale = 0.75f;

    private bool nextPlanetReady = false;
    private bool playerReady = false;

    [SerializeField] MenuControl menu;
    PlanetTimer timer;

    public static bool paused = false;
    private Vector3 pausedVelocity;

    private void Awake () {

        paused = false;

        player.GetComponent<Player_Stats>().SetGod(this);
		
		timer = this.GetComponent<PlanetTimer>();

		musicManger = GetComponent<MusicManager>();
		player.GetComponent<EndLevelTransition>().musicManger = musicManger;

        enemyPools = new ObjectPool[enemyPrefabs.Length];
        for (int i = 0; i < enemyPrefabs.Length; i++) {
            enemyPools[i] = new ObjectPool();
            enemyPools[i].objectPrefab = enemyPrefabs[i];
            enemyPools[i].god = this;
            enemyPools[i].CreateObjectPool();
        }
        

        GenerateSolarSystem();

        FirstPlanet();

    }

    /*
    void GenerateEnemyInfo () {
        enemies = new EnemyInfo[enemyPrefabs.Length];
        for (int i = 0; i < enemyPrefabs.Length; i++) {
            enemies[i] = new EnemyInfo();
            enemies[i].prefab = enemyPrefabs[i];
            if (i < enemyMaxQuantities.Length) {
                enemies[i].maxQuantity = enemyMaxQuantities[i];
            } else {
                enemies[i].maxQuantity = 50;
            }
        }


    }*/

    private void GenerateSolarSystem () {
        //generate x planets (info)
        int numPlanets = 3 + Random.Range(0, completedSolarSystems);
        currSolarSystem = new PlanetInfo[numPlanets];
        Debug.Log("Planets in Solar System: " + currSolarSystem.Length);
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
        PlanetInfo planet = new PlanetInfo();
        planet.prefab = planetPrefabs[Random.Range(0, planetPrefabs.Length)];
        planet.difficultySetting = difficulty;
        planet.stageNumber = planetNumber;
        planet.xpToAdvance = XPtoProceed(planet.difficultySetting, planet.stageNumber);
        planet.size = Random.Range(minPlanetScale, maxPlanetScale);
        planet.sizeCategory = SizeClassOf(planet.size);
        CalculateSurfaceDistances(planet.prefab, out planet.lowestSurfacePoint, out planet.highestSurfacePoint);
		planet.staticEnvPrefabs = staticEnvPrefabs;
        planet.looseEnvPrefabs = looseEnvPrefabs;
        planet.clusterPrefabs = clusterPrefabs;
        planet.plantPrefabs = plantPrefabs;
        planet.enemyPrefabs = enemyPrefabs;
        planet.hasMoon = true;
        planet.moonPrefab = moonPrefab;
        planet.hasBoss = false;
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
        currPlanet = Instantiate(planetToCreate.prefab, Vector3.zero, Quaternion.identity);
        currPlanet.GetComponent<PlanetManager>().SetInfo(planetToCreate);
        StartCoroutine(currPlanet.GetComponent<PlanetManager>().LoadPlanet());
        currPlanet.GetComponent<PlanetManager>().god = this;
        player.GetComponent<Gravity_AttractedObject>().SetGravitySource(currPlanet.GetComponent<Gravity_Source>());
        player.GetComponent<Player_Stats>().SetXP(planetToCreate.xpToAdvance);
    }
	
	public void CheckRemainingEnemies()	{
		int enemiesRemaining = 0;
		int minEnemiesForActionMusic = 0;
        foreach (ObjectPool pool in enemyPools) enemiesRemaining += pool.GetNumOfActiveEnemies();
		if (enemiesRemaining <= minEnemiesForActionMusic) //musicManger.CrossFade(0.75f);
		{
			musicManger.StartCoroutine(musicManger.PlayTransition(musicManger.outro, false, false));
			musicManger.CrossFade(0.0f);
			musicManger.FadeIn(musicManger.outro.length);
		}
	}

    public void NextPlanetReady () {
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
            currPlanet.GetComponent<PlanetManager>().DestroyPlanet();
        }
        
        //create new planet
        PlanetInfo planetToCreate = currSolarSystem[planetInSolarSystem];
        
        currPlanet = Instantiate(planetToCreate.prefab, Vector3.zero, Quaternion.identity);
        currPlanet.GetComponent<PlanetManager>().SetInfo(planetToCreate);
        StartCoroutine(currPlanet.GetComponent<PlanetManager>().LoadPlanet());
        currPlanet.GetComponent<PlanetManager>().god = this;
        player.GetComponent<Gravity_AttractedObject>().SetGravitySource(currPlanet.GetComponent<Gravity_Source>());
        player.GetComponent<Player_Stats>().SetXP(planetToCreate.xpToAdvance);

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

    private int XPtoProceed (int difficulty, int planetNumber) {
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
            pausedVelocity = player.GetComponent<Rigidbody>().velocity;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        } else {
            player.GetComponent<Rigidbody>().velocity = pausedVelocity;
        }
    }
    
    public void PlayerDeath () {
        //PauseGameElements(false);
        PauseGame(true);

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
		SceneManager.LoadScene("Menu");
	}

    public PlanetManager GetCurrPlanet () {
        return currPlanet.GetComponent<PlanetManager>();
    }

}

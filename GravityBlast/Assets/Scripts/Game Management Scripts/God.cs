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
    //level tracker
    int completedPlanets = 0;
    int planetInSolarSystem = 0;
    //player
    [SerializeField] GameObject playerPrefab;
    [SerializeField] public GameObject player;
    //
    [SerializeField] int difficulty = 1; //1, 2, 3
    //current solar system
    private PlanetInfo[] currSolarSystem;
    //current planet
    GameObject currPlanet;
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
	
	private MusicManager musicManger;

    float minPlanetScale = 0.45f;
    float maxPlanetScale = 0.75f;

    private bool nextPlanetReady = false;
    private bool playerReady = false;

    [SerializeField] MenuControl menu;

    private void Awake () {
        
        player.GetComponent<Player_Stats>().SetGod(this);
		musicManger = GetComponent<MusicManager>();
		player.GetComponent<EndLevelTransition>().musicManger = musicManger;

        GenerateSolarSystem();

        FirstPlanet();

    }

    private void Update () {

        //Test Inputs
        if (Input.GetKeyDown(KeyCode.L)) { StartCoroutine(NextPlanet()); }
        //if (statScreen.activeInHierarchy && Input.GetKeyDown(KeyCode.N)) { PlayerReady(); }

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
        int numPlanets = 3;
        currSolarSystem = new PlanetInfo[numPlanets];
        for (int i = 0; i < numPlanets; i++) {
            currSolarSystem[i] = GeneratePlanetInfo(completedPlanets + i);
        }
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

    public void NextPlanetReady () {
        nextPlanetReady = true;
    }

    public void PlayerReady () {
        playerReady = true;
    }

    public IEnumerator NextPlanet () {

        PauseGameElements(true);

        //step forward in planet progression
        completedPlanets++;
        planetInSolarSystem++;

        

        //check to see if that was the end of the solar system
        if (planetInSolarSystem == currSolarSystem.Length) {
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
		musicManger.PlayIntro();

        //turn off overlay
        statScreen.gameObject.SetActive(false);
        menu.gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Locked, false);
        PauseGameElements(false);
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
    }

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
    }

    public void PlayerDeath () {
        PauseGameElements(false);
        //EndScreen.SetActive(true);
        SceneManager.LoadScene("Menu");
    }

}

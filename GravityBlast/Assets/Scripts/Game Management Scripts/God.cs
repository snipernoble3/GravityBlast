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
        //size category
        public float size;
        public string sizeCategory;
        public GameObject[] plantPrefabs;
        public GameObject[] enemyPrefabs;
        //moon
        public bool hasMoon;
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
    [SerializeField] private GameObject player;
    //
    int difficulty = 1; //1, 2, 3
    //current solar system
    private PlanetInfo[] currSolarSystem;
    //current planet
    GameObject currPlanet;
    //planet prefabs
    [SerializeField] private GameObject[] planetPrefabs;
    //plant prefabs
    [SerializeField] private GameObject[] plantPrefabs;
    //enemy info
    [SerializeField] private GameObject[] enemyPrefabs;
    //[SerializeField] private int[] enemyMaxQuantities;
    //private EnemyInfo[] enemies;

    float minPlanetScale = 0.45f;
    float maxPlanetScale = 0.75f;

    private bool nextPlanetReady = false;
    private bool playerReady = false;

    [SerializeField] MenuControl menu;

    private void Awake () {
        
        //player.GetComponent<PlayerStats>().SetGod(this);

        GenerateSolarSystem();

        FirstPlanet();

    }

    private void Update () {
        if (Input.GetKeyDown(KeyCode.L)) {
            StartCoroutine(NextPlanet());
        }

        if (statScreen.activeInHierarchy && Input.GetKeyDown(KeyCode.N)) {
            PlayerReady();
        }
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
        planet.size = Random.Range(minPlanetScale, maxPlanetScale);
        planet.sizeCategory = SizeClassOf(planet.size);
        planet.plantPrefabs = plantPrefabs;
        planet.enemyPrefabs = enemyPrefabs;
        planet.hasMoon = false;
        planet.hasBoss = false;
        return planet;
    }

    void FirstPlanet () {
        //create player
        if (player == null) {
            player = Instantiate(playerPrefab);
        }
        //create planet
        PlanetInfo planetToCreate = currSolarSystem[planetInSolarSystem];
        player.transform.position = player.transform.position + (player.transform.up * 100);
        currPlanet = Instantiate(planetToCreate.prefab, Vector3.zero, Quaternion.identity);
        currPlanet.GetComponent<PlanetManager>().SetInfo(planetToCreate);
        StartCoroutine(currPlanet.GetComponent<PlanetManager>().LoadPlanet());
        currPlanet.GetComponent<PlanetManager>().god = this;
        player.GetComponent<Gravity_AttractedObject>().SetGravitySource(currPlanet.GetComponent<Gravity_Source>());
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
        //place player on planet

        //overlay on
        statScreen.gameObject.SetActive(true);
        menu.gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.None, true);
        UpdateStatScreen();
        player.transform.position = player.transform.position + (player.transform.up * 20);

        yield return new WaitUntil( () => nextPlanetReady && playerReady);
        //yield return new WaitForSeconds(5f);

        //turn off overlay
        statScreen.gameObject.SetActive(false);
        menu.gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Locked, false);
        PauseGameElements(false);
        nextPlanetReady = false;
        playerReady = false;
        
    }

    private string SizeClassOf (float size) {

        //determine the class by how it compares to the size cutoffs for small med large
        if (size < 0.6) {
            return "small";
        } else if (size < 0.8) {
            return "medium";
        } else {
            return "large";
        }
    }

    private void UpdateStatScreen () {
        completedStages.text = "" + completedPlanets;
    }

    public void PauseGameElements (bool pause) {
        //send pause/unpause to all enemies
        currPlanet.GetComponent<PlanetManager>().PauseEnemies(pause);
        //turn inputs off/on
        player.GetComponent<WeaponManager>().paused = pause;
        player.GetComponent<Player_BlastMechanics>().paused = pause;
        player.GetComponent<Player_Movement>().paused = pause;
        player.GetComponent<Gravity_AttractedObject>().paused = pause;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //level tracker
    int completedPlanets = 0;
    int planetInSolarSystem = 0;
    //player
    [SerializeField] GameObject player;
    //current solar system
    private PlanetInfo[] currSolarSystem;
    //current planet
    GameObject currPlanet;
    //planet prefabs
    [SerializeField] private GameObject[] planetPrefabs;
    //enemy info
    [SerializeField] private GameObject[] enemyPrefabs;
    //[SerializeField] private int[] enemyMaxQuantities;
    //private EnemyInfo[] enemies;

    float minPlanetScale = 0.5f;
    float maxPlanetScale = 1.0f;

    private void Awake () {
        //GenerateEnemyInfo();

        GenerateSolarSystem();

    }

    private void Update () {
        if (Input.GetKeyDown(KeyCode.L)) {
            NextPlanet();
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
        planet.difficultySetting = 1;
        planet.stageNumber = planetNumber;
        planet.size = Random.Range(minPlanetScale, maxPlanetScale);
        planet.sizeCategory = SizeClassOf(planet.size);
        planet.enemyPrefabs = enemyPrefabs;
        planet.hasMoon = false;
        planet.hasBoss = false;
        return planet;
    }

    void FirstPlanet () {
        PlanetInfo planetToCreate = currSolarSystem[planetInSolarSystem];
        currPlanet = Instantiate(planetToCreate.prefab, Vector3.zero, Quaternion.identity);
        currPlanet.GetComponent<PlanetManager>().SetInfo(planetToCreate);
        currPlanet.GetComponent<PlanetManager>().LoadPlanet();
        player.GetComponent<Gravity_AttractedObject>().SetGravitySource(currPlanet.GetComponent<Gravity_Source>());
    }

    public void NextPlanet () {

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
        currPlanet.GetComponent<PlanetManager>().LoadPlanet();
        player.GetComponent<Gravity_AttractedObject>().SetGravitySource(currPlanet.GetComponent<Gravity_Source>());
        
        //place player on planet


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

}

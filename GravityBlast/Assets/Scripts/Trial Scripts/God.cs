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
        //moon
        public bool hasMoon;
        //boss
        public bool hasBoss;
    }

    //level tracker
    int completedPlanets = 0;
    int planetInSolarSystem = 0;
    //player
    //current solar system
    private PlanetInfo[] currSolarSystem;
    //current planet
    GameObject currPlanet;
    //planet prefabs
    [SerializeField] private GameObject[] planetPrefabs;

    float minPlanetScale = 0.5f;
    float maxPlanetScale = 1.0f;

    private void Update () {
        if (Input.GetKeyDown(KeyCode.L)) {
            NextPlanet();
        }
    }


    private void GenerateSolarSystem () {
        //generate x planets (info)
    }

    private PlanetInfo GeneratePlanetInfo () {
        PlanetInfo planet = new PlanetInfo();
        planet.prefab = planetPrefabs[Random.Range(0, planetPrefabs.Length)];
        planet.difficultySetting = 1;
        planet.stageNumber = completedPlanets + 1;
        planet.size = Random.Range(minPlanetScale, maxPlanetScale);
        planet.sizeCategory = SizeClassOf(planet.size);
        planet.hasMoon = false;
        planet.hasBoss = false;
        return planet;
    }

    public void NextPlanet () {
        //destroy old planet
        if (currPlanet != null) {
            Destroy(currPlanet.gameObject);
        }
        //create new planet
        PlanetInfo planetToCreate = GeneratePlanetInfo();
        currPlanet = Instantiate(planetToCreate.prefab, Vector3.zero, Quaternion.identity);
        currPlanet.GetComponent<PlanetManager>().SetInfo(planetToCreate);
        currPlanet.GetComponent<PlanetManager>().LoadPlanet();
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

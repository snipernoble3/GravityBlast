using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {

    public God god;

    // PLANET INFO //
    God.PlanetInfo planet;
    [SerializeField] GameObject killBounds;
    //ready checks
    bool environmentReady;
    bool enemiesReady;
    bool moonReady;
    bool bossReady;
    bool lootReady;

    // ENVIRONMENT //
    GameObject environmentContainer; //for large env items and loose env items
    GameObject plantContainer; //for clusters and individual plants
    GameObject[,] staticEnvPool;
    int staticEnvCap = 15; //each
    GameObject[,] looseEnvPool;
    int looseEnvCap = 30; //each
    GameObject[,] clusterPool;
    int clusterCap = 20; //each
    GameObject[,] plantPool;
    int plantCap = 20; //each

    // ENEMIES //
    public GameObject enemyContainer;
    GameObject[,] enemyPool;
    int enemyCap;

    // MOON //
    GameObject[] moons;

    public IEnumerator LoadPlanet () { //called by game manager
        
        //set name
        name = "Planet " + planet.stageNumber;
        //set size
        transform.localScale = planet.size * Vector3.one;
		
		//Set planet colors.
		Material planetSurface = GetComponent<Renderer>().material;
		planetSurface.SetFloat("_Hue", Random.Range(0.0f, 1.0f));
		planetSurface.SetFloat("_SurfaceInner", planet.lowestSurfacePoint);
		planetSurface.SetFloat("_SurfaceOuter", planet.highestSurfacePoint);

        CreateComponents();
        yield return null;

        //spawn environment
        StartCoroutine(PopulateEnvironment());
        yield return new WaitUntil(() => environmentReady);
        //spawn enemies
        StartCoroutine(PopulateEnemies());
        yield return new WaitUntil(() => enemiesReady);
        //spawn moon if required
        if (planet.hasMoon) StartCoroutine(GenerateMoon(1)); else moonReady = true;
        yield return new WaitUntil(() => moonReady);
        //add boss if required
        bossReady = true;
        yield return new WaitUntil(() => bossReady);
        //add loot
        lootReady = true;
        yield return new WaitUntil(() => lootReady);

        //Setup MiniMap Planet
		Transform miniMap = transform.Find("MiniMap_Planet");
		
		//float cameraOffsetSize = planet.lowestSurfacePoint + planet.highestSurfacePoint;
		//float cameraDistance = 1.2f; // Pick a distance that MiniMap camera will be away from the planet.
		//cameraDistance += (planet.highestSurfacePoint - cameraOffsetSize) / miniMap.localScale.x;
		
		//miniMap.localScale = transform.localScale;
		miniMap.localScale = Vector3.one;
		miniMap.GetComponent<MeshFilter>().mesh = transform.GetComponent<MeshFilter>().mesh;
		
		Material topographyMat = miniMap.GetComponent<Renderer>().material;
		topographyMat.SetFloat("_SurfaceInner", planet.lowestSurfacePoint);
		topographyMat.SetFloat("_SurfaceOuter", planet.highestSurfacePoint);
		
		//Setup MiniMap Camera and Pivot
		Transform cameraPivot = miniMap.transform.Find("MiniMap_CameraPivot");
		Transform camera = cameraPivot.transform.Find("MiniMap_Camera");
		float cameraDistance = 120.0f;
		
		Transform player = GameObject.FindWithTag("Player").transform;
		cameraPivot.LookAt(player, player.forward); // Aim the pivot at the player.
		//camera.localPosition = new Vector3(0.0f, 0.0f, (planet.highestSurfacePoint * 2.0f) + (cameraDistance * miniMap.localScale.x)); // Offset the camera to the appropriate distance to render the planet.
		//camera.localPosition = new Vector3(0.0f, 0.0f, cameraDistance); // Offset the camera to the appropriate distance to render the planet.
		camera.localPosition = new Vector3(0.0f, 0.0f, cameraDistance * (camera.lossyScale.z / miniMap.lossyScale.z)); // Offset the camera to the appropriate distance to render the planet.
		camera.LookAt(cameraPivot, transform.forward); // Aim the camera at the planet.

        yield return null;

        god.NextPlanetReady();
    }

    public void DestroyPlanet () {
        //destruction animation?
        god.player.transform.parent = null;
        foreach (ObjectPool e in god.enemyPools) {
            e.DespawnAllObjects();
        }
        Destroy(this.gameObject);
    }

    void CreateComponents () {

        //create kill boundaries
        GameObject k = Instantiate(killBounds, Vector3.zero, Quaternion.identity, this.transform);
        //k.transform.localScale = 70f * Vector3.one;
        k.transform.localScale = (planet.lowestSurfacePoint -2f) * Vector3.one * 2f;
        killBounds = k;

        //set up empty containers
        environmentContainer = new GameObject();
        environmentContainer.name = "Environment Container";
        environmentContainer.transform.parent = this.gameObject.transform;

        plantContainer = new GameObject();
        plantContainer.name = "Plant Container";
        plantContainer.transform.parent = this.gameObject.transform;

        enemyContainer = new GameObject();
        enemyContainer.name = "Enemy Container";
        enemyContainer.transform.parent = this.gameObject.transform;

    }

    private IEnumerator PopulateEnvironment () {

        //generate spawn pools
        staticEnvPool = new GameObject[planet.staticEnvPrefabs.Length, staticEnvCap];
        looseEnvPool = new GameObject[planet.looseEnvPrefabs.Length, looseEnvCap];
        clusterPool = new GameObject[planet.clusterPrefabs.Length, clusterCap];
        plantPool = new GameObject[planet.plantPrefabs.Length, plantCap];

        //change ground textures

        yield return null;

        //spawn large constructs
        for (int i = 0; i < planet.staticEnvPrefabs.Length; i++) {
            int sRand = Random.Range(3, staticEnvCap);
            for (int j = 0; j < sRand; j++) { //spawn (<) x plants
                int rDist = Random.Range(-5, 10);
                Vector3 point = RandomSpawnPoint(rDist);
                Vector3 dir = (point - transform.position).normalized * 9.8f;
                staticEnvPool[i, j] = Instantiate(planet.staticEnvPrefabs[i], point, Quaternion.LookRotation(dir));
                staticEnvPool[i, j].transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                float rScale = Random.Range(5f, 15f);
                staticEnvPool[i, j].transform.localScale = Vector3.one * rScale;
                staticEnvPool[i, j].transform.parent = environmentContainer.transform;
            }
            yield return null;
        }
        yield return null;

        /*
        //spawn plant clusters
        for (int i = 0; i < planet.clusterPrefabs.Length; i++) {
            int cRand = Random.Range(0, clusterCap);
            for (int j = 0; j < cRand; j++) { //spawn (<) x plants
                Vector3 point = RandomSpawnPoint(0f);
                Vector3 dir = (point - transform.position).normalized * 9.8f;
                clusterPool[i, j] = Instantiate(planet.clusterPrefabs[i], point, Quaternion.LookRotation(dir));
                clusterPool[i, j].transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                clusterPool[i, j].transform.parent = plantContainer.transform;
            }
            yield return null;
        }
        yield return null;
        */

        //for each plant
        for (int i = 0; i < planet.plantPrefabs.Length; i++) {
            int pRand = Random.Range(0, plantCap);
            for (int j = 0; j < pRand; j++) { //spawn pRand plants
                Vector3 point = RandomSpawnPoint(-0.05f);
                Vector3 dir = (point - transform.position).normalized * 9.8f;
                plantPool[i, j] = Instantiate(planet.plantPrefabs[i], point, Quaternion.LookRotation(dir));
                plantPool[i, j].transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                plantPool[i, j].transform.Rotate(new Vector3(0, Random.value * 360f, 0), Space.Self);
                plantPool[i, j].transform.localScale = Random.Range(1f, 1.5f) * Vector3.one; 
                plantPool[i, j].transform.parent = plantContainer.transform;
            }
            yield return null;
        }
        yield return null;

        //spawn loose environment pieces
        for (int i = 0; i < planet.looseEnvPrefabs.Length; i++) {
            int lRand = Random.Range(5, looseEnvCap);
            for (int j = 0; j < lRand; j++) { //spawn (<) x plants
                Vector3 point = RandomSpawnPoint(0f);
                Vector3 dir = (point - transform.position).normalized * 9.8f;
                looseEnvPool[i, j] = Instantiate(planet.looseEnvPrefabs[i], point, Quaternion.LookRotation(dir));
                looseEnvPool[i, j].transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                looseEnvPool[i, j].transform.parent = environmentContainer.transform;
            }
            yield return null;
        }
        yield return null;


        environmentReady = true;
    }

    private IEnumerator PopulateEnemies () {

        CalculateEnemyCap();

        enemyPool = new GameObject[planet.enemyPrefabs.Length, enemyCap];
        //for each enemy
        for (int i = 0; i < planet.enemyPrefabs.Length; i++) {
            int rNum = Random.Range(planet.xpToAdvance/planet.enemyPrefabs.Length + 2, enemyCap);
            for (int j = 0; j < rNum; j++) { //spawn (<) x enemies
                //Vector3 point = RandomSpawnPoint();
                //enemyPool[i, j] = Instantiate(planet.enemyPrefabs[i], RandomSpawnPoint(0.5f), Quaternion.identity);
                //edit rotation?
                //enemyPool[i, j].transform.parent = enemyContainer.transform;
                god.enemyPools[i].SpawnObject();
            }
            yield return null;
        }

        enemiesReady = true;

        
    }

    private void CalculateEnemyCap () {
        int categoryMod = 1;
        
        switch (planet.sizeCategory) {
            case ("small"):
                categoryMod = 1;
                break;
            case ("medium"):
                categoryMod = 2;
                break;
            case ("large"):
                categoryMod = 3;
                break;
        }
        categoryMod *= 2;

        int difficultyMod = planet.difficultySetting * 15;

        int xpReq = planet.xpToAdvance / planet.enemyPrefabs.Length;

        enemyCap = xpReq + planet.stageNumber*categoryMod + difficultyMod;
        //stage 1 difficulty 1 cap = (xp/numEnemies) + 17
    }

    public Vector3 RandomSpawnPoint (float distanceBuffer = 0f) {
        Vector3 spawnPoint;
        //get random point on sphere larger than the surface
        Vector3 outer = Random.onUnitSphere * (planet.size * 80f);
        spawnPoint = outer;
        //direction to planet
        Vector3 dir = (outer - transform.position).normalized * -9.8f;
        //raycast to point on planet
        RaycastHit[] hits;
        //Physics.Raycast(outer, dir, out hit, 100f);
        //Debug.DrawLine(outer, hit.point, Color.green, 2f);
        //Vector3 spawnPoint = hit.point;
        hits = Physics.RaycastAll(outer, dir, Vector3.Distance(outer, transform.position));
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].transform == transform) {
                spawnPoint = hits[i].point - (dir.normalized * distanceBuffer);
                //Debug.DrawLine(outer, hits[i].point, Color.green, 2f);
            }
        }
        //return value
        return spawnPoint;
    }

    private IEnumerator GenerateMoon (int num = 1) {
        for (int i = 0; i < num; i++) {
            Vector3 point = RandomSpawnPoint(50f);
            Vector3 dir = (point - transform.position).normalized * 9.8f;
            GameObject moon = Instantiate(planet.moonPrefab, point, Quaternion.LookRotation(dir));
            moon.transform.parent = gameObject.transform;
            moon.GetComponent<Moon>().god = god;
            moon.GetComponent<Moon>().GenerateChest();

            yield return null;
        }

        moonReady = true;
    }
    
    public void SetInfo (God.PlanetInfo p) {
        planet = p;
    }
    
}

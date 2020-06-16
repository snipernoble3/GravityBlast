using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {

    // PLANET INFO //
    PlanetInfo planet;

    //ready checks
    bool environmentReady;
    bool enemiesReady;
    bool moonReady;
    bool bossReady;
    bool lootReady;

    // ENVIRONMENT //
    GameObject environmentContainer; //for large env items and loose env items
    GameObject plantContainer; //for clusters and individual plants
    int staticEnvCap = 15; //each
    int looseEnvCap = 30; //each
    int clusterCap = 10; //each
    int plantCap = 20; //each

    // ENEMIES //
    //GameObject enemyContainer;
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
        yield return null;

        CreateComponents();
        yield return null;

        //spawn environment
        StartCoroutine(PopulateEnvironment());
        yield return new WaitUntil(() => environmentReady);
        //spawn enemies
        StartCoroutine(PopulateEnemies());
        yield return new WaitUntil(() => enemiesReady);
        //spawn moon if required
        StartCoroutine(GenerateMoon(planet.moons));
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
		//camera.localPosition = new Vector3(0.0f, 0.0f, (planet.highestSurfacePoint * 2.0f) + (cameraDistance * miniMap.localScale.x)); // Offset the camera to the appropriate distance to render the planet.
		//camera.localPosition = new Vector3(0.0f, 0.0f, cameraDistance); // Offset the camera to the appropriate distance to render the planet.
		camera.localPosition = new Vector3(0.0f, 0.0f, cameraDistance * (camera.lossyScale.z / miniMap.lossyScale.z)); // Offset the camera to the appropriate distance to render the planet.
		cameraPivot.LookAt(player, player.forward); // Aim the pivot at the player.
		camera.LookAt(cameraPivot, player.forward); // Aim the camera at the planet.

        yield return null;

        GameManager.gm.NextPlanetReady();
    }

    public void DestroyPlanet () {
        //destruction animation?
        GameManager.gm.player.transform.parent = null;

        PrefabManager.DespawnAllPools();

        Destroy(this.gameObject);
    }

    void CreateComponents () {

        //set up empty containers
        environmentContainer = new GameObject("Environment Container");
        environmentContainer.transform.parent = this.gameObject.transform;

        plantContainer = new GameObject("Plant Container");
        plantContainer.transform.parent = this.gameObject.transform;

        //enemyContainer = new GameObject("Enemy Container");
        //enemyContainer.transform.parent = this.gameObject.transform;

    }

    private IEnumerator PopulateEnvironment () {
        
        //change ground textures

        yield return null;

        //spawn large constructs
        for (int i = 0; i < PrefabManager.manager.staticEnvPrefabs.Length; i++) {
            int sRand = Random.Range(3, staticEnvCap);
            for (int j = 0; j < sRand; j++) { //spawn (<) x plants
                int rDist = Random.Range(-5, 20);
                float rScale = Random.Range(5f, 15f);
                //Vector3 point = RandomSpawnPoint(out Vector3 dir, rDist);
                //dir = (point - transform.position).normalized * 9.8f;  // We might not need this line anymore now that the RandomSpawnPoint gives the surfaceNormal of the raycast hit.
                //RaycastHit hit = RandomSpawnPoint();
                //staticEnvPool[i, j] = Instantiate(planet.staticEnvPrefabs[i], hit.point + (hit.normal * rDist), Quaternion.LookRotation(hit.normal));
                //staticEnvPool[i, j].transform.Rotate(new Vector3(90, 0, 0), Space.Self);

                //staticEnvPool[i, j].transform.localScale = Vector3.one * rScale;
                //staticEnvPool[i, j].transform.parent = environmentContainer.transform;
                GameObject staticEnv = PrefabManager.staticEnvPools[i].SpawnObject();
                staticEnv.transform.localScale = rScale * Vector3.one;
                RaycastHit hit = RandomSpawnPoint();
                staticEnv.transform.position = hit.point + (hit.normal * rDist);
                staticEnv.transform.rotation = Quaternion.LookRotation(hit.normal);
                staticEnv.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                staticEnv.transform.Rotate(new Vector3(0, Random.value * 360f, 0), Space.Self);
                staticEnv.transform.parent = environmentContainer.transform;
                staticEnv.SetActive(true);
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
        for (int i = 0; i < PrefabManager.manager.plantPrefabs.Length; i++) {
            int pRand = Random.Range(5, plantCap);
            for (int j = 0; j < pRand; j++) { //spawn pRand plants
                //Vector3 point = RandomSpawnPoint(out Vector3 dir, -0.11f);
                //plantPool[i, j] = Instantiate(planet.plantPrefabs[i], hit.point + (hit.normal * -0.11f), Quaternion.LookRotation(hit.normal));
                //plantPool[i, j].transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                //plantPool[i, j].transform.Rotate(new Vector3(0, Random.value * 360f, 0), Space.Self);
                //plantPool[i, j].transform.localScale = Random.Range(1f, 1.5f) * Vector3.one; 
                //plantPool[i, j].transform.parent = plantContainer.transform;
                //Debug.DrawRay(point, dir * 20.0f, Color.white, 5.0f);

                GameObject plant = PrefabManager.plantPools[i].SpawnObject();
                plant.transform.localScale = Random.Range(1f, 1.5f) * Vector3.one;
                RaycastHit hit = RandomSpawnPoint();
                plant.transform.position = hit.point + (hit.normal * -0.11f);
                plant.transform.rotation = Quaternion.LookRotation(hit.normal);
                plant.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                plant.transform.Rotate(new Vector3(0, Random.value * 360f, 0), Space.Self);
                plant.transform.parent = plantContainer.transform;
                plant.SetActive(true);

            }
            yield return null;
        }
        yield return null;

        //spawn loose environment pieces
        for (int i = 0; i < PrefabManager.manager.looseEnvPrefabs.Length; i++) {
            int lRand = Random.Range(0, looseEnvCap);
            for (int j = 0; j < lRand; j++) { //spawn (<) x plants

                //Vector3 point = RandomSpawnPoint(out Vector3 dir);
                //dir = (point - transform.position).normalized * 9.8f;  // We might not need this line anymore now that the RandomSpawnPoint gives the surfaceNormal of the raycast hit.

                //looseEnvPool[i, j] = Instantiate(PrefabManager.looseEnvPrefabs[i], hit.point, Quaternion.LookRotation(hit.normal));
                //looseEnvPool[i, j].transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                //looseEnvPool[i, j].transform.parent = environmentContainer.transform;
                GameObject looseEnv = PrefabManager.looseEnvPools[i].SpawnObject();
                RaycastHit hit = RandomSpawnPoint();
                looseEnv.transform.position = hit.point + (hit.normal * 3f);
                looseEnv.transform.rotation = Quaternion.LookRotation(hit.normal);
                looseEnv.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                looseEnv.transform.localScale = Random.Range(1f, 2.5f) * Vector3.one;
                Gravity_AttractedObject attractedObject = looseEnv.GetComponent<Gravity_AttractedObject>();
                if (attractedObject != null) attractedObject.CurrentGravitySource = Gravity_Source.DefaultGravitySource;
                looseEnv.SetActive(true);
            }
            yield return null;
        }
        yield return null;


        environmentReady = true;
    }

    private IEnumerator PopulateEnemies () {

        CalculateEnemyCap();
        //Debug.Log("Enemy Cap: " + enemyCap);

        //for each enemy
        for (int i = 0; i < PrefabManager.manager.enemyPrefabs.Length; i++) {
            int rNum = Random.Range(planet.xpToAdvance/PrefabManager.manager.enemyPrefabs.Length + 2, enemyCap);
            for (int j = 0; j < rNum; j++) { //spawn (<) x enemies
                
                GameObject enemy = PrefabManager.enemyPools[i].SpawnObject();
                RaycastHit hit = RandomSpawnPoint();
                enemy.transform.position = hit.point + (hit.normal * 1f);
                EnemyInfo info = enemy.GetComponent<EnemyInfo>();
                info.playerStats = GameManager.gm.playerStats;
                Gravity_AttractedObject attractedObject = enemy.GetComponent<Gravity_AttractedObject>();
                if (attractedObject != null) attractedObject.CurrentGravitySource = Gravity_Source.DefaultGravitySource;
                enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
                enemy.SetActive(true);
                info.StartCoroutine(info.Stun(2f));
            }
            //Debug.Log(PrefabManager.manager.enemyPrefabs[i].name + " Spawned: " + rNum);
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

        int difficultyMod = GameManager.difficulty * 15;

        int xpReqPerEnemy = planet.xpToAdvance / PrefabManager.manager.enemyPrefabs.Length;

        enemyCap = xpReqPerEnemy + planet.stageNumber*categoryMod + difficultyMod;
        //stage 1 difficulty 1 cap = (xp/numEnemies) + 17
    }

    public RaycastHit RandomSpawnPoint () {
        Vector3 spawnPoint;
        //get random point on sphere larger than the surface
        Vector3 outer = Random.onUnitSphere * (planet.size * 80f);
        spawnPoint = outer;
        //direction to planet
        Vector3 dir = (outer - transform.position).normalized * -9.8f;
		//surfaceNormal = dir;
        //raycast to point on planet
        RaycastHit[] hits;
        //Physics.Raycast(outer, dir, out hit, 100f);
        //Debug.DrawLine(outer, hit.point, Color.green, 2f);
        //Vector3 spawnPoint = hit.point;
        
        hits = Physics.RaycastAll(outer, dir, Vector3.Distance(outer, transform.position));
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].transform.gameObject.tag == "Planet" || hits[i].transform.gameObject.tag == "StaticEnv") {
                //spawnPoint = hits[i].point - (dir.normalized * distanceBuffer);
                //surfaceNormal = hits[i].normal;
                //Debug.DrawLine(outer, hits[i].point, Color.green, 2f);
                return hits[i];
            }
        }

        //return value
        return hits[0];
    }

    private IEnumerator GenerateMoon (int num = 1) {
        for (int i = 0; i < num; i++) {
            float spawnDistance = Random.Range(35f, 50f);

            //Vector3 point = RandomSpawnPoint(out Vector3 dir, spawnDistance);
            //dir = (point - transform.position).normalized * 9.8f; // We might not need this line anymore now that the RandomSpawnPoint gives the surfaceNormal of the raycast hit.
            RaycastHit hit = RandomSpawnPoint();
            GameObject moon = Instantiate(PrefabManager.manager.moonPrefab, hit.point + (hit.normal * spawnDistance), Quaternion.LookRotation(hit.normal));
            moon.transform.parent = gameObject.transform;
            moon.GetComponent<Moon>().ChangeScale((spawnDistance / 100f) + Random.Range(0.4f, 0.7f));
            yield return null;
            moon.GetComponent<Moon>().GenerateChest();
            yield return null;
        }

        moonReady = true;
    }
    
    public void SetInfo (PlanetInfo p) {
        planet = p;
    }
}

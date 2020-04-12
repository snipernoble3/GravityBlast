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

    // PLANTS //
    GameObject plantContainer;
    //plant pool
    GameObject[,] plantPool;
    int plantCap = 50;

    // ENEMIES //
    GameObject enemyContainer;
    //enemy pool
    GameObject[,] enemyPool;
    int enemyCap;

    public IEnumerator LoadPlanet () { //called by game manager
        
        //set name
        name = "Planet " + planet.stageNumber;
        //set size
        transform.localScale = planet.size * Vector3.one;
        //create kill boundaries
        GameObject k = Instantiate(killBounds, Vector3.zero, Quaternion.identity, this.transform);
        //scale kill boundaries - 70 scale
        k.transform.localScale = 70f * Vector3.one;
        yield return null;
        //spawn environment
        StartCoroutine(PopulateEnvironment());
        yield return new WaitUntil(() => environmentReady);
        //spawn enemies
        StartCoroutine(PopulateEnemies());
        yield return new WaitUntil(() => enemiesReady);
        //spawn moon if required
        if (planet.hasMoon) GenerateMoon(); else moonReady = true;
        yield return new WaitUntil(() => moonReady);
        //add boss if required
        bossReady = true;
        yield return new WaitUntil(() => bossReady);
        //add loot
        lootReady = true;
        yield return new WaitUntil(() => lootReady);

        yield return new WaitUntil(() => PlanetLoaded());

        god.NextPlanetReady();

    }

    private bool PlanetLoaded () {
        
        return true;
    }

    public void DestroyPlanet () {
        //destruction animation?

        Destroy(this.gameObject);
    }

    private IEnumerator PopulateEnvironment () {

        plantContainer = new GameObject();
        plantContainer.name = "Plant Container";
        plantContainer.transform.parent = this.gameObject.transform;

        plantPool = new GameObject[planet.plantPrefabs.Length, plantCap];
        //for each plant
        for (int i = 0; i < planet.plantPrefabs.Length; i++) {
            int num = Random.Range(0, plantCap);
            for (int j = 0; j < num; j++) { //spawn (<) x plants
                Vector3 point = RandomSpawnPoint(0f);
                Vector3 dir = (point - transform.position).normalized * 9.8f;
                plantPool[i, j] = Instantiate(planet.plantPrefabs[i], point, Quaternion.LookRotation(dir));
                plantPool[i, j].transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                plantPool[i, j].transform.parent = plantContainer.transform;
            }
            yield return null;
        }

        //spawn ground textures

        //spawn large constructs
        yield return new WaitForSeconds(0.01f);

        //spawn plant clusters / small constructs
        //yield return new WaitForSeconds(0.01f);

        //spawn random plants
        yield return new WaitForSeconds(0.01f);

        //spawn loose environment pieces
        //yield return new WaitForSeconds(0.01f);

        environmentReady = true;
    }

    private IEnumerator PopulateEnemies () {

        CalculateEnemyCap();

        enemyContainer = new GameObject();
        enemyContainer.name = "Enemy Container";
        enemyContainer.transform.parent = this.gameObject.transform;

        enemyPool = new GameObject[planet.enemyPrefabs.Length, enemyCap];
        //for each enemy
        for (int i = 0; i < planet.enemyPrefabs.Length; i++) {
            int rNum = Random.Range(planet.xpToAdvance/planet.enemyPrefabs.Length + 2, enemyCap);
            for (int j = 0; j < rNum; j++) { //spawn (<) x enemies
                //Vector3 point = RandomSpawnPoint();
                enemyPool[i, j] = Instantiate(planet.enemyPrefabs[i], RandomSpawnPoint(0.5f), Quaternion.identity);
                //edit rotation?
                enemyPool[i, j].transform.parent = enemyContainer.transform;
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

    Vector3 RandomSpawnPoint (float distanceBuffer) {
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

    private void GenerateMoon () {

    }
    
    public void SetInfo (God.PlanetInfo p) {
        planet = p;
    }

    public void PauseEnemies (bool pause) {
        
    }

}

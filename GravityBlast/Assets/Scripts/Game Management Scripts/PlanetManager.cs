using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {

    public God god;

    // PLANET INFO //
    God.PlanetInfo planet;
    [SerializeField] GameObject killBounds;

    // PLANTS //
    //plant pool
    GameObject[,] plantPool;

    // ENEMIES //
    //enemy pool
    GameObject[,] enemyPool;

    public IEnumerator LoadPlanet () { //called by game manager
        
        //set name
        name = "Planet " + planet.stageNumber;
        //set size
        transform.localScale = planet.size * Vector3.one;
        //create kill boundaries
        //GameObject k = Instantiate(killBounds, Vector3.zero, Quaternion.identity, this.transform);
        //scale kill boundaries - 70 scale
        //k.transform.localScale = 70f * Vector3.one;
        //spawn environment
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(PopulateEnvironment());
        //spawn enemies
        yield return new WaitForSeconds(0.1f);
        PopulateEnemies();
        //spawn moon if required
        yield return new WaitForSeconds(0.1f);
        if (planet.hasMoon) GenerateMoon();
        //add boss if required

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

        plantPool = new GameObject[planet.plantPrefabs.Length, 75];
        //for each enemy
        for (int i = 0; i < planet.plantPrefabs.Length; i++) {
            for (int j = 0; j < 75; j++) { //spawn (<) x plants
                Vector3 point = RandomSpawnPoint(0f);
                Vector3 dir = (point - transform.position).normalized * 9.8f;
                plantPool[i, j] = Instantiate(planet.plantPrefabs[i], point, Quaternion.LookRotation(dir));
                plantPool[i, j].transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                plantPool[i, j].transform.parent = transform;
                j++;
            }
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

    }

    private void PopulateEnemies () {
        enemyPool = new GameObject[planet.enemyPrefabs.Length, 50];
        //for each enemy
        for (int i = 0; i < planet.enemyPrefabs.Length; i++) {
            for (int j = 0; j < 50; j++) { //spawn (<) x enemies
                //Vector3 point = RandomSpawnPoint();
                enemyPool[i, j] = Instantiate(planet.enemyPrefabs[i], RandomSpawnPoint(0.5f), Quaternion.identity);
                //edit rotation?
                enemyPool[i, j].transform.parent = transform;
                j++;
            }
        }

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

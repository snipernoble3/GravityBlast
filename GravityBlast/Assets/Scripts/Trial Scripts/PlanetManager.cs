using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {

    struct EnemyInfo {
        GameObject prefab;
        float size;
        int maxQuantity;
    }

    // PLANET INFO //
    God.PlanetInfo planet;
    [SerializeField] GameObject killBounds;

    // ENEMIES //
    //enemies to spawn
    EnemyInfo[] enemyInfos;
    //enemy pool
    GameObject[][] enemyPool;

    public void LoadPlanet () { //called by game manager
        //set size
        transform.localScale = planet.size * Vector3.one;
        //create kill boundaries
        GameObject k = Instantiate(killBounds, Vector3.zero, Quaternion.identity, this.transform);
        //scale kill boundaries - 70 scale
        k.transform.localScale = 70f * Vector3.one;
        //spawn environment
        PopulateEnvironment();
        //spawn enemies
        PopulateEnemies();
        //spawn moon if required

        //add boss if required


    }

    public void DestroyPlanet () {
        //destruction animation?
        //remove enemies
    }

    private void PopulateEnvironment () {
        //spawn ground textures

        //spawn large constructs

        //spawn plant clusters / small constructs

        //spawn random plants

    }

    private void PopulateEnemies () {

        //for each enemy :        
        //determine how many enemies to spawn

        //spawn them randomly around the surface

    }

    private void GenerateMoon () {

    }
    
    public void SetInfo (God.PlanetInfo p) {
        planet = p;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour {

    public static PrefabManager manager { get; private set; }

    [SerializeField] public GameObject[] planetPrefabs;
    [SerializeField] public GameObject moonPrefab;

    [SerializeField] public GameObject[] staticEnvPrefabs;
    public static ObjectPool[] staticEnvPools { get; private set; }

    //public static GameObject[] clusterPrefabs;
    //public static ObjectPool[] clusterPools { get; private set; }

    [SerializeField] public GameObject[] plantPrefabs;
    public static ObjectPool[] plantPools { get; private set; }

    [SerializeField] public GameObject[] looseEnvPrefabs;
    public static ObjectPool[] looseEnvPools { get; private set; }

    [SerializeField] public GameObject[] enemyPrefabs;
    public static ObjectPool[] enemyPools { get; private set; }

    [SerializeField] public GameObject xpPrefab;
    public static ObjectPool xpPool { get; private set; }

    public static bool poolsLoaded { get; private set; }



    private void Start () {
        if (manager == null) manager = this;
        else Destroy(this);
        
    }

    public IEnumerator GenerateObjectPools () {
        //Create Object Pools

        staticEnvPools = new ObjectPool[staticEnvPrefabs.Length];
        for (int i = 0; i < staticEnvPrefabs.Length; i++) {
            staticEnvPools[i] = new ObjectPool(staticEnvPrefabs[i], 50);
        }

        yield return null;

        /*
        clusterPools = new ObjectPool[clusterPrefabs.Length];
        for (int i = 0; i < clusterPrefabs.Length; i++) {
            clusterPools[i] = new ObjectPool(clusterPrefabs[i], 50);
        }

        yield return null;
        */

        plantPools = new ObjectPool[plantPrefabs.Length];
        for (int i = 0; i < plantPrefabs.Length; i++) {
            plantPools[i] = new ObjectPool(plantPrefabs[i], 50);
        }

        yield return null;

        looseEnvPools = new ObjectPool[looseEnvPrefabs.Length];
        for (int i = 0; i < looseEnvPrefabs.Length; i++) {
            looseEnvPools[i] = new ObjectPool(looseEnvPrefabs[i], 50);
        }

        yield return null;

        enemyPools = new ObjectPool[enemyPrefabs.Length];
        for (int i = 0; i < enemyPrefabs.Length; i++) {
            enemyPools[i] = new ObjectPool(enemyPrefabs[i], 100);
        }

        yield return null;

        xpPool = new ObjectPool(xpPrefab, 100 * enemyPools.Length);

        yield return null;

        poolsLoaded = true;

    }

    public static void DespawnAllPools () {

        foreach (ObjectPool se in PrefabManager.staticEnvPools) {
            se.DespawnAllObjects();
        }

        /*
        foreach (ObjectPool ce in PrefabManager.clusterPools) {
            c.DespawnAllObjects();
        }
        */

        foreach (ObjectPool p in PrefabManager.plantPools) {
            p.DespawnAllObjects();
        }

        foreach (ObjectPool le in PrefabManager.looseEnvPools) {
            le.DespawnAllObjects();
        }

        foreach (ObjectPool e in PrefabManager.enemyPools) {
            e.DespawnAllObjects();
        }

        PrefabManager.xpPool.DespawnAllObjects();
    }

}

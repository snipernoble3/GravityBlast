using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class ObjectPool : MonoBehaviour
public class ObjectPool
{
    public God god;
    
    // Enemy Pool
	private int objectPoolSize = 128;
	private Queue<GameObject> objectQueue;
	private List<GameObject> activeObjects;
	public GameObject objectPrefab;
	
    public void CreateObjectPool()
    {
        objectQueue = new Queue<GameObject>();
		activeObjects = new List<GameObject>();
	
		for (int i = 0; i < objectPoolSize; i++)
		{
			GameObject enemyInstance = GameObject.Instantiate(objectPrefab); // Create an enemy that can be used in the pool.
			enemyInstance.GetComponent<Health>().manager = this; // Pass in a reference so that the enemy can talk back to this script.
			enemyInstance.SetActive(false); // Deactiveate the enemy after creation so it sits idle in the pool.
            enemyInstance.transform.parent = god.gameObject.transform;
			objectQueue.Enqueue(enemyInstance); // Add this newly created enemy into the pool for later use.
		}
    }
	
	public void SpawnObject()
	{
		if (objectQueue.Count != 0)
		{
			GameObject spawnedEnemy = objectQueue.Dequeue(); // Spawn an enemy by pulling it out of the queue.
			
			spawnedEnemy.SetActive(true);
			
			// If the enemy is not already in the list of active enemies, then add it.
			if(!activeObjects.Contains(spawnedEnemy)) activeObjects.Add(spawnedEnemy);

            // Set Spawn Postion
            spawnedEnemy.transform.position = god.GetCurrPlanet().RandomSpawnPoint(1f);
            spawnedEnemy.GetComponent<Gravity_AttractedObject>().SetGravitySource(god.GetCurrPlanet().gameObject.GetComponent<Gravity_Source>());
            spawnedEnemy.transform.parent = god.GetCurrPlanet().enemyContainer.transform;
		} else {
            //generate more enemies
        }
	}
		
	public void DespawnObject(GameObject despawnedEnemy, bool removeFromActive = true)
	{
        despawnedEnemy.transform.parent = god.gameObject.transform;
        objectQueue.Enqueue(despawnedEnemy);
		if(removeFromActive && activeObjects.Contains(despawnedEnemy)) activeObjects.RemoveAt(activeObjects.IndexOf(despawnedEnemy));
		despawnedEnemy.SetActive(false);
		
		god.CheckRemainingEnemies();
	}
	
    public void DespawnAllObjects () {

        foreach (GameObject enemy in activeObjects) {
            DespawnObject(enemy, false);
        }

        activeObjects = new List<GameObject>();

    }

	public int GetNumOfActiveEnemies()
	{
		return activeObjects.Count;
	}
}
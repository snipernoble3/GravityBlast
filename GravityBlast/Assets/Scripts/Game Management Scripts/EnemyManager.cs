using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class EnemyManager : MonoBehaviour
public class EnemyManager
{
    public God god;
    
    // Enemy Pool
	private int enemyPoolSize = 128;
	private Queue<GameObject> enemyQueue;
	private List<GameObject> activeEnemies;
	public GameObject enemyPrefab;
	
    public void CreateEnemyPool()
    {
        enemyQueue = new Queue<GameObject>();
		activeEnemies = new List<GameObject>();
	
		for (int i = 0; i < enemyPoolSize; i++)
		{
			GameObject enemyInstance = GameObject.Instantiate(enemyPrefab); // Create an enemy that can be used in the pool.
			enemyInstance.GetComponent<Health>().manager = this; // Pass in a reference so that the enemy can talk back to this script.
			enemyInstance.SetActive(false); // Deactiveate the enemy after creation so it sits idle in the pool.
            enemyInstance.transform.parent = god.gameObject.transform;
			enemyQueue.Enqueue(enemyInstance); // Add this newly created enemy into the pool for later use.
		}
    }
	
	public void SpawnEnemy()
	{
		if (enemyQueue.Count != 0)
		{
			GameObject spawnedEnemy = enemyQueue.Dequeue(); // Spawn an enemy by pulling it out of the queue.
			
			spawnedEnemy.SetActive(true);
			
			// If the enemy is not already in the list of active enemies, then add it.
			if(!activeEnemies.Contains(spawnedEnemy)) activeEnemies.Add(spawnedEnemy);

            // Set Spawn Postion
            spawnedEnemy.transform.position = god.GetCurrPlanet().RandomSpawnPoint(1f);
            spawnedEnemy.GetComponent<Gravity_AttractedObject>().SetGravitySource(god.GetCurrPlanet().gameObject.GetComponent<Gravity_Source>());
            spawnedEnemy.transform.parent = god.GetCurrPlanet().enemyContainer.transform;
		} else {
            //generate more enemies
        }
	}
		
	public void DespawnEnemy(GameObject despawnedEnemy, bool removeFromActive = true)
	{
        despawnedEnemy.transform.parent = god.gameObject.transform;
        enemyQueue.Enqueue(despawnedEnemy);
		if(removeFromActive && activeEnemies.Contains(despawnedEnemy)) activeEnemies.RemoveAt(activeEnemies.IndexOf(despawnedEnemy));
		despawnedEnemy.SetActive(false);
		
		god.CheckRemainingEnemies();
	}
	
    public void DespawnAllEnemies () {

        foreach (GameObject enemy in activeEnemies) {
            DespawnEnemy(enemy, false);
        }

        activeEnemies = new List<GameObject>();

    }

	public int GetNumOfActiveEnemies()
	{
		return activeEnemies.Count;
	}
}
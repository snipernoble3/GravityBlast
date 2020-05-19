﻿using System.Collections;
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

        IncreasePool(objectPoolSize);
    }

    public void IncreasePool (int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject objectInstance = GameObject.Instantiate(objectPrefab); // Create an object that can be used in the pool.
            if (objectInstance.GetComponent<Health>()) objectInstance.GetComponent<Health>().pool = this; // Enemy Hookup - Pass in a reference so that the enemy can talk back to this script.
            objectInstance.SetActive(false); // Deactiveate the object after creation so it sits idle in the pool.
            objectInstance.transform.parent = god.gameObject.transform;
            objectQueue.Enqueue(objectInstance); // Add this newly created object into the pool for later use.
        }
    }

    public void SpawnObject(Transform parent = null)
	{
		if (objectQueue.Count != 0)
		{
			GameObject spawnedObject = objectQueue.Dequeue(); // Spawn an enemy by pulling it out of the queue.
			
			spawnedObject.SetActive(true);
			
			// If the enemy is not already in the list of active enemies, then add it.
			if(!activeObjects.Contains(spawnedObject)) activeObjects.Add(spawnedObject);

            // Set Spawn Postion
            Vector3 dir = Vector3.zero; // This gets thrown away, but the out parameter below will complain without it.
			spawnedObject.transform.position = god.GetCurrPlanet().RandomSpawnPoint(out dir, 1.0f);
            
			
			Gravity_AttractedObject attractedObject = spawnedObject.GetComponent<Gravity_AttractedObject>();
			
			if (attractedObject != null) attractedObject.CurrentGravitySource = god.GetCurrPlanet().gameObject.GetComponent<Gravity_Source>();
            spawnedObject.transform.parent = parent;
		}
		else
		{
            IncreasePool(20); //generate more enemies
            SpawnObject(parent); //attempt to spawn the enemy again
        }
	}
		
	public void DespawnObject(GameObject despawnedObject, bool removeFromActive = true)
	{
        despawnedObject.transform.parent = god.gameObject.transform;
        objectQueue.Enqueue(despawnedObject);
		if(removeFromActive && activeObjects.Contains(despawnedObject)) activeObjects.RemoveAt(activeObjects.IndexOf(despawnedObject));
		despawnedObject.SetActive(false);
		
		if (despawnedObject.GetComponent<Health>()) god.CheckRemainingEnemies();
	}
	
    public void DespawnAllObjects () {

        foreach (GameObject obj in activeObjects) {
            DespawnObject(obj, false);
        }

        activeObjects = new List<GameObject>();

    }

	public int GetNumOfActiveEnemies()
	{
		return activeObjects.Count;
	}
}
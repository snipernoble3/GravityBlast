﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class ObjectPool : MonoBehaviour
public class ObjectPool {
    
    // Enemy Pool
	private int objectPoolSize = 128;
	private Queue<GameObject> objectQueue;
	private List<GameObject> activeObjects;
	public GameObject objectPrefab;
	
    public ObjectPool (GameObject prefab, int startingSize = 0) {
        objectPrefab = prefab;
        CreateObjectPool(startingSize);
    }

    public void CreateObjectPool(int size = 0)
    {
        objectQueue = new Queue<GameObject>();
		activeObjects = new List<GameObject>();

        if (size == 0) {
            IncreasePool(objectPoolSize);
        } else {
            IncreasePool(size);
        }
        
    }

    public void IncreasePool (int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject objectInstance = GameObject.Instantiate(objectPrefab); // Create an object that can be used in the pool.
            if (objectInstance.GetComponent<Health>()) objectInstance.GetComponent<Health>().objectPool = this; // Enemy Hookup - Pass in a reference so that the enemy can talk back to this script.
            if (objectInstance.GetComponent<EnemyInfo>()) objectInstance.GetComponent<EnemyInfo>().objectPool = this;
            if (objectInstance.GetComponent<CollectablePickup>()) objectInstance.GetComponent<CollectablePickup>().objectPool = this;
            objectInstance.SetActive(false); // Deactiveate the object after creation so it sits idle in the pool.
            objectInstance.transform.parent = GameManager.gm.gameObject.transform;
            objectQueue.Enqueue(objectInstance); // Add this newly created object into the pool for later use.
        }
    }

    public GameObject SpawnObject()
	{
		if (objectQueue.Count != 0)
		{
			GameObject spawnedObject = objectQueue.Dequeue(); // Spawn an enemy by pulling it out of the queue.
			
			//spawnedObject.SetActive(true);
			
			// If the enemy is not already in the list of active enemies, then add it.
			if(!activeObjects.Contains(spawnedObject)) activeObjects.Add(spawnedObject);

            Gravity_AttractedObject attractedObject = spawnedObject.GetComponent<Gravity_AttractedObject>();
            if (attractedObject != null) {
                attractedObject.ResetGravitySources();
            }

            return spawnedObject;
		}
		else
		{
            IncreasePool(20); //generate more enemies
            return SpawnObject(); //attempt to spawn the enemy again
        }
	}
		
	public void DespawnObject(GameObject despawnedObject, bool removeFromActive = true)
	{
        despawnedObject.transform.parent = GameManager.gm.gameObject.transform;
        objectQueue.Enqueue(despawnedObject);
		if(removeFromActive && activeObjects.Contains(despawnedObject)) activeObjects.RemoveAt(activeObjects.IndexOf(despawnedObject));
		despawnedObject.SetActive(false);
		
		if (despawnedObject.GetComponent<Health>()) GameManager.gm.CheckRemainingEnemies();
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

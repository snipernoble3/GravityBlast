using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePickup : MonoBehaviour
{
	[SerializeField] private int xpToGive = 1;
	[HideInInspector] public float lerpValue = 0.0f; // Used when vacuuming up this object.
    [HideInInspector] public ObjectPool objectPool;

    private void OnTriggerEnter (Collider other)
	{
        Player_Stats stats = other.gameObject.GetComponent<Player_Stats>();
		if (stats != null)
		{
			stats.CollectXP(xpToGive);
			lerpValue = 1.0f;  // Force the lerp to be marked as completed.
            if (objectPool != null) objectPool.DespawnObject(gameObject);
            else gameObject.SetActive(false);
		}
    }
}

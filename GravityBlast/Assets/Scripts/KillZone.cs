using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {

    [HideInInspector] public God g;

    private void OnTriggerEnter (Collider other)
	{
        RaycastHit hit = g.GetCurrPlanet().RandomSpawnPoint();
        other.transform.position = hit.point + (hit.normal * 1f);
		
		Rigidbody otherRB = other.GetComponent<Rigidbody>();
		if (otherRB != null)
		{
			otherRB.velocity = Vector3.zero;
			otherRB.angularVelocity = Vector3.zero;
		}
		
		Gravity_AttractedObject attractedObject = other.GetComponent<Gravity_AttractedObject>();
		if (attractedObject != null)
		{
			attractedObject.ResetGravitySources();
		}
	    
		//Debug.Log("Teleported! " + other.gameObject.name);
		//else Debug.Log("Something entered the killzone but nothing happened.");
		
		//////////////////////////
		/*if (other.gameObject.GetComponent<Health>()) {
            other.gameObject.GetComponent<Health>().Kill();
            for (int i = 0; i < g.enemyPools.Length; i++) {
                if (other.gameObject == g.enemyPools[i].objectPrefab) {
                    g.enemyPools[i].SpawnObject(g.GetCurrPlanet().enemyContainer.transform);
                }
            }
        } else {
            other.gameObject.SetActive(false);
        }*/
    }
}

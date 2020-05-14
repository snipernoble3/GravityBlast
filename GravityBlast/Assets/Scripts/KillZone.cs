using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {

    [HideInInspector] public God g;

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.GetComponent<Health>()) {
            other.gameObject.GetComponent<Health>().Kill();
            for (int i = 0; i < g.enemyPools.Length; i++) {
                if (other.gameObject == g.enemyPools[i].objectPrefab) {
                    g.enemyPools[i].SpawnObject(g.GetCurrPlanet().enemyContainer.transform);
                }
            }
        } else {
            other.gameObject.SetActive(false);
        }
    }
}

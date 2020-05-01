using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {

    public bool destroy;

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.GetComponent<Health>()) {
            other.gameObject.GetComponent<Health>().Kill();
        } else {
            other.gameObject.SetActive(false);
        }
    }
}

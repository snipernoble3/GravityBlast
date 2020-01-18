using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModificationPickup : MonoBehaviour {

    //type

    private void OnTriggerEnter (Collider collision) {
        if (collision.gameObject.tag == "Player") {
            collision.gameObject.GetComponentInChildren<Shooting>().MaxUpgradeAll();
            this.gameObject.SetActive(false);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XP_Info : MonoBehaviour {

    [SerializeField] int xpToGive = 1;

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.GetComponent<Player_Stats>()) {
            other.gameObject.GetComponent<Player_Stats>().CollectXP(xpToGive);
            gameObject.SetActive(false);
        }
    }

    private void Update () {
        //float above the ground
    }

}

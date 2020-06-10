using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCollider : MonoBehaviour {

    [SerializeField] EnemyInfo controller;

    private void OnTriggerEnter (Collider other) {
        controller.PlayerEnteredRange();
    }

    private void OnTriggerExit (Collider other) {
        controller.PlayerExitRange();
    }

}

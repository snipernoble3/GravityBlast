﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {

    public bool destroy;

    private void OnTriggerEnter (Collider other) {
        if (destroy) {
            Destroy(other.gameObject);
        } else {
            other.gameObject.SetActive(false);
        }
    }
}

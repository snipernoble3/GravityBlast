using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModChest : MonoBehaviour {

    //mod spawn point
    [SerializeField] Transform spawnPoint;

    //mods to select from
    [SerializeField] GameObject[] mods;

    //selected mod
    GameObject mod;

    //is player in range to open
    bool playerNear;
    bool closed = true;

    private void Awake () {
        SelectMod();
        mod.SetActive(false);
    }

    private void Update () {

        if (closed && playerNear && Input.GetKeyDown(KeyCode.E)) {
            OpenChest();
        }

    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.tag == "Player") {
            playerNear = true;
        }
    }

    private void OnTriggerExit (Collider other) {
        if (other.gameObject.tag == "Player") {
            playerNear = false;
        }
    }

    void OpenChest () {
        //play anim here
        mod.SetActive(true);
        Debug.Log("Open");
        closed = false;
    }

    void SelectMod () {
        mod = Instantiate(mods[0], spawnPoint.position, spawnPoint.rotation);
    }

}

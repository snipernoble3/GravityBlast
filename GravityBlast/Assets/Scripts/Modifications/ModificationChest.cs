using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModificationChest : MonoBehaviour {

    //array of available modifications

    //3 selected mods (on awake)
    [SerializeField] GameObject[] modifications;
    

    //player raycast + E to select modification

    //turn chest effects "off"


    [SerializeField] GameObject player;
    [SerializeField] float playerActivationRange = 15f;
    bool opened;
    [SerializeField] GameObject display;
    [SerializeField] GameObject unopenedChest;
    [SerializeField] GameObject openedChest;

    private void Awake () {
        unopenedChest.SetActive(true);
        openedChest.SetActive(false);
        //select mods
        //add mods as children to the display empty
    }

    private void Update () {
        //when player is within range, auto open/display modifications
        if (!opened && Mathf.Abs(Vector3.Magnitude(player.transform.position - transform.position)) <= playerActivationRange) {
            display.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit)) {
                    if (hit.transform.gameObject.GetComponent<ModInfo>()) {
                        SelectModification(hit.transform.gameObject);
                    }
                }
            }
        } else {
            display.SetActive(false);
        }
        
    }

    public void SelectModification (GameObject g) {

        //compare g to mod list
        string n = g.GetComponent<ModInfo>().modName;
        int i = g.GetComponent<ModInfo>().levels;
        //pass the mod info to the player stats script
        player.GetComponent<Player_Stats>().UpdateModification(n, i);
        //player.GetComponent<WeaponManager>().UpgradeWeapon();
        opened = true;
        unopenedChest.SetActive(false);
        openedChest.SetActive(true);
    }

}

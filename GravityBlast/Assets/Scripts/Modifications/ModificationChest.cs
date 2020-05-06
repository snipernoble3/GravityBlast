using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModificationChest : MonoBehaviour {
    
    [SerializeField] GameObject[] modifications;

    [HideInInspector] public GameObject player;
    [SerializeField] float playerActivationRange = 15f;
    bool opened;
    [SerializeField] GameObject display;
    [SerializeField] GameObject unopenedChest;
    [SerializeField] GameObject openedChest;

    private void Awake () {
        unopenedChest.SetActive(true);
        openedChest.SetActive(false);
        //select mods
        GenerateMods();
    }

    private void Update () {
        
        //when player is within range, auto open/display modifications
        if (player != null && !opened && Mathf.Abs(Vector3.Magnitude(player.transform.position - transform.position)) <= playerActivationRange) {
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

    void GenerateMods () {
        int[] mods = new int[3];

        mods[0] = Random.Range(0, modifications.Length);

        do {
            mods[1] = Random.Range(0, modifications.Length);
        } while (mods[0] == mods[1]);

        do {
            mods[2] = Random.Range(0, modifications.Length);
        } while (mods[0] == mods[2] || mods[1] == mods[2]);

        foreach (int i in mods) {
            Instantiate(modifications[i], display.transform);
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

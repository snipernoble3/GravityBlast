using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static God;

public class ModificationChest : MonoBehaviour {
    
    [SerializeField] GameObject[] modifications;

    [HideInInspector] public GameObject player;
    [SerializeField] float playerActivationRange = 15f;
    private bool expended = false;
    [SerializeField] GameObject display;
	
	[SerializeField] private Animator chest_Animator;
	[SerializeField] private Material[] crateMats;
	private Renderer[] crateRends;
	
	private bool playerWithinRange = false;

    private void Awake ()
	{
		crateRends = GetComponentsInChildren<Renderer>();
		SetColor(0);
		
		closeOpenChest(false);
	   
	    //select mods
        GenerateMods();
    }

    private void Update ()
	{
		//when player is within range, auto open/display modifications
		if (!expended && playerWithinRange)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit)) {
                    if (hit.transform.gameObject.GetComponent<ModInfo>()) {
                        SelectModification(hit.transform.gameObject);
                    }
                }
            }
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
        expended = true;
        SetColor(1);
		closeOpenChest(false);
    }
	
	private void SetColor(int i)
	{
		foreach (Renderer rend in crateRends)
		{
			rend.material = crateMats[i];
		}
	}
	
	private void OnTriggerEnter(Collider triggeredObject)
    {
		if (!expended && triggeredObject.gameObject.tag == "Player")
		{
			closeOpenChest(true);
		}
    }
	
	private void OnTriggerExit(Collider triggeredObject)
    {
		if (!expended && triggeredObject.gameObject.tag == "Player")
		{
			closeOpenChest(false);
		}
    }
	
	private void closeOpenChest(bool closeOpenState)
	{
		playerWithinRange = closeOpenState;
		chest_Animator.SetBool("isOpen", closeOpenState);
        display.SetActive(closeOpenState);
	}
}

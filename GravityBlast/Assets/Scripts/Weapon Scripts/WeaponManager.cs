using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponManager : MonoBehaviour {

	[SerializeField] private GameObject[] weaponPrefabs; // An array of all weapon prefabs the player can have.
	private GameObject[] weapons; // An array of the instantiated weapons the player has.
	[SerializeField] private Transform[] weaponBones; // The weapon "bone" positions in the armature to attach the weapons to.
	[SerializeField] private Animator animator; // A reference to Gabriel's animation system.
	[SerializeField] private TextMeshProUGUI ammoUI;
    
    //[SerializeField] private int startingGun;
    private int currentWeapon = 0;

    private void Awake()
	{
		
		weapons = new GameObject[weaponPrefabs.Length];
		
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i] = Instantiate(weaponPrefabs[i]); // Instantiate the weapon.
			weapons[i].transform.parent = weaponBones[1]; // Make the weapon a child of the right hand weapon bone.
			weapons[i].transform.localPosition = Vector3.zero; // Clear out the transform so it aligns with the weapon bone.
			//weapons[i].transform.localRotation = Quaternion.identity;
			weapons[i].transform.localRotation = Quaternion.Euler(Vector3.right * 90.0f);
			weapons[i].transform.localScale = Vector3.one;
			
			// Set up references.
			weapons[i].GetComponent<Deliverance>().player = gameObject;
			weapons[i].GetComponent<Deliverance>().arms = animator;
			weapons[i].GetComponent<Deliverance>().ammoUI = ammoUI;
			
			weapons[i].SetActive(false); // Hide the weapon.
        }
		
		//currentWeapon = startingGun;
		//SwitchWeapon(startingGun);
		SwitchWeapon(currentWeapon);
    }

    //reload passthrough
    public void Reload()
	{
        weapons[currentWeapon].GetComponent<IProjectileWeapon>().Reload();
    }
	
	//test multiple weapons
    void Update()
	{
        if (Input.GetKeyDown(KeyCode.Alpha0)) SwitchWeapon(0);
		if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
		if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
		if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);
    }

    //switch weapon
	void SwitchWeapon(int weaponOfChoice)
	{
		if (weaponOfChoice >= 0 && weaponOfChoice < weapons.Length)
		{		
			foreach (GameObject weapon in weapons)
			{
				weapon.SetActive(false);
			}
			weapons[weaponOfChoice].SetActive(true);
		}
	}

    public void UpgradeWeapon () {
        weapons[currentWeapon].GetComponent<Deliverance>().ModifyAll(true);
    }

}

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

    Player_Stats ps;

    //[SerializeField] private int startingGun;
    private int currentWeapon = 0;
	
	GameObject realMag; // A reference to the magazine that stays loaded into the gun.
	GameObject fakeMag; // A reference to the fake instance magazine that moves with the reloading hand.

    [HideInInspector] public bool paused;

    private void Awake() {

        ps = gameObject.GetComponent<Player_Stats>();

        weapons = new GameObject[weaponPrefabs.Length];
		
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i] = Instantiate(weaponPrefabs[i]); // Instantiate the weapon.
			weapons[i].transform.parent = weaponBones[1]; // Make the weapon a child of the right hand weapon bone.
			weapons[i].transform.localPosition = Vector3.zero; // Clear out the transform so it aligns with the weapon bone.
			weapons[i].transform.localRotation = Quaternion.identity;
			//weapons[i].transform.localRotation = Quaternion.Euler(Vector3.right * 90.0f);
			weapons[i].transform.localScale = Vector3.one;
			
			// Set up references. - eventually change to send message to weapon object
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
		
		realMag.SetActive(true);
		Destroy(fakeMag);
    }
	
	public void RemoveMagazine()
	{
		realMag = weapons[currentWeapon].GetComponent<Deliverance>().magazine;
		fakeMag = Instantiate(realMag, realMag.transform.position, realMag.transform.rotation);
		fakeMag.transform.SetParent(weaponBones[0]);
		
		realMag.SetActive(false);
    }
	
	//test multiple weapons
    void Update() {
        
        animator.SetFloat("fireSpeed", 1.0f + ps.mFireRate);
        animator.SetFloat("reloadSpeed", 1.0f + ps.mReload);

        if (Input.GetKeyDown(KeyCode.Alpha0) && !paused) SwitchWeapon(0);
		if (Input.GetKeyDown(KeyCode.Alpha1) && !paused) SwitchWeapon(0);
		if (Input.GetKeyDown(KeyCode.Alpha2) && !paused) SwitchWeapon(1);
		if (Input.GetKeyDown(KeyCode.Alpha3) && !paused) SwitchWeapon(2);

        if (Input.GetButton("Fire1") && !paused) {
            weapons[currentWeapon].GetComponent<IProjectileWeapon>().FireInput();
            //animator.SetBool("fire", true);
        } else {
            animator.SetBool("fire", false);
        }
        if (Input.GetButton("Reload") && !paused) { weapons[currentWeapon].GetComponent<IProjectileWeapon>().ReloadInput(); }

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
        //weapons[currentWeapon].GetComponent<Deliverance>().ModifyAll(true);
    }

}

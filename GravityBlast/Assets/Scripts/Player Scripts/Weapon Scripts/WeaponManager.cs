using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponManager : MonoBehaviour {

	[SerializeField] private GameObject[] weaponPrefabs; // An array of all weapon prefabs the player can have.
	[HideInInspector] public ProjectileWeapon[] weapons; // An array of the instantiated weapons the player has.
	[SerializeField] private Transform[] weaponBones; // The weapon "bone" positions in the armature to attach the weapons to.
	[SerializeField] public Animator animator; // A reference to Gabriel's animation system.
	[SerializeField] private TextMeshProUGUI ammoUI;

    Player_Stats playerStats;

    //[SerializeField] private int startingGun;
    public int currentWeapon = 0;
	
	GameObject realMag; // A reference to the magazine that stays loaded into the gun.
	GameObject fakeMag; // A reference to the fake instance magazine that moves with the reloading hand.

    private void Awake() {

        playerStats = gameObject.GetComponent<Player_Stats>();
        playerStats.SetWeaponManager(this);

        weapons = new ProjectileWeapon[weaponPrefabs.Length];
		
		for (int i = 0; i < weapons.Length; i++)
		{
            GameObject newWeapon = Instantiate(weaponPrefabs[i]); // Instantiate the weapon.
            newWeapon.transform.parent = weaponBones[1]; // Make the weapon a child of the right hand weapon bone.
            newWeapon.transform.localPosition = Vector3.zero; // Clear out the transform so it aligns with the weapon bone.
			newWeapon.transform.localRotation = Quaternion.identity;
			//weapons[i].transform.localRotation = Quaternion.Euler(Vector3.right * 90.0f);
			newWeapon.transform.localScale = Vector3.one;

            weapons[i] = newWeapon.GetComponent<ProjectileWeapon>();
			weapons[i].player = gameObject;
            weapons[i].playerStats = playerStats;
			weapons[i].arms = animator;
			weapons[i].ammoUI = ammoUI;
            weapons[i].UpdateAnimations();
			
			weapons[i].gameObject.SetActive(false); // Hide the weapon.
        }
		
		//currentWeapon = startingGun;
		//SwitchWeapon(startingGun);
		SwitchWeapon(currentWeapon);
    }

    //reload passthrough
    public void Reload()
	{
        weapons[currentWeapon].EndReload();
		realMag.SetActive(true);
		Destroy(fakeMag);
    }
	
	public void RemoveMagazine()
	{
		realMag = weapons[currentWeapon].magazine;
		fakeMag = Instantiate(realMag, realMag.transform.position, realMag.transform.rotation);
		fakeMag.transform.SetParent(weaponBones[0]);
		
		realMag.SetActive(false);
    }
	
	//test multiple weapons
    void Update() {
        
        //animator.SetFloat("fireSpeed", 1.0f + playerStats.mFireRate);
        //animator.SetFloat("reloadSpeed", 1.0f + playerStats.mReload);

		/*
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
		*/
    }

    //switch weapon
	public void SwitchWeapon(int weaponOfChoice)
	{
		if (weaponOfChoice >= 0 && weaponOfChoice < weapons.Length)
		{		
			foreach (ProjectileWeapon weapon in weapons)
			{
				weapon.gameObject.SetActive(false);
			}
			weapons[weaponOfChoice].gameObject.SetActive(true);
		}

        UpdateAnimations();
	}

    public void UpdateAnimations () {
        weapons[currentWeapon].UpdateAnimations();
    }

}

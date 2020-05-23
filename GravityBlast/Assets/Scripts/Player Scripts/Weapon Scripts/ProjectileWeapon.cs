using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class ProjectileWeapon : MonoBehaviour {

    // External Components //
    public GameObject player;
    public Player_Stats playerStats;
    public GameObject firingPosition;
    //bullet class?
    public GameObject bullet;
    public Animator arms;
    public string armsFireAnimation;
    public string armsFireSpeed;
    public string armsReloadAnimation;
    public string armsReloadSpeed;
    public Weapon_Barrel barrel;
    public GameObject magazine;
    public TextMeshProUGUI ammoUI;


    // Abstract methods should only be the things ALL weapons will need to be hooked up properly

    public abstract void FireInput (); //When the input manager recieves a fire input, this is called to check if the weapon can be fired

    public abstract void ReloadInput (); //When the input manager recieves a fire input, this is called to check if the weapon can be fired

    public abstract void EndReload (); //This is called during the reload animation to signal when to add bullets back into the gun

    public abstract void UpdateUI (); //This updates the ammo counter

    public abstract void UpdateAnimations (); //This can be broken into more specific methods if needed, but for now all the animation settings are set here

}

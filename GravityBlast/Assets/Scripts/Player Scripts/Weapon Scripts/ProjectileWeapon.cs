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

    public abstract void FireInput ();

    public abstract void ReloadInput ();

    public abstract void EndReload ();

    public abstract void UpdateUI ();

    public abstract void UpdateAnimations ();

}

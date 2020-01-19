using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

    [SerializeField] GameObject[] GunList;
    
    [SerializeField] int startingGun;
    int currGun;

    private void Start () {
        currGun = startingGun;
    }

    //reload passthrough
    void AnimReload () {
        GunList[currGun].GetComponent<IProjectileWeapon>().Reload();
    }

    //swap gun

}

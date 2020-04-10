using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IProjectileWeapon {

    void FireInput ();

    void Fire ();

    void Recoil ();

    void UpdateSpread ();

    void UseAmmo (int amount);

    void ReloadInput ();

    void Reload ();

    void UpdateUI ();

    //void UpdateModifications ();
}

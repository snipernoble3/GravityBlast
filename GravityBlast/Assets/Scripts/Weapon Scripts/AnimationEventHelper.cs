using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHelper : MonoBehaviour
{
	[SerializeField] private WeaponManager weaponManager;
	
    void RemoveMagazine()
    {
		weaponManager.RemoveMagazine();
    }
    
	void ReplaceMagazine()
    {
        weaponManager.Reload();
    }
}

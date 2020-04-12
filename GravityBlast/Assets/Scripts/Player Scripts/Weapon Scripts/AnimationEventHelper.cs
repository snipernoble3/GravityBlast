using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHelper : MonoBehaviour
{
	[SerializeField] private WeaponManager weaponManager;
	[SerializeField] private Player_BlastMechanics blastMechanics;
	
    void RemoveMagazine()
    {
		weaponManager.RemoveMagazine();
    }
    
	void ReplaceMagazine()
    {
        weaponManager.Reload();
    }
	
	void DisableAttacks()
    {
        blastMechanics.EnableBlast(false);
    }
	
	void EnableAttacks()
    {
		blastMechanics.EnableBlast(true);
    }
}

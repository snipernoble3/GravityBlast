﻿using System.Collections;
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
		Player_Input.SetBlastState(true);
    }
	
	void DisableAttacks()
    {
        Player_Input.SetBlastState(false);
		Player_Input.SetShootState(false);
    }
	
	void EnableAttacks()
    {
		Player_Input.SetBlastState(true);
		Player_Input.SetShootState(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHelper : MonoBehaviour
{
	[SerializeField] private WeaponManager weaponManager;
	
    // Update is called once per frame
    void ReloadEvent()
    {
        weaponManager.Reload();
    }
}

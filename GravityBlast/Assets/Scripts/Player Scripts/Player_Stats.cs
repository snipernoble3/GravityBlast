﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player_Stats : MonoBehaviour {

    // Health //
    [SerializeField] int baseHP = 3;
    private int maxHP;
    private int currHP;
    private bool noHealth;
    [SerializeField] private bool godMode;
	[SerializeField] GameObject postProcessing;
	private Volume hurtVFX;

    public TextMeshProUGUI healthText;
    private string baseText;
    private int maxPips = 10;

    // XP //
    private int xpToAdvance;
    private int currXP;
    //private int overflowXP;
    [HideInInspector] public bool xpFull;
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI xpText;
    private string jumpReady = "Jump Ready - Press B to Jump to Next Planet";

    // Modifications //
    //increased health
    [HideInInspector] public int mHealth;
    int mHealthLevel = 0;
    int mHealthCap = 15;
    int mHealthPerLevel = 1;
    //extended clip - 5 levels, +40% each (+200% cap)
    [HideInInspector] public float mAmmo;
    int mAmmoLevel = 0;
    int mAmmoCap = 5;
    float mAmmoPerLevel = 0.4f;
    //faster reload - 3 levels, +25% each (+75% cap)
    [HideInInspector] public float mReload;
    int mReloadLevel = 0;
    int mReloadCap = 3;
    float mReloadPerLevel = 0.25f;
    //increased fire rate - 5 levels, +10% each (+50% cap) -- this translates to firing twice as fast
    [HideInInspector] public float mFireRate;
    int mFireRateLevel = 0;
    int mFireRateCap = 5;
    float mFireRatePerLevel = 0.1f;
    //increased range (time to bullet expiration) - 5 levels, +10% each (+50% cap)
    [HideInInspector] public float mProjectileDuration;
    int mProjectileDurationLevel = 0;
    int mProjectileDurationCap = 5;
    float mProjectileDurationPerLevel = 0.1f;
    //increased bullet size - 5 levels, +20% each (+50% cap)
    [HideInInspector] public float mProjectileScale;
    int mProjectileScaleLevel = 0;
    int mProjectileScaleCap = 5;
    float mProjectileScalePerLevel = 0.1f;
    //increased bullet force - 5 levels, +10% each (+50% cap)
    [HideInInspector] public float mProjectileForce;
    int mProjectileForceLevel = 0;
    int mProjectileForceCap = 5;
    float mProjectileForcePerLevel = 0.1f;

    private WeaponManager weaponManager;

    private void Start () {
        
		hurtVFX = postProcessing.GetComponents<Volume>()[1];
		
		maxHP = baseHP;
        currHP = maxHP;

        UpdateModification("all"); //just base set up all modifications
    }


    private void Update () {

        //Test Inputs
        //if (Input.GetKeyDown(KeyCode.LeftAlt)) { UpdateModification("all", 100); }
        //if (Input.GetKeyDown(KeyCode.Equals)) { UpdateModification("all", 1); }
        //if (Input.GetKeyDown(KeyCode.Minus)) { UpdateModification("all", -1); }



    }

    public void UpdateHealth (int i = 0) {
        
        if (godMode) { return; }

        if (i != 0) { i = (i > 0) ? 1 : -1; }
            

        switch (i) {
            case 1:
                //increase health
                currHP = Mathf.Clamp(currHP + 1, 0, maxHP);
                break;
            case -1:
                //decrease health
                currHP--;
                if (currHP <= 0 && !noHealth) {
                    //player death
                    NoHealth();
                    currHP = 0;
                }
				else PainVFX();
                break;
            case 0:
                //update max health
                int dif = (baseHP + mHealth) - maxHP;
                maxHP = baseHP + mHealth;
                if (dif != 0) UpdateHealth(dif); //gain or lose hp to match the adjustment in max health
                break;
        }

        UpdateHealthUI();
    }

    private void NoHealth () {
        noHealth = true;
        if (currHP <= 0) {
			PainVFX();
            GameManager.gm.PlayerDeath();
        } else {
            noHealth = false;
        }
        
    }
	
	private void PainVFX()
	{
		if (hurtVFX != null) StartCoroutine(PainVFXPingPong());
	}
	
	IEnumerator PainVFXPingPong()
    {
		float pingPong = 2.0f;
		if (noHealth) pingPong = 1.0f;
		
		float speed = 2.0f;
		float progress = 0.0f;
        while(progress < pingPong)
        {
			hurtVFX.weight = Mathf.PingPong(progress, 1.0f);
			progress += Time.deltaTime * speed;
            yield return null;
        }
		if (!noHealth) hurtVFX.weight = 0.0f;
		else hurtVFX.weight = 1.0f;
    }

    public void UpdateModification (string mName, int i = 0) {
        //Debug.Log("Upgrading " + mName + " by " + i);
        switch (mName) {
            case "all":
                if (i == 0) {
                    mHealth = mHealthLevel * mHealthPerLevel;
                    UpdateHealth(0);
                    mAmmo = (mAmmoLevel * mAmmoPerLevel);
                    mReload = mReloadLevel * mReloadPerLevel;
                    mFireRate = mFireRateLevel * mFireRatePerLevel;
                    UpdateWeaponAnimations();
                    mProjectileDuration = mProjectileDurationLevel * mProjectileDurationPerLevel;
                    mProjectileScale = mProjectileScaleLevel * mProjectileScalePerLevel;
                    mProjectileForce = mProjectileForceLevel * mProjectileForcePerLevel;
                } else {
                    mHealthLevel = Mathf.Clamp(mHealthLevel + i, 0, mHealthCap);
                    mHealth = mHealthLevel * mHealthPerLevel;
                    UpdateHealth(0);
                    mAmmoLevel = Mathf.Clamp(mAmmoLevel + i, 0, mAmmoCap);
                    mAmmo = (mAmmoLevel * mAmmoPerLevel);
                    mReloadLevel = Mathf.Clamp(mReloadLevel + i, 0, mReloadCap);
                    mReload = mReloadLevel * mReloadPerLevel;
                    mFireRateLevel = Mathf.Clamp(mFireRateLevel + i, 0, mFireRateCap);
                    mFireRate = mFireRateLevel * mFireRatePerLevel;
                    UpdateWeaponAnimations();
                    mProjectileDurationLevel = Mathf.Clamp(mProjectileDurationLevel + i, 0, mProjectileDurationCap);
                    mProjectileDuration = mProjectileDurationLevel * mProjectileDurationPerLevel;
                    mProjectileScaleLevel = Mathf.Clamp(mProjectileScaleLevel + i, 0, mProjectileScaleCap);
                    mProjectileScale = mProjectileScaleLevel * mProjectileScalePerLevel;
                    mProjectileForceLevel = Mathf.Clamp(mProjectileForceLevel + i, 0, mProjectileForceCap);
                    mProjectileForce = mProjectileForceLevel * mProjectileForcePerLevel;
                }
                
                break;
            case "mHealth":
                mHealthLevel = Mathf.Clamp(mHealthLevel + i, 0, mHealthCap);
                mHealth = mHealthLevel * mHealthPerLevel;
                UpdateHealth(0);
                break;
            case "mAmmo":
                mAmmoLevel = Mathf.Clamp(mAmmoLevel + i, 0, mAmmoCap);
                mAmmo = (mAmmoLevel * mAmmoPerLevel);
                break;
            case "mReload":
                mReloadLevel = Mathf.Clamp(mReloadLevel + i, 0, mReloadCap);
                mReload = mReloadLevel * mReloadPerLevel;
                UpdateWeaponAnimations();
                break;
            case "mFireRate":
                mFireRateLevel = Mathf.Clamp(mFireRateLevel + i, 0, mFireRateCap);
                mFireRate = mFireRateLevel * mFireRatePerLevel;
                UpdateWeaponAnimations();
                break;
            case "mProjectileDuration":
                mProjectileDurationLevel = Mathf.Clamp(mProjectileDurationLevel + i, 0, mProjectileDurationCap);
                mProjectileDuration = mProjectileDurationLevel * mProjectileDurationPerLevel;
                break;
            case "mProjectileScale":
                mProjectileScaleLevel = Mathf.Clamp(mProjectileScaleLevel + i, 0, mProjectileScaleCap);
                mProjectileScale = mProjectileScaleLevel * mProjectileScalePerLevel;
                break;
            case "mProjectileForce":
                mProjectileForceLevel = Mathf.Clamp(mProjectileForceLevel + i, 0, mProjectileForceCap);
                mProjectileForce = mProjectileForceLevel * mProjectileForcePerLevel;
                break;
        }
        
    }

    public void SetWeaponManager (WeaponManager wm) {
        weaponManager = wm;
    }

    void UpdateWeaponAnimations () {
        weaponManager.UpdateAnimations();
    }

    public void UpdateHealthUI () {
        try {
            healthText.text = "0"; // 0 is the "shield" symbol in the dingbats
            for (int i = 1; i <= currHP && i <= maxPips; i++) {
                healthText.text += "1"; // 1 is the "pip" symbol in the dingbats
                if (i == maxPips) healthText.text += "+"; // Indicate that there is more health than is currently displayed in the number of pips.
            }
        } catch (NullReferenceException e) {
            Debug.Log("Player_Stats.UpdateHealthUI(): " + e);
        }
    }

    public void CollectXP (int x) {
        if (!xpFull) {
            currXP += x;
            if (currXP >= xpToAdvance) {
                xpFull = true;
                //overflow += currXP
            }
            UpdateXPUI();
        } else {
            //overflow += x;
        }
    }

    public void UpdateXPUI () {
        xpBar.value = currXP;
        if (xpFull) {
            xpText.text = jumpReady;
        } else {
            xpText.text = currXP + "/" + xpToAdvance;
        }
    }

    public void SetXP (int newTargetXP) {
        xpFull = false;
        currXP = 0;
        xpToAdvance = newTargetXP;
        xpBar.minValue = 0;
        xpBar.maxValue = xpToAdvance;
        UpdateXPUI();
    }
    
    public void toggleGodMode () {
        godMode = !godMode;
    }

    public void FillXP () {
        CollectXP(xpToAdvance);
    }

}

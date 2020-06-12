using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deliverance2 : ProjectileWeapon {

    // Base Info //
    [SerializeField] float fireRate = 0.05f;
    [SerializeField] int maxAmmo = 50;
    [SerializeField] float reloadSpeed = 1f;
    [SerializeField] float bulletScale = 1f;
    [SerializeField] float bulletForce = 60f;
    [SerializeField] int currAmmo;
    [SerializeField] float maxDeviation = 0.001f;
    [SerializeField] float deviationDistance = 25f;

    // Counters //
    float timeToFire = 0f;
    float shotCount = 0; //how many bullets are being counted as part of the current spray / affecting bullet spread
    float spreadReductionTimer = 0f;
    bool reloading;
	
	// Ammo Meter
	[SerializeField] private Renderer ammoMeter;
	private Material ammoMeterMat;
	[SerializeField] private Gradient ammoMeterGradient;

    void Start () {
		ammoMeterMat = ammoMeter.material;
		currAmmo = maxAmmo;
    }

    public void Update () {

        if (timeToFire > 0) {
            timeToFire -= Time.deltaTime;
        }

        //UpdateAnimations(); // Will move this to be called only when Player_Stats has a change that warrants this being updated

    }

    public void LateUpdate () {

        if (spreadReductionTimer > 0 && shotCount > 0) {
            spreadReductionTimer -= Time.deltaTime;
        }

        if (reloading || ((currAmmo == 0 || !Input.GetMouseButton(0)) && spreadReductionTimer <= 0)) { // Check to reduce the spread if reloading or not firing
            shotCount = Mathf.Clamp(shotCount - 0.75f, 0, maxAmmo);
            int r = reloading ? 1 : 0; 
            spreadReductionTimer = (0.01f * shotCount / 2) - r; // This will speed up the spread reset if reloading or out of ammo
            UpdateSpread();
        }

    }

    public override void FireInput () {

        if (timeToFire <= 0 && currAmmo != 0 && !reloading) {
            Fire();
            timeToFire = fireRate * (1 - (playerStats.mFireRate));
        }

        if (currAmmo != 0 && !reloading) {
            arms.SetBool(armsFireAnimation, true);
            if (barrel != null) barrel.Spin();
        } else {
            arms.SetBool(armsFireAnimation, false);
        }
    }

    public void Fire () {

        // These lines calculate the random shot direction as affected by the spread
        Vector3 deviation3D = Random.insideUnitCircle * maxDeviation;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward * deviationDistance + deviation3D);
        Vector3 forwardVector = Camera.main.transform.rotation * rot * Vector3.forward;

        // Create and fire the bullet - Will eventually switch to object pooling
        GameObject b = Instantiate(bullet, firingPosition.transform.position, firingPosition.transform.rotation, null);
        b.GetComponent<Gravity_AttractedObject>().CurrentGravitySource = player.GetComponent<Gravity_AttractedObject>().CurrentGravitySource;
        b.transform.rotation = Quaternion.FromToRotation(firingPosition.transform.rotation.eulerAngles, forwardVector);
        b.transform.Rotate(new Vector3(0, 5, 0), Space.Self); // Add +5 degrees vertical here - This helps offset the feel of immediate drop off from projectile vs raycast
        b.transform.localScale = new Vector3(b.transform.localScale.x * (1f + playerStats.mProjectileScale), b.transform.localScale.x * (1f + playerStats.mProjectileScale), b.transform.localScale.x * (1f + playerStats.mProjectileScale));
        b.GetComponent<Rigidbody>().AddForce(forwardVector * (60 * (1 + playerStats.mProjectileForce)), ForceMode.VelocityChange);
        Destroy(b, 5f * (1f + playerStats.mProjectileDuration));

        shotCount++;
        if (shotCount > 18) shotCount = 18; //stops shotCount from continually stacking beyond the maximum to affect the spread (17)

        UpdateSpread();
        UseAmmo(1);

        return;
    }

    public override void ReloadInput () {
        if (!reloading && currAmmo != (int)(maxAmmo * (1 + (playerStats.mAmmo)))) {
            Player_Input.SetBlastState(false); // Move this functionality into the weapon manager later!!!
            arms.Play(armsReloadAnimation, 0, 0.0f); // Play the reload animation.
            reloading = true;
        }
    }

    public override void EndReload () {
        currAmmo = (int)(maxAmmo * (1 + (playerStats.mAmmo)));
        UpdateUI();
        reloading = false;
        return;
    }

    public void UpdateSpread () {

        //The first 3 shots will be very precise, while the 4th to 10th shot will be increasingly inaccurate. Shots beyond the 10th will slowly extend to the spray cap (hit at 17 shots)
        if (shotCount == 0) {
            maxDeviation = 0.001f;
        } else if (shotCount <= 3) {
            maxDeviation = (-.00738f * shotCount * shotCount * shotCount) + (.0999f * shotCount * shotCount) - (.1799f * shotCount) + .0873f;
        } else if (shotCount <= 10) {
            maxDeviation = (.0016f * shotCount * shotCount * shotCount) - (.0299f * shotCount * shotCount) + (.2467f * shotCount) - .3092f;
        } else {
            maxDeviation = Mathf.Clamp(1f + (.15f * (shotCount - 10)), 1f, 2f); //changing the last number will change how large the spread can end
        }

        return;
    }

    public void UseAmmo (int amount) {
        currAmmo -= amount;
        UpdateUI();
        return;
    }

    public override void UpdateUI () {
        if (ammoUI != null) ammoUI.text = "<font=\"GravityBlast_Dingbats SDF\" material=\"GravityBlast_Dingbats SDF_Holographic\">2</font> " + currAmmo;
		
		// Update the ammo meter on the gun model.
		if (ammoMeterMat != null)
		{
			float ammoPercent = 1.0f;
			if (playerStats != null) ammoPercent = Mathf.InverseLerp(0, (int)(maxAmmo * (1 + (playerStats.mAmmo))), currAmmo);
			else ammoPercent = Mathf.InverseLerp(0, maxAmmo, currAmmo); // THIS IF STATEMENT WON'T BE NEEDED AFTER FIXING THE REFERENCE ISSUE.
			
			Color col = new Color();
			
			col = ammoMeterGradient.Evaluate(ammoPercent);
			col *= 2.5f; // Add some intensity for the HDR since the gradient isn't very bright.
			col *= 1.0f + ((1.0f - ammoPercent) * 2.0f); // Glow more as the ammo count reduces.
			
			ammoMeterMat.SetColor("_Color", col);
			ammoMeterMat.SetFloat("_MeterHeight", ammoPercent);
		}	
		
        return;
    }

    public override void UpdateAnimations () {
        barrel.fireRateMultiplier = 1.0f + playerStats.mFireRate;
        arms.SetFloat(armsFireSpeed, 1.0f + playerStats.mFireRate);
        arms.SetFloat(armsReloadSpeed, 1.0f + playerStats.mReload);
        return;
    }

}

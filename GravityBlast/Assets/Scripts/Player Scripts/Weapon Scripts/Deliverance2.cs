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
    float shotCount = 0;
    float spreadReductionTimer = 0f;
    bool reloading;

    void Start () {

        currAmmo = maxAmmo;

        playerStats = player.GetComponent<Player_Stats>();

        UpdateAnimations();
    }

    public void Update () {

        if (timeToFire > 0) {
            timeToFire -= Time.deltaTime;
        }

        UpdateAnimations();

    }

    public void LateUpdate () {

        if (spreadReductionTimer > 0 && shotCount > 0) {
            spreadReductionTimer -= Time.deltaTime;
        }

        if (reloading || ((currAmmo == 0 || !Input.GetMouseButton(0)) && spreadReductionTimer <= 0)) {
            shotCount = Mathf.Clamp(shotCount - 0.75f, 0, maxAmmo);
            int r = reloading ? 1 : 0;
            spreadReductionTimer = (0.01f * shotCount / 2) - r;
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

        Vector3 deviation3D = Random.insideUnitCircle * maxDeviation;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward * deviationDistance + deviation3D);
        Vector3 forwardVector = Camera.main.transform.rotation * rot * Vector3.forward;

        GameObject b = Instantiate(bullet, firingPosition.transform.position, firingPosition.transform.rotation);
        b.GetComponent<Gravity_AttractedObject>().CurrentGravitySource = player.GetComponent<Gravity_AttractedObject>().CurrentGravitySource;
        b.transform.rotation = Quaternion.FromToRotation(firingPosition.transform.rotation.eulerAngles, forwardVector);
        b.transform.Rotate(new Vector3(0, 5, 0), Space.Self); //add +5 degrees vertical here?
        b.transform.localScale = new Vector3(b.transform.localScale.x * (1f + playerStats.mProjectileScale), b.transform.localScale.x * (1f + playerStats.mProjectileScale), b.transform.localScale.x * (1f + playerStats.mProjectileScale));
        b.GetComponent<Rigidbody>().AddForce(forwardVector * (60 * (1 + playerStats.mProjectileForce)), ForceMode.VelocityChange);
        Destroy(b, 5f * (1f + playerStats.mProjectileDuration));

        shotCount++;
        if (shotCount > 18) shotCount = 18;

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

        if (shotCount == 0) {
            maxDeviation = 0.001f;
        } else if (shotCount < 3) {
            maxDeviation = (-.00738f * shotCount * shotCount * shotCount) + (.0999f * shotCount * shotCount) - (.1799f * shotCount) + .0873f;
        } else if (shotCount < 10) {
            maxDeviation = (.0016f * shotCount * shotCount * shotCount) - (.0299f * shotCount * shotCount) + (.2467f * shotCount) - .3092f;
        } else {
            maxDeviation = Mathf.Clamp(1f + (.15f * (shotCount - 10)), 1f, 2f);
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
        return;
    }

    public override void UpdateAnimations () {
        barrel.fireRateMultiplier = 1.0f + playerStats.mFireRate;
        arms.SetFloat(armsFireSpeed, 1.0f + playerStats.mFireRate);
        arms.SetFloat(armsReloadSpeed, 1.0f + playerStats.mReload);
        return;
    }

}

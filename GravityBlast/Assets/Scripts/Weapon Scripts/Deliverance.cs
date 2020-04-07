using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deliverance : MonoBehaviour, IProjectileWeapon {

    // External Components //
    public GameObject player;
    public GameObject firingPosition;
    //bullet class?
    public GameObject bullet;
    //player stats/modifications
    public Animator arms;
    public Weapon_Barrel barrel;
	public GameObject magazine;
    public TextMeshProUGUI ammoUI;


    // Base Info //
    [SerializeField] private float fireRate = 0.05f;
    [SerializeField] private int maxAmmo = 50;
    private float reloadSpeed = 1f;
    private float bulletScale = 1f;
    private float bulletForce = 60f;
    private int currAmmo;
    private float maxDeviation = 0.001f;
    public float deviationDistance = 25f;
    

    // Counters //
    float timeToFire = 0f;
    float shotCount = 0;
    float spreadReductionTimer = 0f;
    private bool reloading;
    private bool godMode;


    Player_Stats ps;
    /*
    // Modifications //
    //extended clip - 5 levels, +40% each (+200% cap)
    int mAmmo;
    int mAmmoLevel = 0;
    int mAmmoCap = 5;
    float mAmmoPerLevel = 0.4f;
    //faster reload - 3 levels, +25% each (+75% cap)
    float mReload;
    int mReloadLevel = 0;
    int mReloadCap = 3;
    float mReloadPerLevel = 0.25f;
    //increased fire rate - 5 levels, +20% each (+100% cap)
    float mFireRate;
    int mFireRateLevel = 0;
    int mFireRateCap = 5;
    float mFireRatePerLevel = 0.2f;
    //increased range (time to bullet expiration) - 5 levels, +10% each (+50% cap)
    float mProjectileDuration;
    int mProjectileDurationLevel = 0;
    int mProjectileDurationCap = 5;
    float mProjectileDurationPerLevel = 0.1f;
    //increased bullet size - 5 levels, +20% each (+50% cap)
    float mProjectileScale;
    int mProjectileScaleLevel = 0;
    int mProjectileScaleCap = 5;
    float mProjectileScalePerLevel = 0.1f;
    //increased bullet force - 5 levels, +10% each (+50% cap)
    float mProjectileForce;
    int mProjectileForceLevel = 0;
    int mProjectileForceCap = 5;
    float mProjectileForcePerLevel = 0.1f;
    */

    // Start is called before the first frame update
    void Start () {

        currAmmo = maxAmmo;

        UpdateUI();

        ps = player.GetComponent<Player_Stats>();

    }

    // Update is called once per frame
    void Update () {
        ps = player.GetComponent<Player_Stats>();

        barrel.fireRateMultiplier = 1.0f + ps.mFireRate;
        arms.SetFloat("fireSpeed", 1.0f + ps.mFireRate);
        arms.SetFloat("reloadSpeed", 1.0f + ps.mReload);

        if (timeToFire > 0) {
            timeToFire -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && timeToFire <= 0) {
            if (currAmmo != 0 && !reloading) {
                Fire();
                timeToFire = fireRate * (1 - (ps.mFireRate));
            }
        }

        if (Input.GetButton("Fire1") && currAmmo != 0) {
            arms.SetBool("fire", true);
            if (barrel != null && !reloading) barrel.Spin();
        } else {
            arms.SetBool("fire", false);
        }

        if (Input.GetButton("Reload") && !reloading && currAmmo != (int)(maxAmmo * (1 + (ps.mAmmo)))) {
            arms.Play("Rifle_Reload", 0, 0.0f); // Play the reload animation.
            reloading = true;
        }

        //if (Input.GetKeyDown(KeyCode.LeftAlt)) { MaxUpgrades(); } //god mode (max upgrades / min upgrades toggle)
        //if (Input.GetKeyDown(KeyCode.Equals)) { ModifyAll(true); } //increase all mod levels by 1
        //if (Input.GetKeyDown(KeyCode.Minus)) { ModifyAll(false); } //decrease all mod levels by 1

    }

    void LateUpdate () {
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

    public void Fire () {

        Vector3 deviation3D = Random.insideUnitCircle * maxDeviation;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward * deviationDistance + deviation3D);
        //Quaternion cam = Camera.main.transform.rotation + Quaternion.Euler(new Vector3(0f, 5f, 0f)); //add +5 degrees vertical here?
        Vector3 forwardVector = Camera.main.transform.rotation * rot * Vector3.forward;

        GameObject b = Instantiate(bullet, firingPosition.transform.position, firingPosition.transform.rotation);
        b.GetComponent<Gravity_AttractedObject>().SetGravitySource(player.GetComponent<Player_Movement>().gravitySource);
        b.transform.rotation = Quaternion.FromToRotation(firingPosition.transform.rotation.eulerAngles, forwardVector);
        b.transform.localScale = new Vector3(b.transform.localScale.x * (1f + ps.mProjectileScale), b.transform.localScale.x * (1f + ps.mProjectileScale), b.transform.localScale.x * (1f + ps.mProjectileScale));
        b.GetComponent<Rigidbody>().AddForce(forwardVector * (60 * (1 + ps.mProjectileForce)), ForceMode.VelocityChange);
        Destroy(b, 5f * (1f + ps.mProjectileDuration));
        
        shotCount++;
        if (shotCount > 18) shotCount = 18;

        UpdateSpread();
        UseAmmo(1);

        return;
    }

    public void Recoil () {
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

    public void Reload () {
        currAmmo = (int)(maxAmmo * (1 + (ps.mAmmo)));
        UpdateUI();
        reloading = false;
        return;
    }

    public void UpdateUI () {
        //ps.UpdateHealthUI();
        if (ammoUI != null) ammoUI.text = "<font=\"GravityBlast_Dingbats SDF\" material=\"GravityBlast_Dingbats SDF_Holographic\">2</font> " + currAmmo;
        return;
    }

    /*
    public void UpdateModifications () {

        ps.mAmmo = (int)(maxAmmo * (ps.mAmmoLevel * ps.mAmmoPerLevel));
        //Debug.Log("" + mAmmo);
        mReload = mReloadLevel * mReloadPerLevel;
        mFireRate = mFireRateLevel * mFireRatePerLevel;
        mProjectileDuration = mProjectileDurationLevel * mProjectileDurationPerLevel;
        mProjectileScale = mProjectileScaleLevel * mProjectileScalePerLevel;
        mProjectileForce = mProjectileForceLevel * mProjectileForcePerLevel;

        barrel.fireRateMultiplier = 1.0f + mFireRate;
        arms.SetFloat("fireSpeed", 1.0f + mFireRate);
        arms.SetFloat("reloadSpeed", 1.0f + mReload);

        return;
    }
    */

    /*
    void MaxUpgrades () {

        Debug.Log("Max Upgrades");

        int x = godMode ? -1 : 10;

        mAmmoLevel = Mathf.Clamp(x, 0, mAmmoCap);
        mFireRateLevel = Mathf.Clamp(x, 0, mFireRateCap);
        mReloadLevel = Mathf.Clamp(x, 0, mReloadCap);
        mProjectileScaleLevel = Mathf.Clamp(x, 0, mProjectileScaleCap);
        mProjectileForceLevel = Mathf.Clamp(x, 0, mProjectileForceCap);
        mProjectileDurationLevel = Mathf.Clamp(x, 0, mProjectileDurationCap);

        UpdateModifications();

        godMode = !godMode;
    }

    public void ModifyAll (bool b) {

        //Debug.Log("ModifyAll: " + b);

        int x = b ? 1 : -1;

        mAmmoLevel = Mathf.Clamp(mAmmoLevel + x, 0, mAmmoCap);
        mFireRateLevel = Mathf.Clamp(mFireRateLevel + x, 0, mFireRateCap);
        mReloadLevel = Mathf.Clamp(mReloadLevel + x, 0, mReloadCap);
        mProjectileScaleLevel = Mathf.Clamp(mProjectileScaleLevel + x, 0, mProjectileScaleCap);
        mProjectileForceLevel = Mathf.Clamp(mProjectileForceLevel + x, 0, mProjectileForceCap);
        mProjectileDurationLevel = Mathf.Clamp(mProjectileDurationLevel + x, 0, mProjectileDurationCap);

        UpdateModifications();
    }
    */

}

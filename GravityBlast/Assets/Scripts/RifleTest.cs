using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleTest : MonoBehaviour {

    public KeyCode firingKey = KeyCode.Mouse0;

    public int maxAmmo;
    private int currAmmo;
    private bool reloading;

    //halo assault rifle ref: 10.8 shots per sec, 905 m/s velocity, 300 m "effective range"
    public float shotCooldown;
    private float timeToFire = 0f;
    //private int sprayCount;
    public float bulletVelocity;
    public float maxRange;

    public LayerMask bulletCollision;

    public GameObject bulletPrefab;
    public GameObject firingPosition;
    public GameObject spreadDisplay;

    //audio stuff and particle effects


    
    // Start is called before the first frame update
    void Start() {
        currAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update() {
        if (timeToFire > 0) {
            timeToFire -= Time.deltaTime;
        }
            

        if (Input.GetKey(firingKey) && timeToFire <= 0 && !reloading) {
            timeToFire = shotCooldown;
            Fire();
        }
            
        

        

    }

    private void Fire () {

        Debug.Log("Firing");

        

        if (currAmmo == 0)
            return;

        currAmmo--;

        RaycastHit hitTarget;
        //Vector3 target = randCoordinates();
        //Vector3 target = SpraySample();
        Vector3 dir = firingPosition.transform.forward;//spreadDisplay.transform.position;//new Vector3(firingPosition.transform.position.x + target.x, firingPosition.transform.position.y + target.y, firingPosition.transform.position.z + target.z);

        if (Physics.Raycast(firingPosition.transform.position, dir, out hitTarget, maxRange)) {
            Debug.Log("Hit something");
            Debug.DrawLine(firingPosition.transform.position, hitTarget.point, Color.red, 1f);
        }
        Debug.DrawLine(firingPosition.transform.position, dir, Color.green, 1f);

        Debug.Log("Shot Fired");

    }

    private IEnumerator Reload () {
        reloading = true;
        //start animation
        yield return new WaitForSeconds(1); //should time it to when the new clip goes in
        currAmmo = maxAmmo;
        reloading = false;
    }

    
    private Vector3 SpraySample () {
        float r = 1f;
        //rand number within diameter
        float x = Random.Range(0, 2 * r) - r;
        float y = Random.Range(0, 2 * r) - r;
        Vector3 target = firingPosition.transform.forward;
        //target.x += x;
        //target.y += y;
        return spreadDisplay.transform.position;
    }
    

    private Vector3 randCoordinates () {
        int s = maxAmmo - currAmmo;
        float r;//adius

        if (s < 4) {
            r = (-.00738f * s * s * s) + (.0999f * s * s) - (.1799f * s) + .0873f;
        } else if (s < 10) {
            r = (.0016f * s * s * s) - (.0299f * s * s) + (.2467f * s) - .2492f;
        } else {
            r = 1;
        }

        //rand number within diameter
        float x = Random.Range(0, 2 * r) - r;
        float y = Random.Range(0, 2 * r) - r;

        float magnitude = Mathf.Sqrt(x*x + y*y);
        
        //if (magnitude <= Mathf.Clamp(r + ((s * r * .25f) - 10), 0, 2 * r)) {

        //} else {
        //    x = Random.Range(0, 2 * r) - r;
        //    y = Random.Range(0, 2 * r) - r;
        //    magnitude = Mathf.Sqrt(x * x + y * y);
        //}
        

        return (magnitude > r)? randCoordinates() : new Vector3(x, y, 10f);


    }

    private void DealDamage () {

    }

}

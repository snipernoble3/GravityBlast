using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : MonoBehaviour {

    private Transform target;
    private float distanceToTarget;

    private Rigidbody rb;

    public float speed;
    public float awarenessRange;
    public float attackRange;
    public float chargeTime;
    public float cooldownTime;

    private bool attacking = false;


    private void Awake () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update () {
        distanceToTarget = Vector3.Magnitude(transform.position - target.position);
    }

    private void FixedUpdate () {
        if (attacking) {
            //do nothing
            
        } else if (distanceToTarget < attackRange) {
            //begin attack
            StartCoroutine(Charge());
        } else if (distanceToTarget < awarenessRange) {
            //go towards player
            Vector3 v = target.position - transform.position;
            rb.AddRelativeForce(v.normalized * speed, ForceMode.Impulse);
        } else {
            //wander
        }
    }

    IEnumerator Charge () {
        attacking = true;
        Vector3 t = target.position;

        yield return new WaitForSeconds(chargeTime);
        //turn on damage collider
        Vector3 f = t - transform.position;
        rb.AddRelativeForce(f.normalized * f.magnitude * 4f, ForceMode.Impulse);
        //turn off damage collider
        yield return new WaitForSeconds(cooldownTime);

        attacking = false;
    }

}

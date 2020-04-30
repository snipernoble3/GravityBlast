using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour {

    private enum State { Attacking, Moving, Idle };
    private State currState;
    private bool stunned;

    private Rigidbody rb;
    private GameObject player;
    private God god;

    //health
    [SerializeField] int health;
    private GameObject[] hitBy;

    //xp
    [SerializeField] static GameObject xpPrefab;
    [SerializeField] int XP;

    //movement
    private float moveSpeed;
    private float turnSpeed;

    //attack


    private void Start () {
        rb = GetComponent<Rigidbody>();
        currState = State.Idle;
        hitBy = new GameObject[health];
    }


    public void TakeDamage (GameObject fromObject, int amount = 1) {

        foreach (GameObject g in hitBy) {
            if (g == fromObject) {
                return;
            }
        }

        health -= amount;
        if (health <= 0) {
            OnDeath();
        }

        for (int i = 0; i < hitBy.Length; i++) {
            if (hitBy[i] == null) {
                hitBy[i] = fromObject;
            }
        }
    }

    public IEnumerator Stun (float seconds) {

        stunned = true;
        //show stun icon/effect?
        yield return new WaitForSeconds(seconds);
        //hide stun icon/effect
        stunned = false;

    }

    private void OnDeath () {
        //spawn xp
        for (int i = 0; i < XP; i++) {
            //instantiate xp
        }

        //destroy self


    }


}

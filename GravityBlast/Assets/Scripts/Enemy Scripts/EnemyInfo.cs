using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour {

    private enum State { Attacking, Moving, Idle };
    private State currState;
    private bool stunned;

    [HideInInspector] public God god;
    [HideInInspector] public ObjectPool objectPool;
    private Rigidbody rb;
    private GameObject player;
    

    //health
    [SerializeField] int health;

    //xp
    //[SerializeField] static GameObject xpPrefab;
    [SerializeField] int xpValue;

    //movement
    private float moveSpeed;
    private float turnSpeed;

    //attack


    private void Start () {
        rb = GetComponent<Rigidbody>();
        player = god.player;
        currState = State.Idle;
    }

    public void TakeDamage (int amount = 1) {
        
        health -= amount;

        if (health <= 0) {
            OnDeath();
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
        for (int i = 0; i < xpValue; i++) {
            GameObject xp = god.xpPool.SpawnObject();
            xp.transform.position = transform.position;
            xp.GetComponent<Gravity_AttractedObject>().CurrentGravitySource = gameObject.GetComponent<Gravity_AttractedObject>().CurrentGravitySource;
            xp.SetActive(true);
        }

        //despawn self
        objectPool.DespawnObject(gameObject);

    }


}

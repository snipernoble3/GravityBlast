using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleQueen : EnemyInfo {

    private State currState;

    private Rigidbody rb;

    private bool outOfPlay = false;

    // Movement
    float moveForce = 900f;
    float lookSpeed = 3f;
    float randomForceMod;


    //private Transform target;
    Quaternion targetLookDirection;
    //Vector3 tempRotation;

    // Beetle Spawning
    int maxSpawn = 10; // idk if we want this?
    float minTimeToSpawn = 3f;
    float maxTimeToSpawn = 5f;

    // Counters
    int hasSpawned = 0;
    float timeToSpawn = 0f;
    float pickNewDirection = 0f;


    private void Start () {
        currState = State.Idle;
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        gravityScript = GetComponent<Gravity_AttractedObject>();
        randomForceMod = Random.Range(0.75f, 1.25f);
        //target = Camera.main.transform;
    }


    private void Update () {
        if (timeToSpawn > 0) {
            timeToSpawn -= Time.deltaTime;
        }

        if (pickNewDirection > 0) {
            pickNewDirection -= Time.deltaTime;
        } else {
            //rb.AddRelativeTorque(0, 500, 0);
        }

        //movement

        switch (currState) {
            case State.Running:
                targetLookDirection = Quaternion.LookRotation(transform.position - playerStats.gameObject.transform.position, transform.position - GameManager.currPlanet.transform.position);
                //raycast up and down, use the lowest cast with no collision
                transform.rotation = Quaternion.Slerp(transform.rotation, targetLookDirection, lookSpeed);
                break;
            case State.Idle:
                if (pickNewDirection <= 0) {

                    pickNewDirection = 5f;
                }


                break;
        }


        //move forward
        //rb.AddRelativeForce(Vector3.forward * moveForce);
        Vector3 tempV = transform.forward * 5f;
        rb.velocity = tempV + (transform.up * -1f);

        //spawning
        if (currState == State.Running && timeToSpawn <= 0) {
            SpawnBeetle();
            timeToSpawn = Random.Range(minTimeToSpawn, maxTimeToSpawn);
        }

    }


    private void SpawnBeetle () {
        Debug.Log("Spawning Beetle");
        //GameObject newBeetle = PrefabManager.enemyPools[0].SpawnObject(); // will need to change this to find the proper enemy pool
        //move to spawn position
        //set important info
        //SetActive
    }

    public override void PlayerEnteredRange () {
        currState = State.Running;
    }

    public override void PlayerExitRange () {
        currState = State.Idle;
    }

    public override void OutOfPlay () {
        throw new System.NotImplementedException();
    }

}
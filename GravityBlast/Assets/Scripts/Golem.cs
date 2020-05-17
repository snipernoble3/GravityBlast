using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonoBehaviour {

    private GameObject target;
    public GameObject projectile;
    public GameObject throwingPosition;
    private Rigidbody rb;

    [SerializeField] private float moveForce;
    [SerializeField] private float attackRange = 50f;
    [SerializeField] private float projectileVelocity;
    [SerializeField] private float projectileThrowRate;

    private float grabDiameter = 5f;
    private float timeToNextAttack;

    private bool attacking;
    private bool aiming;
    private bool moving;
    private bool waiting;
    private float timeMoving;
    private float timeToNextMove;

    private Vector2 randLocation;
    private Vector3 targetLocation;

    private void Awake () {
        target = GameObject.FindGameObjectWithTag("Player");

        rb = GetComponent<Rigidbody>();
    }

    private void Update () {
        if (timeToNextAttack > 0) {
            timeToNextAttack -= Time.deltaTime;
        }

        if (timeMoving > 0) {
            timeMoving -= Time.deltaTime;
        }

        if (timeToNextMove > 0) {
            timeToNextMove -= Time.deltaTime;
        }

        if (Vector3.Magnitude(target.transform.position - transform.position) < 50f && timeToNextAttack <= 0 && !attacking) { //if player in range and able to attack and not already attacking
            //start attack
            attacking = true;
            moving = false;
            waiting = false;
            StartCoroutine(Attack());

        } else if (aiming) {

            Quaternion targetLookDirection = Quaternion.LookRotation(target.transform.position - transform.position, transform.position - GetComponent<Gravity_AttractedObject>().CurrentGravitySource.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetLookDirection, 5f * Time.deltaTime);

        } else if (!attacking) { //if not attacking
            //idle
            if (!moving && !waiting) {
                timeToNextMove = 0.2f;
                waiting = true;
            }

            if (moving) {
                //pick a new direction to move in

                //move and rotate to the move direction
                Quaternion targetLookDirection = Quaternion.LookRotation(targetLocation - transform.position, transform.position - GetComponent<Gravity_AttractedObject>().CurrentGravitySource.transform.position);//, transform.position - GetComponent<Gravity_AttractedObject>().GetGravitySource().transform.position);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLookDirection, 5f * Time.deltaTime);

                //rb.AddRelativeForce(transform.forward * moveForce, ForceMode.Impulse);
                transform.position += transform.forward * moveForce * Time.fixedDeltaTime;
                float distance = Vector3.Magnitude(targetLocation - transform.position);
                if (timeMoving <= 0 || Mathf.Abs(distance) <= 2f) {
                    timeToNextMove = Random.Range(1f, 4f);
                    moving = false;
                    waiting = true;
                }

            } else if (waiting) {
                //wait for a bit
                if (timeToNextMove <= 0) {
                    timeMoving = Random.Range(1f, 4f);
                    moving = true;
                    waiting = false;
                    randLocation = Random.insideUnitCircle * 20f;
                    targetLocation = new Vector3(transform.position.x + randLocation.x, transform.position.y, transform.position.z - randLocation.y);
                    TargetLocationCorrection();
                }
            }

        }
        

        if (Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.LeftControl)) {
            attacking = true;
            moving = false;
            waiting = false;
            StartCoroutine(Attack());
        }

    }


    private void TargetLocationCorrection () {
        RaycastHit up;
        RaycastHit down;

        if (Physics.Raycast(targetLocation, targetLocation - gameObject.GetComponent<Gravity_AttractedObject>().CurrentGravitySource.transform.position, out down)) {
            if (down.collider.gameObject.tag == "Planet") {
                targetLocation = down.point;
            }
        }

        if (Physics.Raycast(targetLocation, targetLocation + gameObject.GetComponent<Gravity_AttractedObject>().CurrentGravitySource.transform.position, out up)) {
            if (up.collider.gameObject.tag == "Planet") {
                targetLocation = up.point;
            }
        }

    }

    private IEnumerator Attack () {
        //find and grab throw object
        GameObject throwable = FindThrowable();

        yield return new WaitForSeconds(1f); //temporary wait, just to visually see the cube appear

        //pick up and get into throwing position
        PickUp(throwable);

        yield return new WaitForSeconds(1f); //change to wait until object in throw position
        
        //charge up
        yield return new WaitForSeconds(0.5f); //charge throw

        aiming = false;

        //throw object
        ThrowObject(throwable);

        yield return new WaitForSeconds(0.5f); //throw/follow through/reset to normal

        timeToNextAttack = 3f;
        attacking = false;
    }

    private GameObject FindThrowable () {
        GameObject canThrowThis = null;
        //search in area around for any beetles
        //if one is found, return it
        //else grab a terrain piece
        Vector2 spawnPoint = Random.insideUnitCircle * 6;
        //spawnPoint.x += 2.5f; //doesnt work for negatives
        //spawnPoint.y += 2.5f; //see above
        canThrowThis = Instantiate(projectile, new Vector3(transform.position.x + spawnPoint.x, transform.position.y, transform.position.z + spawnPoint.y), transform.rotation);
		//canThrowThis.transform.GetComponent<Gravity_AttractedObject>().SetGravitySource(transform.GetComponent<Gravity_Source>()); // Set the project's gravity source to match the golem's gravity source.
        return canThrowThis;
    }

    private void PickUp (GameObject throwable) {
        //send a stun message to the game object (in case its a moving target)
        //run animation to pick the game object up
        throwable.transform.position = Vector3.MoveTowards(throwable.transform.position, throwingPosition.transform.position, Vector3.Magnitude(throwingPosition.transform.position - throwable.transform.position));
        throwable.transform.parent = transform;
        //throwable.transform.position = throwingPosition.transform.position;
        //parent the game object when contacted
        aiming = true;
    }

    private void ThrowObject (GameObject throwable) {
        throwable.transform.parent = null;
        Vector3 forwardVector = target.transform.position - throwingPosition.transform.position;
        throwable.transform.rotation = Quaternion.FromToRotation(throwingPosition.transform.rotation.eulerAngles, forwardVector);
        Rigidbody tRB = throwable.GetComponent<Rigidbody>();
        tRB.isKinematic = false;
        tRB.useGravity = true;
        tRB.AddForce(forwardVector.normalized * 50, ForceMode.VelocityChange);
        throwable.GetComponent<BoxCollider>().isTrigger = false;
        Destroy(throwable, 5.0f);
    }

}

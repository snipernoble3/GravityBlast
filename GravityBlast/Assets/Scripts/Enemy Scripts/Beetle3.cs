using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle3 : EnemyInfo {

    private State currState;

    private Rigidbody rb;

    private bool outOfPlay = false;
    bool isGrounded;
	public LayerMask raycastMask;

    //target tracking
    private Transform target;
    private Quaternion targetLookDirection;
    private bool targetInRange;

    //movement
    Vector3 newVelocity;
    private float moveSpeed = 15f;
    private float turnSpeed = 3.5f;
    private float randomSpeedChange = 1f;

    //attack
    public float attackRange = 5f;
    public float chargeSpeed = 20f;
    public int damageOnHit = 1;

    private bool attacking;
    private bool charging;
    private bool dealtDamage;
    private bool cooldown;
    private Vector3 hitLocation;
    private Vector3 lastLocation;
    public float channelTime = 0.25f;
    public float chargeTime = 0.5f;
    public float cooldownTime = 0.75f;

    //idle
    RaycastHit temporaryTarget;
    float timeSinceTargeting = 0f;
    float timeSinceHop = 0f;
	
	private Animator beetleAnimator;

    void Start () {
        rb = GetComponent<Rigidbody>();
		beetleAnimator = GetComponentInChildren<Animator>();
		
        gravityScript = GetComponent<Gravity_AttractedObject>();
        target = Camera.main.transform; //GameManager.gm.player.transform;

        randomSpeedChange = Random.Range(1f, 1.5f);
        timeSinceHop = Random.Range(3f, 7f);

        currState = State.Idle;
    }

    private void Update () {

        newVelocity = Vector3.zero;

        if (timeSinceTargeting > 0) {
            timeSinceTargeting -= Time.deltaTime;
        }
        if (timeSinceHop > 0) {
            timeSinceHop -= Time.deltaTime;
        }

        if (currState == State.Stunned) return;

		// GROUNDED CHECK:
		Transform gravitySurface = gravityScript.CurrentGravitySource.GetSurface();
		float groundLength = 0.5f; // Farther than this, the beetle won't be grounded.
		
		Vector3 groundDirection = gravitySurface.position - transform.position;
        //Debug.DrawRay(transform.position, groundDirection, Color.green);
		
		RaycastHit hit;
		if (Physics.Raycast(transform.position, groundDirection, out hit, groundLength, raycastMask, QueryTriggerInteraction.Ignore))
		{
			//Debug.Log("We're in the air!");			
			isGrounded = true;
			rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			if (beetleAnimator != null) beetleAnimator.SetBool("isFlying", false);
		}
        else
		{
			//Debug.Log("We're on the ground!");
			isGrounded = false;
			rb.constraints = RigidbodyConstraints.None;
			if (beetleAnimator != null) beetleAnimator.SetBool("isFlying", true);
		}
		//


        switch (currState) {
            case State.Attacking:
                if (!attacking) {
                    StartCoroutine(Attack());
                }

                if (charging) {
                    transform.position += transform.forward * chargeSpeed * Time.deltaTime;
                } else if (!cooldown) {
                    targetLookDirection = Quaternion.LookRotation(hitLocation - transform.position, transform.position - gravityScript.CurrentGravitySource.transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetLookDirection, turnSpeed * Time.deltaTime);
                }

                break;
            case State.Chasing:
                if (!targetInRange) currState = State.Idle;

                /*
                if (timeSinceHop <= 0) {
                    RandomHop();
                    timeSinceHop = Random.Range(2f, 5f);
                }
                */

                targetLookDirection = Quaternion.LookRotation(target.position - transform.position, transform.position - gravityScript.CurrentGravitySource.transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetLookDirection, turnSpeed * randomSpeedChange * Time.deltaTime);

                RaycastHit hitForward;
                if (Physics.Raycast(transform.position, transform.forward, out hitForward, attackRange)) {
                    if (hitForward.transform.tag == "Player") {
                        hitLocation = hitForward.transform.position;
                        currState = State.Attacking;
                    }
                }

                //transform.position += transform.forward * moveSpeed * randomSpeedChange * Time.deltaTime;
                newVelocity = transform.forward * moveSpeed * randomSpeedChange;
                //if hopTime <= 0 newVelocity += hop

                break;
            case State.Idle:
                
                if (targetInRange) {
                    currState = State.Chasing;

                } else if (outOfPlay) {
                    targetLookDirection = Quaternion.LookRotation(GameManager.currPlanet.gameObject.transform.position - transform.position); //, transform.position - gravityScript.CurrentGravitySource.transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetLookDirection, turnSpeed * randomSpeedChange * Time.deltaTime);
                    rb.velocity = transform.forward * moveSpeed * moveSpeed * randomSpeedChange;
                } else {
                    if (timeSinceTargeting <= 0) {
                        temporaryTarget = GameManager.currPlanet.RandomSpawnPoint();
                        timeSinceTargeting = Random.Range(3f, 7f);
                    }

                    /*
                    if (timeSinceHop <= 0) {
                        RandomHop();
                        timeSinceHop = Random.Range(2f, 5f);
                    }
                    */

                    targetLookDirection = Quaternion.LookRotation(temporaryTarget.point - transform.position, transform.position - gravityScript.CurrentGravitySource.transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetLookDirection, turnSpeed * 0.5f * randomSpeedChange * Time.deltaTime);
                    newVelocity = transform.forward * moveSpeed * 0.75f * randomSpeedChange;
                    
                }

                break;
        }

        
        if (!isGrounded)
		{
            Vector3 down = -1 * (transform.position - gravitySurface.position);
            down = down.normalized;
            down *= Mathf.Clamp(hit.distance * hit.distance, 0, 30f);
            newVelocity += down;
        }
        

        //rb.velocity = newVelocity;
        rb.AddForce(newVelocity * rb.mass);
        
    }

    public override IEnumerator Stun (float seconds) {

        //Debug.Log(name + " is stunned");

        currState = State.Stunned;
        //show stun icon/effect?
        yield return new WaitForSeconds(seconds);
        //hide stun icon/effect
        currState = State.Idle;

        //Debug.Log(name + " is unstunned");

    }

    private void OnCollisionEnter (Collision collision) {

        if (charging && !dealtDamage) {

            if (collision.gameObject == playerStats.gameObject) {
                playerStats.UpdateHealth(-damageOnHit);
                dealtDamage = true;
            }

        }

        if (collision.gameObject.tag == "Bullet") {
            //Debug.Log(name + " has taken damage");
            TakeDamage(1);
            Destroy(collision.gameObject);
        }

    }

    IEnumerator Attack () {
        attacking = true;

        yield return new WaitForSeconds(channelTime);

        charging = true;

        yield return new WaitForSeconds(chargeTime);

        charging = false;
        dealtDamage = false;
        cooldown = true;

        yield return new WaitForSeconds(cooldownTime);
        cooldown = false;
        attacking = false;
        currState = State.Idle;
    }

    private void RandomHop () {
        Vector2 randomCircle = Random.insideUnitCircle * 1.5f;
        Vector3 randomPoint = transform.up * -1.5f;
        randomPoint += transform.right * randomCircle.x;
        randomPoint += transform.forward * randomCircle.y;
        //rb.AddForce(transform.up * 200f);
        rb.AddExplosionForce(200f, randomPoint, 5f);
    }

    public override void PlayerEnteredRange () {
        targetInRange = true;
    }

    public override void PlayerExitRange () {
        targetInRange = false;
    }

    public override void OutOfPlay () {
        outOfPlay = true;
    }

}

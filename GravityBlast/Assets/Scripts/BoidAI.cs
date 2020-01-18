using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAI : MonoBehaviour {

    /**
     * scores
     * player score: targeting range / distance from player
     * separation score: min distance / distance to nearest unit
     * cohesion score: distance to center / min distance
     * alignment score: angle between avg direction and current direction
     * object avoidance: raycasts in front of boid, steer in direction of least collision, only apply other rules that dont steer into obj if obj is within min distance
     *                   maybe get normal of contact point and use that to turn the boid away from the object?
     * 
     * 
     * unit vectors of each force * score
     * combine vectors
     * unit vector of combination * steering force(speed)
     * 
     * OR
     * 
     * handle each vector from the weakest link out
     * compare two scores, bigger one wins
     * 
     * OR
     * 
     * divide area around boids into boxes (center of rubix cube)
     * only move to boxes with no obstacles
     * move in the best box to get avg direction while avoiding boxes with units
     * 
     */
    
    private Rigidbody rb;
    public float speed = 1f;
    public float neighborRadius = 10f;
    public float minSeparationDistance = 2f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1f;
    //public float obstacleWeight = 1f;
    //public float targetWeight = 1f;

    Collider[] neighbors;
    float totalNeighbors;
    Vector3 center;
    Vector3 avgDirection;
    Vector3 closestUnit;


    void Awake () {
        

        rb = GetComponent<Rigidbody>();
        
    }

    void Update () {

        CheckForNeighbors();

    }

    void FixedUpdate () {
        //apply force
        //transform.position += GetDirection();
        transform.LookAt(GetDirection());
        transform.position += transform.forward * speed;
    }

    Vector3 GetDirection () {
        Vector3 dir = transform.position;
        var alignment = SteerTowards(avgDirection) * Vector3.Dot(dir, avgDirection) * alignmentWeight;
        var cohesion = SteerTowards(center - transform.position) * ((Vector3.Magnitude(transform.position - center)) / minSeparationDistance) * cohesionWeight;
        var separation = SteerTowards(closestUnit) * -1 * (minSeparationDistance / Vector3.Magnitude(closestUnit)) * separationWeight;
        //var obstacle = SteerTowards() * obstacleWeight;
        dir += alignment;
        dir += cohesion;
        dir += separation;
        return dir;
    }

    Vector3 SteerTowards (Vector3 vector) {
        Vector3 v = vector.normalized;
        return v;
    }

    void CheckForNeighbors () {
        totalNeighbors = 0;
        avgDirection = transform.forward;
        closestUnit = transform.position + transform.forward * 10;
        

        neighbors = Physics.OverlapSphere(transform.position, neighborRadius);
        foreach(Collider n in neighbors) {
            if (n.gameObject.GetComponent<BoidAI>()) {
                totalNeighbors++;
                center += n.gameObject.transform.position;
                avgDirection += transform.forward;
                if (Vector3.Magnitude(transform.position - closestUnit) > Vector3.Magnitude(transform.position - n.gameObject.transform.position)) closestUnit = n.gameObject.transform.position;
            }
        }

        center /= totalNeighbors;
        avgDirection /= totalNeighbors;


    }

}

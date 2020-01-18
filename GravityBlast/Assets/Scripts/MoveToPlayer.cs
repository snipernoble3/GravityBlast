using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPlayer : MonoBehaviour {

    private GameObject target;
    [SerializeField] private float speed = 0.1f;

    private void Awake () {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update () {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + Vector3.up, speed);
    }


}

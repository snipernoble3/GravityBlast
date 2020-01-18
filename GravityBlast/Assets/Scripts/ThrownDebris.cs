using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownDebris : MonoBehaviour {

    private void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.GetComponent<Health>().GetTag() == Health.AlignmentTag.Player) {
            collision.gameObject.GetComponent<Health>().TakeDamage(1);
            gameObject.SetActive(false);
        } else {
            StartCoroutine(turnOff());
        }
        
    }

    IEnumerator turnOff () {
        yield return new WaitForSeconds(4f);
        gameObject.SetActive(false);
    }

}

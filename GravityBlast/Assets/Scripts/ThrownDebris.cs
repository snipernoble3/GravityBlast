using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownDebris : MonoBehaviour
{
    private void OnCollisionEnter (Collision collision)
	{
        Health health = collision.gameObject.GetComponent<Health>();
		
		if (health != null)
		{			
			if (health.GetTag() == Health.AlignmentTag.Player)
			{
				health.TakeDamage(1);
				gameObject.SetActive(false);
			}
			else StartCoroutine(TurnOff());
		}
    }

    private IEnumerator TurnOff()
	{
        yield return new WaitForSeconds(4.0f);
        gameObject.SetActive(false);
    }
}

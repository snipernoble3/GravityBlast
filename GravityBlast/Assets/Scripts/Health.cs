using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class Health : MonoBehaviour {

    public enum AlignmentTag { Player, Ally, Enemy, Neutral };
    [SerializeField] AlignmentTag alignment;

    [SerializeField] int maxHealth = 3;
    private int currHealth;

    public TextMeshProUGUI healthText;
    private string baseText;
	private int maxPips = 10;

    public ParticleSystem death;
    public ParticleSystem critDeath;
    public int xpDrop;
    public GameObject XP;

    private bool alive = true;
    
    void Awake() {
        FullHeal();

        try {
            baseText = healthText.text;
        } catch (NullReferenceException e) {

        }
        
        UpdateUI();
    }

    private void OnCollisionEnter (Collision collision) {
        if (alignment != AlignmentTag.Player && collision.gameObject.tag == "Bullet") {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    public void GainHealth (int heal) {
        if (alive) {
            currHealth = Mathf.Min(currHealth + heal, maxHealth);
            UpdateUI();
        }
    }

    public void TakeDamage (int damage) {
        currHealth -= damage;
        UpdateUI();
        if (currHealth <= 0) {
            alive = false;
            if (death != null) {
                Instantiate(death, transform.position, transform.rotation);
            }
            if (XP != null) {
                for (int i = 0; i < xpDrop; i++) {
                    Instantiate(XP, transform.position + UnityEngine.Random.insideUnitSphere, transform.rotation);
                }
            }
            OnDeath();
        }
    }

    void UpdateUI () {
        try {
            
			//healthText.text = baseText + currHealth;

			healthText.text = "0"; // 0 is the "shield" symbol in the dingbats
			for (int i = 1; i <= currHealth && i <= maxPips; i++)
			{
				healthText.text += "1"; // 1 is the "pip" symbol in the dingbats
				if (i == maxPips) healthText.text += "+"; // Indicate that there is more health than is currently displayed in the number of pips.
			}
			
			//healthText.text = currHealth.ToString(); // Don't print the original text anymore.
        } catch (NullReferenceException e) {

        }
    }

    public void Kill () {
        currHealth = 0;
        alive = false;
        if (death != null) {
            Instantiate(death, transform.position, transform.rotation);
        }
        if (XP != null) {
            for (int i = 0; i < xpDrop; i++) {
                Instantiate(XP, transform.position + UnityEngine.Random.insideUnitSphere, transform.rotation);
            }
        }
        OnDeath();
    }

    public void CritKill () {
        //Debug.Log("Crit Death");
        Instantiate(critDeath, transform.position, transform.rotation);
        for (int i = 0; i < xpDrop * 2; i++) {
            Instantiate(XP, transform.position + UnityEngine.Random.insideUnitSphere, transform.rotation);
        }
        currHealth = 0;
        alive = false;
        OnDeath();
    }

    public void FullHeal () {
        currHealth = maxHealth;
    }

    private void OnDeath () {
        switch (alignment) {
            case AlignmentTag.Player:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            default:
                this.gameObject.SetActive(false);
                break;
        }
        
    }
    /*
    IEnumerator OnDeath () {
        //play death animation
        yield return new WaitForEndOfFrame();
        this.gameObject.SetActive(false);
    }
    */

    //if tag != player on mouse over show healthbar if not full health

    public bool IsAlive () { return alive; }

    public AlignmentTag GetTag () { return alignment; }
    

}

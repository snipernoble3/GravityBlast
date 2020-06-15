using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyInfo : MonoBehaviour {

    public enum State { Attacking, Chasing, Idle, Running, Stunned };

    [HideInInspector] public ObjectPool objectPool;
    [HideInInspector] public Player_Stats playerStats;
    [HideInInspector] public Gravity_AttractedObject gravityScript;

    [SerializeField] int health;
    [SerializeField] int xpValue;

    bool dying = false;

    [SerializeField] GameObject model;
    [SerializeField] ParticleSystem deathEffect;

    public void TakeDamage (int amount = 1) {
        
        health -= amount;

        if (health <= 0 && !dying) {
            StartCoroutine(OnDeath());
            dying = true;
        }
        
    }

    private IEnumerator OnDeath () {
        deathEffect.gameObject.SetActive(true);
        model.SetActive(false);
        
        //spawn xp
        for (int i = 0; i < xpValue; i++) {
            GameObject xp = PrefabManager.xpPool.SpawnObject();
            xp.transform.position = transform.position;
            xp.GetComponent<Gravity_AttractedObject>().CurrentGravitySource = gravityScript.CurrentGravitySource;
            xp.SetActive(true);
        }

        yield return new WaitForSeconds(deathEffect.main.duration);

        //despawn self
        deathEffect.gameObject.SetActive(false);
        model.SetActive(true);
        dying = false;
        objectPool.DespawnObject(gameObject);

    }

    public abstract IEnumerator Stun (float seconds);

    public abstract void PlayerEnteredRange ();

    public abstract void PlayerExitRange ();

    public abstract void OutOfPlay ();

}

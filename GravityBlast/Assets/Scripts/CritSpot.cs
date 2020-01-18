using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritSpot : MonoBehaviour {

    private enum CritType { FlatBonus, Multiplier, MaxValue};
    [SerializeField] private CritType type;
    [SerializeField] private int mod;

    private Health healthScript;
    public enum AlignmentTag { Player, Ally, Enemy, Neutral };
    private AlignmentTag alignment;

    private int h;
    private int d;

    private void Awake () {
        //find health script on gameobject
        healthScript = transform.parent.GetComponent<Health>();
        SetAlignment();
    }

    private void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag == "Bullet") {
            TakeDamage(-1);
            Destroy(collision.gameObject);
        }
    }

    private void SetAlignment () {
        switch (transform.parent.GetComponent<Health>().GetTag()) {
            case Health.AlignmentTag.Player:
                alignment = AlignmentTag.Player;
                break;
            case Health.AlignmentTag.Ally:
                alignment = AlignmentTag.Ally;
                break;
            case Health.AlignmentTag.Enemy:
                alignment = AlignmentTag.Enemy;
                break;
            case Health.AlignmentTag.Neutral:
                alignment = AlignmentTag.Neutral;
                break;
        }
    }

    private int ModifyValue (int v) {
        int value = v;

        switch (type) {
            case CritType.FlatBonus:
                value += mod;
                break;
            case CritType.Multiplier:
                value *= mod;
                break;
            case CritType.MaxValue: //insta kill or full health regen
                value = -1;
                break;
        }

        return value;

    }

    public void GainHealth (int heal) {
        h = ModifyValue(heal);
        if (h == -1) {
            healthScript.FullHeal();
        } else {
            healthScript.GainHealth(h);
        }
    }

    public void TakeDamage (int damage) {
        d = ModifyValue(damage);
        if (d == -1) {
            healthScript.CritKill();
        } else {
            healthScript.TakeDamage(d);
        }

    }

    public AlignmentTag GetTag () { return alignment; }

}

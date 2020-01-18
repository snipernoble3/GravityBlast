using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerResources : MonoBehaviour {

    public ParticleSystem depositLight;

    private int currXP;
    [SerializeField] private int xpCap = 20;
    public bool jumpReady;
    //private int currLvl;
    private int currResourceA; //currency?
    private int currResourceB; //rare gem? battery? turn in to ship for xp + currency

    [SerializeField] private GameObject jumpReadyText;
    //[SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private Slider xpBar;
    //[SerializeField] private TextMeshProUGUI resourceAText;
    //[SerializeField] private TextMeshProUGUI resourceBText;

    private string baseXPText;
    private string baseResourceAText;
    private string baseResourceBText;

    private void Awake () {
        //set base text
        //baseXPText = xpText.text;
        //baseResourceAText = resourceAText.text;
        //baseResourceBText = resourceBText.text;
        
		if (xpBar != null)
		{
			xpBar.minValue = 0;
			xpBar.maxValue = xpCap;
		}
    }

    private void Update () {
        //inventory.SetActive(Input.GetKey(KeyCode.Tab));
        //if (inventory.activeInHierarchy) UpdateUI();

        jumpReady = currXP >= xpCap;
        if (jumpReadyText != null) jumpReadyText.SetActive(jumpReady);
        

    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.name == "Ship") {
            DepositResourceB();
            Instantiate(depositLight, other.gameObject.transform.position, other.gameObject.transform.rotation, other.gameObject.transform);
        }

        if (other.gameObject.GetComponent<MoveToPlayer>()) {
            AddXP(1);
            UpdateUI();
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Resource B") {
            AddResourceB(1);
            other.gameObject.SetActive(false);
        }

    }

    void AddXP (int amount) { currXP += amount; }
    //void RemoveXP (int amount) { currXP -= amount; }

    void AddResourceA (int amount) { currResourceA += amount; }
    void RemoveResourceA (int amount) { currResourceA -= amount; }

    void AddResourceB (int amount) { currResourceB += amount; }
    void RemoveResourceB (int amount) { currResourceB -= amount; }
    void EmptyResourceB () { currResourceB = 0; }

    void DepositResourceB () {
        if (currResourceB == 0) return;
        //1 B : 25 XP, 10 A
        AddXP(currResourceB * 25);
        AddResourceA(currResourceB * 10);
        EmptyResourceB();
    }

    void DepositResourceB (int amount) {
        if (currResourceB == 0) return;
        //1 B : 25 XP, 10 A
        int temp = (int)Mathf.Min(amount, currResourceB);
        AddXP(temp * 25);
        AddResourceA(temp * 10);
        RemoveResourceB(temp);
    }

    void UpdateUI () {
        //update text values
        //xpText.text = baseXPText + currXP;
        //resourceAText.text = baseResourceAText + currResourceA;
        //resourceBText.text = baseResourceBText + currResourceB;
        
		if (xpBar != null) xpBar.value = currXP;
        if (currXP >= xpCap) {
            //allow scene to be reloaded
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionNumber : MonoBehaviour {
    
    void Start() {
        GetComponent<TextMeshProUGUI>().text = "" + Application.version;
    }

}

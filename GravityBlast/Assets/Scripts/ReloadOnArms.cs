using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadOnArms : MonoBehaviour{

    [SerializeField] private GameObject g;

    void AnimReload () {
        g.GetComponent<Deliverance>().Reload();
    }

}

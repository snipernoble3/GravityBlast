using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRandomizer : MonoBehaviour
{
    [SerializeField] Material[] materials;
	
	// Start is called before the first frame update
    void Start()
    {
		int rand = Random.Range(0, materials.Length);
		GetComponent<Renderer>().material = materials[rand];
    }
}

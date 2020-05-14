using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRandomizer : MonoBehaviour
{
    [SerializeField] Mesh[] meshes;
	
	// Start is called before the first frame update
    void Start()
    {
		int rando = Random.Range(0, meshes.Length);
		
		GetComponent<MeshFilter>().mesh = meshes[rando];
    }
}

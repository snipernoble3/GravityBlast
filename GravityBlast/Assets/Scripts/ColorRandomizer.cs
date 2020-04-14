using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    private Material planetSurface;
	
	// Start is called before the first frame update
    void Start()
    {
        planetSurface = GetComponent<Renderer>().material;
		planetSurface.SetFloat("_Hue", Random.Range(0.0f, 1.0f));
    }
}

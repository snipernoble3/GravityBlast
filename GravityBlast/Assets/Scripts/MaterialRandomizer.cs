using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRandomizer : MonoBehaviour
{
    [SerializeField] Material[] materials;
	
	// Start is called before the first frame update
    void Start()
    {
		int rando = Random.Range(0, materials.Length);
		Material mat = new Material(materials[rando]);
		
		float h, s, v;
		Color.RGBToHSV(mat.GetColor("_Color"), out h, out s, out v);
        h = Random.Range(0.0f, 1.0f);
		mat.SetColor("_Color", Color.HSVToRGB(h, s, v));

		GetComponent<Renderer>().material = mat;
    }
}

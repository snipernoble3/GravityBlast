using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    private Material planetSurface;
	
	//private Vector3 MeshPosition;
	//private Vector3[] Verts;
	//private Vector3[] Farthest;
	
	// Start is called before the first frame update
    void Start()
    {
		planetSurface = GetComponent<Renderer>().material;
		planetSurface.SetFloat("_Hue", Random.Range(0.0f, 1.0f));
		
		CalculateSurfaceDistances();
    }
	
	private void CalculateSurfaceDistances()
	{
		Vector3[] positions = GetComponent<MeshFilter>().sharedMesh.vertices;
		
		float[] distances = new float[positions.Length];
		
		for (int i = 0; i < positions.Length; i++)
		{
			distances[i] = (positions[i] - Vector3.zero).sqrMagnitude;
		}              
		
		System.Array.Sort(distances, positions);
		
		planetSurface.SetFloat("_SurfaceInner", distances[0] / 50.0f);
		planetSurface.SetFloat("_SurfaceOuter", distances[distances.Length - 1] / 50.0f);
	}
}

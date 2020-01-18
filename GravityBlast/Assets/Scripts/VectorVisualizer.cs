using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorVisualizer : MonoBehaviour
{
    private RectTransform radar;
	private LineRenderer line;
	
    void Awake()
    {
        line = GetComponent<LineRenderer>();
		radar = transform.parent.GetComponent<RectTransform>();
		//line.SetPosition(0, radar.anchoredPosition);
		//line.SetPosition(0, Vector3.zero);
    }
	
	public void SetVector(Vector3 vec)
	{
		float radarImageSize = 512.0f; // In Pixels
		
		//float width_start = 0.002f;
		//float width_start = 1.0f / radarImageSize;
		float width_start = 0.00002f * radar.sizeDelta.x;
		float width_end = 0.5f * width_start;
		//float width_end = 0.0f * 1.0f / radarImageSize;
			
		line.SetWidth(width_start, width_end);
		
		vec = new Vector3(vec.x * radar.sizeDelta.x, vec.y * radar.sizeDelta.y, vec.z);
		vec *= 10.0f / radarImageSize;
		line.SetPosition(0, new Vector3(0.0f, 0.0f, vec.z));
		line.SetPosition(1, vec);
	}
}

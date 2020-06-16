using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class DOFHelper : MonoBehaviour
{
    [SerializeField] private VolumeProfile postProcessing;
	
	public float focusDistance;
	private float previousFocusDistance;
	
	public float focalLength;
	private float previousfocalLength;

    // Update is called once per frame
    void Update()
    {
		if (focusDistance != previousFocusDistance)
		{
			DepthOfField dof;
            if (postProcessing.TryGet(out dof))
            {
				dof.focusDistance.value = focusDistance;
				previousFocusDistance = focusDistance;
			}
		}
		
		if (focalLength !=  previousfocalLength)
		{
			DepthOfField dof;
            if (postProcessing.TryGet(out dof))
            {
				dof.focalLength.value = focalLength;
				previousfocalLength = focalLength;
			}
		}
    }
}
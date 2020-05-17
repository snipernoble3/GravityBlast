using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool enableCameraShake_gpLanding = true;
	
	public IEnumerator Shake(float shake_amplitude, float shake_frequency, float shake_Duration, float fade_Duration_In, float fade_Duration_Out)
	{
		if (enableCameraShake_gpLanding) // If the player has chosen to disable camera shake, then do nothing.
		{
			if (fade_Duration_In + fade_Duration_Out < shake_Duration) // If there won't be enough time to complete the fade in and the fade out, then don't shake at all.
			{
				float fade_Min = 0.0f;
				float fade_Max = 1.0f;

				float fade_Multiplier = fade_Min; // Start with the shake faded out completely.
				float fade_Rate = fade_Max / fade_Duration_In; // Set the fade rate so that the shake will fade in over time.
				
				bool fade_isFadingIn = true;
				bool fade_isFadingOut = false;
				
				float shake_timeElapsed = 0.0f;
				float shake_Angle_Pitch = 0.0f;
				float shake_Angle_Roll = 0.0f;
				
				while (shake_timeElapsed < shake_Duration)
				{
					shake_Angle_Pitch = Mathf.Sin(Time.time * shake_frequency) * shake_amplitude * fade_Multiplier;
					shake_Angle_Roll = Mathf.Cos(Time.time * shake_frequency) * shake_amplitude * fade_Multiplier * 0.5f;
					
					// Increase or decrease the fade multiplier
					if (fade_isFadingIn || fade_isFadingOut)
					{
						fade_Multiplier = Mathf.Clamp(fade_Multiplier + (fade_Rate * Time.deltaTime), fade_Min, fade_Max);
						if (fade_Multiplier == fade_Max) fade_isFadingIn = false;
					}
					
					// Check if it's time to start fading out
					if (!fade_isFadingIn && !fade_isFadingOut && shake_timeElapsed >= shake_Duration - fade_Duration_Out)
					{
						fade_Rate = fade_Max / -fade_Duration_Out;
						fade_isFadingOut = true;
					}
					
					// Apply the shake to the camera
					transform.localEulerAngles = new Vector3(shake_Angle_Pitch, 0.0f, shake_Angle_Roll);	
					
					shake_timeElapsed += Time.deltaTime;

					yield return null;
				}
				
				// Ensure that the camera is back to zero when the shake is done.
				transform.localRotation = Quaternion.identity;
			}
			else Debug.Log("Fade duration ("+ (fade_Duration_In + fade_Duration_Out) +" seconds) can not be longer than the shake duration (" + shake_Duration + " seconds). The camera shake was not performed.");
		}
	}
}

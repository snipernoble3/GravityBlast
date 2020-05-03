using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoIntro : MonoBehaviour
{
    public Animator logo_CameraAnimator;
	public Animator logo_Animator;
	public GameObject logo_Solid;
	public GameObject logo_Fractured;
	
	// Start is called before the first frame update
    void Start()
    {
        logo_CameraAnimator.Play("LogoCameraIntroAnimation", 0, 0.0f);
		StartCoroutine(WaitForMusic());
    }

    IEnumerator WaitForMusic()
    {
        yield return new WaitForSeconds(6.7f);
		FractureLogo();
    }
	
	void FractureLogo()
	{
		logo_Solid.SetActive(false);
		logo_Fractured.SetActive(true);
		logo_Animator.SetFloat("playBackSpeed", 0.25f);
	}
}

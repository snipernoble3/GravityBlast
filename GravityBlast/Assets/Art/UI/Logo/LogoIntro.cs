using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoIntro : MonoBehaviour
{
    public Animator logo_CameraAnimator;
	public Animator logo_Animator;
	public GameObject logo_Solid;
	public GameObject logo_Fractured;
	public CanvasGroup menuOptions;
	public AudioSource music;
	
	private Coroutine WaitForMusicCo;
	private Coroutine MenuFadeInCo;
	
	private bool introIsFinished = false;
	
    void Start()
    {
        music.Play();
		
		menuOptions.alpha = 0.0f;
		menuOptions.interactable = false;
		logo_CameraAnimator.Play("LogoCameraIntroAnimation", 0, 0.0f);
		
		WaitForMusicCo = StartCoroutine(WaitForMusic());
    }
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !introIsFinished)
		{
			IntroEnd();
		}
	}
	
	void IntroEnd()
	{
		if (WaitForMusicCo != null) StopCoroutine(WaitForMusicCo); // Stop the coroutine if it's still going.
		if (MenuFadeInCo != null) StopCoroutine(MenuFadeInCo); // Stop the coroutine if it's still going.
		
		logo_Solid.SetActive(false);
		logo_Fractured.SetActive(true);
		
		menuOptions.alpha = 1.0f; // Make the menu 100% visible.
		menuOptions.interactable = true; // Make the menu interactable.
		logo_CameraAnimator.Play("LogoCameraIntroAnimation", 0, 1.0f);  // Skip to the end of the camera animation;
		logo_Animator.Play("Fracture", 0, 1.0f); // Skip to the end of the fracture animation;
		
		introIsFinished = true;
	}

    IEnumerator WaitForMusic()
    {
        yield return new WaitForSeconds(7.0f);
		FractureLogo();
    }
	
	void FractureLogo()
	{
		logo_Solid.SetActive(false);
		logo_Fractured.SetActive(true);
		logo_Animator.Play("Fracture", 0, 0.0f); // Play the fracture animation.
		MenuFadeInCo = StartCoroutine(MenuFadeIn());
	}
		
	IEnumerator MenuFadeIn()
	{	
		float fadeValue = 0.0f;
		float fadeDuration = 1.0f; // How long it takes for the text to fade in in seconds.
		
		while (fadeValue < 1.0f)
		{
			fadeValue = Mathf.Clamp(fadeValue + Time.deltaTime / fadeDuration, 0.0f, 1.0f);
			menuOptions.alpha = fadeValue;
			
			yield return null;
		}
		
		IntroEnd();
	}
}

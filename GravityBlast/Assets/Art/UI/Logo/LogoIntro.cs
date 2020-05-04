using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoIntro : MonoBehaviour
{
    private Transform camPivot;
	private Animator camPivot_Animator;
	
	private Transform logoCam;
	private Vector3 logoCam_RestingPos;
	
	public GameObject logo_Solid;
	public GameObject logo_Fractured;
	private Animator fracture_Animator;
	
	private Transform gravityText;
	
	public CanvasGroup menuOptions;
	private AudioSource music;
	
	private Coroutine WaitForMusicCo;
	private Coroutine ShakeCameraCo;
	private Coroutine MenuFadeInCo;
	
	private bool introIsFinished = false;
	
    void Start()
    {
		camPivot = transform.Find("Camera Pivot");
		gravityText = transform.Find("Gravity");
		logoCam = camPivot.Find("Logo Camera");
		
		camPivot_Animator = camPivot.GetComponent<Animator>();
		fracture_Animator = logo_Fractured.GetComponent<Animator>();
		
		logoCam_RestingPos = logoCam.position;
		music = GetComponent<AudioSource>();
        
		music.Play();
		
		menuOptions.alpha = 0.0f;
		menuOptions.interactable = false;
		camPivot_Animator.Play("LogoCameraIntroAnimation", 0, 0.0f);
		
		WaitForMusicCo = StartCoroutine(WaitForMusic());
    }
	
	void Update()
	{
		if (!introIsFinished)
		{
			if (Input.GetKeyDown(KeyCode.Escape)) IntroEnd();
			
			gravityText.LookAt(new Vector3(gravityText.position.x, logoCam.position.y, logoCam.position.z));
		}
	}
	
	void IntroEnd()
	{
		if (WaitForMusicCo != null) StopCoroutine(WaitForMusicCo); // Stop the coroutine if it's still going.
		if (ShakeCameraCo != null) StopCoroutine(ShakeCameraCo); // Stop the coroutine if it's still going.
		if (MenuFadeInCo != null) StopCoroutine(MenuFadeInCo); // Stop the coroutine if it's still going.
		
		logo_Solid.SetActive(false);
		logo_Fractured.SetActive(true);
		
		menuOptions.alpha = 1.0f; // Make the menu 100% visible.
		menuOptions.interactable = true; // Make the menu interactable.
		
		camPivot_Animator.Play("LogoCameraIntroAnimation", 0, 1.0f);  // Skip to the end of the camera animation;
		
		logoCam.position = logoCam_RestingPos;
		
		fracture_Animator.Play("Fracture", 0, 1.0f); // Skip to the end of the fracture animation;
		
		introIsFinished = true;
	}

    IEnumerator WaitForMusic()
    {
        yield return new WaitForSeconds(7.0f);
		
		camPivot_Animator.Play("LogoCameraIntroAnimation", 0, 1.0f); // Skip to the end of the camera animation;
		camPivot_Animator.enabled = false;
		ShakeCameraCo = StartCoroutine(ShakeCamera());
    }
	
	void FractureLogo()
	{
		logo_Solid.SetActive(false);
		logo_Fractured.SetActive(true);
		fracture_Animator.Play("Fracture", 0, 0.0f); // Play the fracture animation.
		MenuFadeInCo = StartCoroutine(MenuFadeIn());
	}
	
	IEnumerator ShakeCamera()
	{	
		float shakeProgress = 0.0f; // 0-1 value of progress through the shake.
		float shakeDuration = 0.3f; // Duration of the shake in seconds.
		float shakeIntensity = 0.07f; // The range of movement of the shake.
		
		while (shakeProgress < 1.0f)
		{
			shakeProgress = Mathf.Clamp(shakeProgress + Time.deltaTime / shakeDuration, 0.0f, 1.0f);
			
			float shakeRange = Mathf.Cos(shakeProgress * Mathf.PI * 2.0f) * shakeIntensity;

			logoCam.position = logoCam_RestingPos; // Reset the position to avoid compounding effect from the following statement.
			logoCam.position = logoCam.TransformPoint(new Vector3(Random.Range(-shakeRange, shakeRange), Random.Range(-shakeRange, shakeRange), 0.0f));
			
			yield return null;
		}
		
		logoCam.position = logoCam_RestingPos;
		
		FractureLogo();
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

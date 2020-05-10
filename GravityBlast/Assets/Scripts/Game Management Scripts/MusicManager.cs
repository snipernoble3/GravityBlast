using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup musicOutputGroup;
	
	private AudioSource[] music;
	private int activeLayer = 0; // The currently activeLayer in the music array.
	private int inactiveLayer = 1; // The currently activeLayer in the music array.
	private float defaultFadeDuration = 0.75f; // Duration of the music crossfade in seconds.
	private bool isCrossFading = false;

	private AudioSource transition;
    
	public AudioClip intro;
	public AudioClip mainAction;
	public AudioClip mainCalm;
    public AudioClip outro;
	public AudioClip jetpack;
	public AudioClip gameOver;

    void Start()
    {
		music = new AudioSource[2];
		
		for (int i = 0; i < music.Length; i++)
		{
			music[i] = gameObject.AddComponent<AudioSource>();
			music[i].outputAudioMixerGroup = musicOutputGroup;
			music[i].loop = true;
		}
		
		music[0].clip = mainAction;
		music[1].clip = mainCalm;
		
		transition = gameObject.AddComponent<AudioSource>();
		transition.outputAudioMixerGroup = musicOutputGroup;
		transition.clip = intro;
		transition.loop = false;
		
		StartCoroutine(PlayTransition(intro, true, true));
    }
	
	public void PlayIntro()
	{
		StartCoroutine(PlayTransition(intro, true, true));
	}

	public IEnumerator PlayTransition(AudioClip clipToPlay, bool shouldPauseMusic, bool shouldResumeMusic)
	{
		if (shouldPauseMusic) music[activeLayer].Pause();
		
		transition.clip = clipToPlay;
		transition.loop = false;
		transition.Play();
		yield return new WaitForSeconds(clipToPlay.length);
		
		if (shouldResumeMusic) music[activeLayer].Play();
	}
	
	public void SwitchToAction()
	{
		music[0].time = music[activeLayer].time;
		activeLayer = 0;
		inactiveLayer = 1;
		
		music[activeLayer].volume = 1.0f;
		music[activeLayer].Play();
		
		music[inactiveLayer].volume = 0.0f;
		music[inactiveLayer].Pause();
	}
	
	#region FadeIn
	public void FadeIn(float fadeDuration)
	{
		StartCoroutine(PerformFade(FadeInDuring, FadeInCompleted, fadeDuration));
	}
	
	private void FadeInDuring(float fadeValue)
	{
		music[activeLayer].volume = fadeValue;
	}
	
	private void FadeInCompleted()
	{
		//music[activeLayer].Pause();
	}	
	#endregion
	
	#region FadeOut
	public void FadeOut(float fadeDuration)
	{
		StartCoroutine(PerformFade(FadeOutDuring, FadeOutCompleted, fadeDuration));
	}
	
	private void FadeOutDuring(float fadeValue)
	{
		music[activeLayer].volume = 1.0f - fadeValue;
	}
	
	private void FadeOutCompleted()
	{
		music[activeLayer].Pause();
	}	
	#endregion
	
	#region CrossFade
	public void CrossFade(float fadeDuration)
	{
		if (isCrossFading) return; // Don't try to cross fade if we are already cross fading.
		isCrossFading = true;
		
		music[inactiveLayer].volume = 0.0f;
		music[inactiveLayer].time = music[activeLayer].time;
		music[inactiveLayer].Play();
		
		StartCoroutine(PerformFade(CrossFadeDuring, CrossFadeCompleted, fadeDuration));
	}
	
	private void CrossFadeDuring(float fadeValue)
	{
		music[activeLayer].volume = 1.0f - fadeValue;
		music[inactiveLayer].volume = fadeValue;
	}
	
	private void CrossFadeCompleted()
	{
		int temp = activeLayer;
		activeLayer = inactiveLayer;
		inactiveLayer = temp;
		
		music[inactiveLayer].Pause();
		isCrossFading = false;
	}
	#endregion
	
	public IEnumerator PerformFade(System.Action<float> fadeMethod, System.Action onComplete, float fadeDuration)
	{
		float fadeProgress = 0.0f;
		
		if (fadeDuration > 0.0f)
		{
			fadeMethod(fadeProgress);

			while (fadeProgress < 1.0f)
			{
				fadeProgress = Mathf.Clamp(fadeProgress + Time.deltaTime / fadeDuration, 0.0f, 1.0f);
				fadeMethod(fadeProgress);
				yield return null;
			}
		}
		else if (fadeDuration == 0.0f)
		{
			// If the fade value is 0 seconds, then skip the the end of the fade with a fadeProgress of 1.0f.
			fadeProgress = 1.0f;
			fadeMethod(fadeProgress); 
		}
		else
		{
			Debug.LogError("The fade duration can not be negative.");
		}
		
		onComplete();
	}
}
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
	

	
	// Start is called before the first frame update
    void Start()
    {
        //AudioMixer audioMixer = Resources.Load("AudioMixer") as AudioMixer;
		//AudioMixerGroup[] musicGroups = audioMixer.FindMatchingGroups("Master/Music");
		
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
		//transition.outputAudioMixerGroup = musicGroups[0];
		transition.outputAudioMixerGroup = musicOutputGroup;
		transition.clip = intro;
		transition.loop = false;
		
		StartCoroutine(playTransition(intro, true));
    }
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.M)) StartCoroutine(playTransition(outro, false));
	}
	
	public void PlayIntro()
	{
		StartCoroutine(playTransition(intro, true));
	}

	public IEnumerator playTransition(AudioClip clipToPlay, bool shouldResumeMusic)
	{
		music[activeLayer].Pause();
		
		transition.clip = clipToPlay;
		transition.loop = false;
		transition.Play();
		yield return new WaitForSeconds(clipToPlay.length);
		
		if (shouldResumeMusic) music[activeLayer].Play();
	}
	/*
	
	public IEnumerator musicTransition(AudioClip clipToPlay, bool switchToCalm)
	{
		//music.Pause();
		if (switchToCalm) SwitchToCalm();
		else SwitchToAction();
		
		transition.clip = clipToPlay;
		transition.loop = false;
		transition.Play();
		yield return new WaitForSeconds(clipToPlay.length);
	}
	*/
	
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

	public IEnumerator CrossFade(float fadeDuration)
	{	
		if (isCrossFading) yield break; // Don't try to cross fade if we are already cross fading.
		
		isCrossFading = true;
		float fadeValue = 0.0f;
		
		music[inactiveLayer].volume = 0.0f;
		music[inactiveLayer].time = music[activeLayer].time;
		music[inactiveLayer].Play();
		
		while (fadeValue < 1.0f)
		{
			fadeValue = Mathf.Clamp(fadeValue + Time.deltaTime / fadeDuration, 0.0f, 1.0f);
			music[activeLayer].volume = 1.0f - fadeValue;
			music[inactiveLayer].volume = fadeValue;
			
			yield return null;
		}
		
		int temp = activeLayer;
		activeLayer = inactiveLayer;
		inactiveLayer = temp;
		
		music[inactiveLayer].Pause();
		isCrossFading = false;
	}
}
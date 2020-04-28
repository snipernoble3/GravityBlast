using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup musicOutputGroup;
	
	private AudioSource music;
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
		
		music = gameObject.AddComponent<AudioSource>();
		//music.outputAudioMixerGroup = musicGroups[0];
		music.outputAudioMixerGroup = musicOutputGroup;
		music.clip = mainAction;
		music.loop = true;
		
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
		
		if (Input.GetKeyDown(KeyCode.Z)) StartCoroutine(musicTransition(outro, true));
		
		if (Input.GetKeyDown(KeyCode.X)) StartCoroutine(musicTransition(intro, false));
		//if (Input.GetKeyDown(KeyCode.Z)) SwitchLoop();
	}
	
	private void SwitchToCalm()
	{
		float playBackTime = music.time;
		music.clip = mainCalm;
		music.time = playBackTime;
		music.Play();
	}
	
	private void SwitchToAction()
	{
		float playBackTime = music.time;
		music.clip = mainAction;
		music.time = playBackTime;
		music.Play();
	}
	
	private void SwitchLoop()
	{
		float playBackTime = music.time;
		
		if (music.clip == mainAction)
		{
			music.clip = mainCalm;
			music.time = playBackTime;
			music.Play();
		}			
		else if (music.clip == mainCalm)
		{
			music.clip = mainAction;
			music.time = playBackTime;
			music.Play();
		}
	}
	
	public void PlayIntro()
	{
		StartCoroutine(playTransition(intro, true));
	}

	public IEnumerator playTransition(AudioClip clipToPlay, bool shouldResumeMusic)
	{
		music.Pause();
		
		transition.clip = clipToPlay;
		transition.loop = false;
		transition.Play();
		yield return new WaitForSeconds(clipToPlay.length);
		
		if (shouldResumeMusic) music.Play();
	}
	
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
}

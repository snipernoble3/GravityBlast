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
		//if (Input.GetKeyDown(KeyCode.B)) StartCoroutine(playTransition(jetpack, false));
		//if (Input.GetKeyDown(KeyCode.Z)) StartCoroutine(playTransition(gameOver, false));
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
}

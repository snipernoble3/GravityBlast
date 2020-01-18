using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    Scene scene;
	int numOfScenes;
	int levelToLoad;
	
	void Start()
	{
		numOfScenes = SceneManager.sceneCountInBuildSettings - 1; // Offset so that we can ask for the scene by index more easily.
		scene = SceneManager.GetActiveScene();
		levelToLoad = scene.buildIndex;
	}
	
	void Update()
    {
		if (Input.GetButtonDown("Restart")) SceneManager.LoadScene(scene.buildIndex);        

		if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus)) if (levelToLoad + 1 <= numOfScenes)	levelToLoad++;
		if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Underscore)) if (levelToLoad - 1 >= 0)	levelToLoad--;
		
		if (levelToLoad != scene.buildIndex) SceneManager.LoadScene(levelToLoad);
    }
}

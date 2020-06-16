using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    Scene scene;
	int numOfScenes;
	int levelToLoad;
	
	private void Update ()
	{
		//if (Input.GetButtonDown("Restart")) Restart(); //"Restart" is currently bound to J
		if (Input.GetButtonDown("Restart")) SceneManager.LoadScene(0);
	}
	
	public RestartLevel () {
		numOfScenes = SceneManager.sceneCountInBuildSettings - 1; // Offset so that we can ask for the scene by index more easily.
		scene = SceneManager.GetActiveScene();
		levelToLoad = scene.buildIndex;
	}

    public void Restart () {
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void NextScene () {
        if (levelToLoad + 1 <= numOfScenes) levelToLoad++;
        if (levelToLoad != scene.buildIndex) SceneManager.LoadScene(levelToLoad);
    }

    public void PreviousScene () {
        if (levelToLoad - 1 >= 0) levelToLoad--;
        if (levelToLoad != scene.buildIndex) SceneManager.LoadScene(levelToLoad);
    }

}

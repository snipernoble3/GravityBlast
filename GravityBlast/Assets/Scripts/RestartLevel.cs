using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    Scene scene;
	int numOfScenes;
	int levelToLoad;
	
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

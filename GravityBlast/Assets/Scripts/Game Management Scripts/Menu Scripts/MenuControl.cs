﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour {

    //private static bool created = false;

    [SerializeField] bool canPause;
    [SerializeField] GameObject pauseMenu;

    bool isPaused;

    [SerializeField] God g;

    private void Awake () {
        if (pauseMenu != null) pauseMenu.SetActive(isPaused);
    }

    // MAIN MENU //

    public void Play () {
        SceneManager.LoadScene("PlanetGeneration");
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Locked, false);
    }

    public void Collection () {
        SceneManager.LoadScene("TinyPlanet_Overload");
    }

    public void Settings () {
        
    }

    public void FeedbackReport () {
        Application.OpenURL("https://forms.gle/z5MjDE4gTevbRuhi7");
    }

    public void Quit () {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
    

    // PAUSE MENU //

    public void Pause () {
        if (!canPause) { return; }
        isPaused = true;
        //open pause menu
        g.PauseGame(isPaused);
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Confined, true);
        pauseMenu.SetActive(isPaused);
    }

    public void Resume () {
        isPaused = false;
        //close pause menu, reenable controls and gameplay
        pauseMenu.SetActive(isPaused);
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Locked, false);
        g.PauseGame(isPaused);
    }

    //Settings

    //Restart? idk if we want/need this

    public void ReturnToMenu () {
        SceneManager.LoadScene("Menu");
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Confined, true);
    }

    //Quit

}

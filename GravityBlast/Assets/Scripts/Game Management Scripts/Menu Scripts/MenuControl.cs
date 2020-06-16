using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour {

    //private static bool created = false;

    [SerializeField] bool canPause;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;

    bool isPaused;
    public bool settingsOpen { get; private set; } = false;

    private void Awake () {
        if (pauseMenu != null) pauseMenu.SetActive(isPaused);
    }

    // MAIN MENU //

    public void Play () {
        SceneManager.LoadScene("PlanetGeneration");
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Confined, true);
    }

    public void Collection () {
        
    }

    public void Settings () {
        settingsOpen = !settingsOpen;
        settingsMenu.SetActive(settingsOpen);
    }

    public void FeedbackReport () {
        Application.OpenURL("https://forms.gle/z5MjDE4gTevbRuhi7");
    }

    public void BugReport () {
        Application.OpenURL("https://forms.gle/au1x5r7nA284mdFA7");
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
        GameManager.gm.PauseGame(isPaused);
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Confined, true);
        pauseMenu.SetActive(isPaused);
    }

    public void Resume () {
        isPaused = false;
        //close pause menu, reenable controls and gameplay
        pauseMenu.SetActive(isPaused);
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Locked, false);
        GameManager.gm.PauseGame(isPaused);
    }

    //Settings

    //Restart? idk if we want/need this

    public void ReturnToMenu () {
        Resume();
        SceneManager.LoadScene("Menu");
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Confined, true);
    }

    //Quit

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour {

    //private static bool created = false;

    [SerializeField] bool canPause;

    bool paused;

    [SerializeField] God g;

    private void Awake () {
        
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

    public void Quit () {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
    

    // PAUSE MENU //

    public void Pause () {
        if (!canPause) { return; }
        paused = true;
        //open pause menu
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Confined, true);
        g.PauseGameElements(paused);
    }

    public void Resume () {
        paused = false;
        //close pause menu
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Locked, false);
        g.PauseGameElements(paused);
    }

    //Settings

    //Restart? idk if we want/need this

    public void ReturnToMenu () {
        //load menu
        gameObject.GetComponent<CursorLock>().SetCursor(CursorLockMode.Confined, true);
        //
    }

    //Quit

}

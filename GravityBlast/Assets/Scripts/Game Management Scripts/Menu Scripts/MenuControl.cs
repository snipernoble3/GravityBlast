using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour {
    
    public bool paused;

    // MAIN MENU //

    public void Play () {
        SceneManager.LoadScene("Test_Planets");
    }

    public void Collection () {

    }

    public void Settings () {

    }

    public void Quit () {
        Debug.Log("Quitting Game");
        Application.Quit();
    }


    // PAUSE MENU //

    public void Pause () {
        //open pause menu
        //timescale 0
    }

    public void Resume () {
        //close pause menu
        //timescale 1
    }

    //Settings

    public void Restart () {

    }

    public void ReturnToMenu () {

    }

    //Quit

}

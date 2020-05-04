using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetTimer : MonoBehaviour {

    
    float totalTime = 0f;
    float planetTime = 0f;

    bool paused;

    private void Update () {
        if (!paused) {
            planetTime += Time.deltaTime;
            totalTime += Time.deltaTime;
        }
    }

    public void pauseTimer (bool isPaused) {
        paused = isPaused;
    }

    public void resetPlanetTime () {
        planetTime = 0;
    }

    public string getPlanetTime () {
        return convertTimeToString(planetTime);
    }

    public string getTotalTime () {
        return convertTimeToString(totalTime);
    }

    private string convertTimeToString (float time) {
        int t = (int)time;
        string s = "";
        int hours = t / 3600;
        s = (hours > 9) ? s + hours : s + ("0" + hours);
        s = s + ":";
        if (s == "00:") s = "";
        t %= 3600;
        int minutes = t / 60;
        s = (minutes > 9) ? s + minutes : s + ("0" + minutes);
        s = s + ":";
        t %= 60;
        s = (t > 9) ? s + t : s + ("0" + t);
        return s;
    }

}

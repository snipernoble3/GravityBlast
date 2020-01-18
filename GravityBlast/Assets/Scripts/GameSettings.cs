using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game {

    //Faction Tags
    public enum Faction { Player, Wild, Neutral};

    //Planet Tags
    public enum Classification { Mountain, Plains, Arctic, Blended};

    //Planet Struct
    public struct Planet {
        float size;
        float gravity;
        Classification planetClassification;
    }

    //Planet Generator


    //Planet Tracker
    public Planet[] system;

    //Stage Tracker
    public int stage;



}

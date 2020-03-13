using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {

    // PLANET INFO //
    God.PlanetInfo planet;
    [SerializeField] GameObject killBounds;

    // ENEMIES //
    //enemies to spawn
    //enemy pool

    public void LoadPlanet () { //called by game manager
        //set size
        transform.localScale = planet.size * Vector3.one;
        //create kill boundaries

        //scale kill boundaries - 70 scale

        //spawn environment
        //spawn enemies set as inactive
        //place enemies based on planet details
        //spawn moon if required
    }

    public void DestroyPlanet () {
        //destruction animation?
        //remove enemies
    }

    private void PopulateEnvironment () {

    }

    private void PopulateEnemies () {

    }

    private void GenerateMoon () {

    }
    
    public void SetInfo (God.PlanetInfo p) {
        planet = p;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetInfo {

    //GameObject prefab; //planet model
    public int planetPrefab { get; private set; }

    public int stageNumber { get; private set; }
    public int xpToAdvance { get; private set; }

    public float size { get; private set; }
    public string sizeCategory { get; private set; } // small - medium - large
    public float lowestSurfacePoint { get; private set; }
    public float highestSurfacePoint { get; private set; }
    
    public int moons { get; private set; }

    //public bool hasBoss { get; private set; }

    public PlanetInfo (int _planetPrefab, int _stageNumber, int _xpToAdvance, float _size, string _sizeCategory, float _lowestSurfacePoint, float _highestSurfacePoint, int _moons) {
        planetPrefab = _planetPrefab;
        stageNumber = _stageNumber;
        xpToAdvance = _xpToAdvance;
        size = _size;
        sizeCategory = _sizeCategory;
        lowestSurfacePoint = _lowestSurfacePoint;
        highestSurfacePoint = _highestSurfacePoint;
        moons = _moons;
    }

    public void LastPlanet () { //can be used to set specific details for the last planet
        moons++;
    }

}

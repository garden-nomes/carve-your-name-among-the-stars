using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlanetType
{
    GasGiant,
    RockyPlanet,
    GardenWorld
}

public class PlanetManager : MonoBehaviour
{
    private List<PlanetInfo> _planets = new List<PlanetInfo>();
    public List<PlanetInfo> planets => _planets;

    public System.Action<PlanetInfo> onAddPlanet;

    // singleton implementation
    private static PlanetManager _instance;
    public static PlanetManager instance => _instance;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            // persist between title/game scenes
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            GameObject.Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    public void OnDestroy()
    {
        // because I have the fast play mode settings enabled, we need to manually reset static instances
        if (_instance == this)
        {
            _instance = null;
        }
    }
#endif

    public void AddPlanet(PlanetInfo planet)
    {
        _planets.Add(planet);
        onAddPlanet?.Invoke(planet);
    }
}

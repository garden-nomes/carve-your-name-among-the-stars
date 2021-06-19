using System.Collections;
using UnityEngine;

public enum PlanetClass
{
    GasGiant,
    RockyPlanet,
    GardenWorld
}

public class PlanetInfo : MonoBehaviour
{
    public PlanetClass planetClass;
    public MarkovNameGeneratorScriptableObject nameGenerator;

    [HideInInspector] public bool isScanned = false;
    [HideInInspector] public string planetName;

    void Start()
    {
        planetName = nameGenerator.Generate();
        isScanned = false;
    }
}

using UnityEngine;

public struct PlanetInfo
{
    public Vector3 position;
    public PlanetType type;
    public string name;
    public bool isScanned;

    public PlanetInfo(Vector3 position, PlanetType type)
    {
        this.position = position;
        this.type = type;

        name = null;
        isScanned = false;
    }
}

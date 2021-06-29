using UnityEngine;

public class PlanetInfo
{
    public Vector3 position;
    public PlanetType type;
    public string name = "name";
    public bool isScanned = false;
    public float radius => 0.5f;

    public PlanetInfo(Vector3 position, PlanetType type)
    {
        this.position = position;
        this.type = type;
    }

    public float RayIntersect(Vector3 origin, Vector3 direction)
    {
        var toPlanet = position - origin;

        float toClosestPoint = Vector3.Dot(toPlanet, direction);
        if (toClosestPoint < 0f)
        {
            return -1f;
        }

        float closestPointToPlanetSq = (toClosestPoint * toClosestPoint) - toPlanet.sqrMagnitude;
        if (closestPointToPlanetSq > radius * radius)
        {
            return -1f;
        }

        return toClosestPoint - Mathf.Sqrt(radius * radius - closestPointToPlanetSq);
    }
}

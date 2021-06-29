using UnityEngine;

public class PlanetInfo
{
    public static MarkovNameGeneratorScriptableObject nameGenerator;

    public Vector3 position;
    public PlanetType type;
    public bool isScanned = false;
    public float radius => 0.5f;

    private string _name = null;
    public string name
    {
        get
        {
            if (_name == null && nameGenerator == null)
            {
                Debug.LogWarning("nameGenerator is null");
                return "name";
            }

            return _name ?? (_name = nameGenerator.Generate());
        }
    }

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

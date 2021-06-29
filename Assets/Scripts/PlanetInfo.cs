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
        var toOrigin = origin - position;

        float toClosestPoint = Vector3.Dot(toOrigin, direction);
        float closestPointToSphereRadiusSq = toOrigin.sqrMagnitude - radius * radius;
        float closestPointToIntersect = toClosestPoint * toClosestPoint - closestPointToSphereRadiusSq;

        if (closestPointToIntersect < 0f)
        {
            return -1f;
        }

        return -toClosestPoint - Mathf.Sqrt(closestPointToIntersect);
    }
}

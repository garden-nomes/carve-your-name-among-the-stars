using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSelector : MonoBehaviour
{
    public GameObject spaceship;
    public string planetTag;
    public PlanetIndicator indicator;
    public float minimumDistance = 2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (indicator == null || spaceship == null)
        {
            return;
        }

        var planets = GameObject.FindGameObjectsWithTag(planetTag);

        GameObject bestPlanet = null;
        float bestDot = -1f;

        foreach (var planet in planets)
        {
            var toPlanet = planet.transform.position - spaceship.transform.position;
            float dot = Vector3.Dot(toPlanet.normalized, spaceship.transform.forward);

            if (dot > bestDot)
            {
                bestDot = dot;
                bestPlanet = planet;
            }
        }

        var toBestPlanet = bestPlanet.transform.position - spaceship.transform.position;
        if (toBestPlanet.sqrMagnitude < minimumDistance * minimumDistance)
            indicator.planet = null;
        else
            indicator.planet = bestPlanet;

    }
}

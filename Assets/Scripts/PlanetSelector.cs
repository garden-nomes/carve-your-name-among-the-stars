using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSelector : MonoBehaviour
{
    public string planetTag;
    public PlanetIndicator indicator;
    public float minimumDistance = 2f;
    public float maximumDistance = 200f;

    private void Update()
    {
        var planets = GameObject.FindGameObjectsWithTag(planetTag);

        GameObject bestPlanet = null;
        float bestDot = -1f;

        foreach (var planet in planets)
        {
            var toPlanet = planet.transform.position - transform.position;

            if (toPlanet.sqrMagnitude > maximumDistance * maximumDistance)
            {
                continue;
            }

            float dot = Vector3.Dot(toPlanet.normalized, transform.forward);

            if (dot > bestDot)
            {
                bestDot = dot;
                bestPlanet = planet;
            }
        }

        if (bestPlanet == null)
        {
            indicator.planet = null;
        }
        else
        {
            var toBestPlanet = bestPlanet.transform.position - transform.position;
            if (toBestPlanet.sqrMagnitude < minimumDistance * minimumDistance)
                indicator.planet = null;
            else
                indicator.planet = bestPlanet.GetComponent<PlanetInfo>();
        }
    }
}

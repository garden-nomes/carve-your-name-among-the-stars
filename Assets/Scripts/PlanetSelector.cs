using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetSelector : MonoBehaviour
{
    public string planetTag;
    public PlanetIndicator indicator;
    public float minimumDistance = 2f;
    public float radius = 100f;

    private void Update()
    {
        var inRange = PlanetManager.instance.WithinRadius(transform.position, radius);

        // select the planet closest to the center of the screen using the dot value
        var selectedPlanet = inRange
            .OrderByDescending(planet =>
            {
                var toPlanet = (planet.position - transform.position).normalized;
                return Vector3.Dot(toPlanet, transform.forward);
            })
            .FirstOrDefault();

        if (selectedPlanet == null)
        {
            // planet generation is still in progress, so the KDTree for planets hasn't been built yet
            indicator.planet = null;
            return;
        }

        var toSelected = selectedPlanet.position - transform.position;

        if (toSelected.sqrMagnitude < minimumDistance * minimumDistance)
        {
            indicator.planet = null;
            return;
        }

        var closest = PlanetManager.instance.ClosestPlanet(transform.position);

        bool isBehindOtherPlanet = closest != null &&
            closest.RayIntersect(transform.position, transform.forward) > 0f;

        if (isBehindOtherPlanet)
        {
            indicator.planet = null;
        }
        else
        {
            indicator.planet = selectedPlanet;
        }
    }
}

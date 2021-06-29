using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetSelector : MonoBehaviour
{
    public string planetTag;
    public PlanetIndicator indicator;
    public float minimumDistance = 2f;
    public float maximumDistance = 200f;

    private void Update()
    {
        var inRange = PlanetManager.instance.KNearestPlanets(transform.position, 20);

        var selectedPlanet = inRange
            .OrderByDescending(planet =>
            {
                var toPlanet = (planet.position - transform.position).normalized;
                return Vector3.Dot(toPlanet, transform.forward);
            })
            .First();

        var toSelected = selectedPlanet.position - transform.position;

        if (toSelected.sqrMagnitude < minimumDistance * minimumDistance)
        {
            indicator.planet = null;
            return;
        }

        var closest = PlanetManager.instance.ClosestPlanet(transform.position);
        bool isBehindOtherPlanet = closest.RayIntersect(transform.position, transform.forward) < 0f;

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

using System.Collections;
using System.Collections.Generic;
using Unity.PixelText;
using UnityEngine;

public class PlanetIndicator : MonoBehaviour
{
    public PlanetInfo planet;
    public GameObject spaceship;

    public PixelText nameLabel;
    public PixelText distanceLabel;
    public PixelText typeLabel;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (planet == null)
        {
            // move offscreen
            rectTransform.anchoredPosition = new Vector2(0f, -50f);
            return;
        }

        rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(planet.transform.position);

        float distance = (spaceship.transform.position - planet.transform.position).magnitude;
        distanceLabel.text = $"{distance.ToString("F1")} KM";

        nameLabel.text = planet.planetName.ToUpper();

        switch (planet.planetClass)
        {
            case PlanetClass.GardenWorld:
                typeLabel.text = "GARDEN WORLD";
                break;
            case PlanetClass.GasGiant:
                typeLabel.text = "GAS GIANT";
                break;
            case PlanetClass.RockyPlanet:
                typeLabel.text = "ROCKY PLANET";
                break;
        }
    }
}

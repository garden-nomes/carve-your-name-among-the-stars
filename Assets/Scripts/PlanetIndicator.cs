using Unity.PixelText;
using UnityEngine;

public class PlanetIndicator : MonoBehaviour
{
    public AudioClip tick;
    public GameObject spaceship;
    public PixelText nameLabel;
    public PixelText distanceLabel;
    public PixelText typeLabel;

    private RectTransform rectTransform;

    private PlanetInfo _planet;
    public PlanetInfo planet
    {
        get => _planet;
        set
        {
            if (value != null && value != _planet)
            {
                AudioSource.PlayClipAtPoint(tick, Camera.main.transform.position, 0.2f);
            }

            _planet = value;
        }
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (_planet == null)
        {
            // move offscreen
            rectTransform.anchoredPosition = new Vector2(0f, -50f);
            return;
        }

        rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(_planet.transform.position);

        float distance = (spaceship.transform.position - _planet.transform.position).magnitude;
        distanceLabel.text = $"{distance.ToString("F1")} KM";

        if (_planet.isScanned)
        {
            nameLabel.text = $"{_planet.planetName.ToUpper()} (SCANNED)";
            nameLabel.color = typeLabel.color;
        }
        else
        {
            nameLabel.text = _planet.planetName.ToUpper();
            nameLabel.color = Color.white;
        }

        switch (_planet.planetClass)
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

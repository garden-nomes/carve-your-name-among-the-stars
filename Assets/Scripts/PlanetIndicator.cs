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
            if (value != null && (_planet == null || _planet.position != value.position))
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

        rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(_planet.position);

        float distance = (spaceship.transform.position - _planet.position).magnitude;
        distanceLabel.text = $"{distance.ToString("F1")} KM";

        if (_planet.isScanned)
        {
            nameLabel.text = $"{_planet.name} (SCANNED)";
            nameLabel.color = typeLabel.color;
        }
        else
        {
            nameLabel.text = _planet.name;
            nameLabel.color = Color.white;
        }

        switch (_planet.type)
        {
            case PlanetType.GardenWorld:
                typeLabel.text = "GARDEN WORLD";
                break;
            case PlanetType.GasGiant:
                typeLabel.text = "GAS GIANT";
                break;
            case PlanetType.RockyPlanet:
                typeLabel.text = "ROCKY PLANET";
                break;
        }
    }
}

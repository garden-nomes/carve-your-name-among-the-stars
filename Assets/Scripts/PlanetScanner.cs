using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpaceshipController))]
public class PlanetScanner : MonoBehaviour
{
    public AudioClip scanSound;
    public float scanSoundLoop;
    public System.Action<PlanetInfo> onComplete;
    public string planetTag = "Planet";
    public float scanRadius = 5f;
    public float scanTime = 3f;
    public OffscreenPointer planetPointer;
    public ScannerUI scannerUI;
    public TextController textController;
    public PlanetDescriptions planetDescriptions;

    private SpaceshipController spaceshipController;
    private FuelTank fuelTank;
    private PlanetInfo? planet;
    private bool wasMoving = true;
    private PlanetEncounterSequencer encounterSequencer;
    private float scanSoundTimer = 0f;

    void Start()
    {
        spaceshipController = GetComponent<SpaceshipController>();
        fuelTank = GetComponent<FuelTank>();
        encounterSequencer = new PlanetEncounterSequencer(planetDescriptions);
    }

    void Update()
    {
        // check for planets in scan radius
        planet = null;
        var colliders = Physics.OverlapSphere(transform.position, scanRadius);
        foreach (var collider in colliders)
        {
            // NOTE: we don't expect to have more than one planet in the scan radius given how planets are spaced
            planet = collider.GetComponent<PlanetInfo>();
            if (planet != null) break;
        }

        // show arrow towards planet
        // planetPointer.target = planet == null || planet.Value.isScanned ? null : planet.Value.position;

        // check if planet is onscreen
        if (planet is PlanetInfo planet_)
        {
            var viewportPosition = Camera.main.WorldToViewportPoint(planet_.position);
            var viewportRect = new Rect(Vector2.zero, Vector2.one);
            bool isPlanetOnscreen = viewportPosition.z > 0f && viewportRect.Contains(viewportPosition);

            // scan if spaceship isn't moving
            if (spaceshipController.speed == 0f &&
                isPlanetOnscreen &&
                !planet_.isScanned &&
                !fuelTank.isEmpty)
            {
                // play noise at regular intervals
                scanSoundTimer -= Time.deltaTime;
                if (scanSoundTimer <= 0f)
                {
                    AudioSource.PlayClipAtPoint(scanSound, transform.position);
                    scanSoundTimer = scanSoundLoop;
                }

                // show scanner UI
                scannerUI.isVisible = true;

                // use progress bar as timer
                if (scannerUI.progress < 1f)
                {
                    scannerUI.progress += Time.deltaTime / scanTime;

                    if (scannerUI.progress >= 1f)
                    {
                        // scan complete
                        onComplete?.Invoke(planet_);
                        ShowDescription(planet_);
                        planet_.isScanned = true;
                        scannerUI.progress = 1f;
                    }
                }
            }
            else
            {
                scannerUI.isVisible = false;
                scannerUI.progress = 0f;
            }
        }
    }

    private void ShowDescription(PlanetInfo planet)
    {
        StartCoroutine(ShowDescriptionCoroutine(planet));
    }

    private IEnumerator ShowDescriptionCoroutine(PlanetInfo planet)
    {
        // show encounter text
        string encounter = encounterSequencer.Next(planet.type);
        yield return textController.ShowText(encounter);

        // if gas giant, trigger refueling sequence
        if (planet.type == PlanetType.GasGiant)
        {
            yield return GetComponent<FuelTank>().RefuelCoroutine();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Test Garden World Encounters")]
    private void TestGardenWorldEncounters()
    {
        StartCoroutine(TestEncountersCoroutine(planetDescriptions.gardenWorldEvents));
    }

    [ContextMenu("Test Rocky Planet Encounters")]
    private void TestRockyPlanetEncounters()
    {
        StartCoroutine(TestEncountersCoroutine(planetDescriptions.rockyPlanetEvents));
    }

    private IEnumerator TestEncountersCoroutine(string[] encounters)
    {
        foreach (var encounter in encounters)
        {
            yield return textController.ShowText(encounter);
        }
    }
#endif
}

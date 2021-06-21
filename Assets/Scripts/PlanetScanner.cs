using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpaceshipController))]
public class PlanetScanner : MonoBehaviour
{
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
    private PlanetInfo planet;
    private bool wasMoving = true;
    private PlanetEncounterSequencer encounterSequencer;
    private bool wasMainStoryComplete = false;

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
        planetPointer.target = planet == null ? null : planet.transform;

        // check if planet is onscreen
        bool isPlanetOnscreen =
            planet != null &&
            new Rect(Vector2.zero, Vector2.one).Contains(Camera.main.WorldToViewportPoint(planet.transform.position));

        // scan if spaceship isn't moving
        if (spaceshipController.Velocity.sqrMagnitude == 0f &&
            planet != null &&
            isPlanetOnscreen &&
            !planet.isScanned &&
            !fuelTank.isEmpty)
        {
            // show scanner UI
            scannerUI.isVisible = true;

            // use progress bar as timer
            if (scannerUI.progress < 1f)
            {
                scannerUI.progress += Time.deltaTime / scanTime;

                if (scannerUI.progress >= 1f)
                {
                    // scan complete
                    onComplete?.Invoke(planet);
                    ShowDescription(planet);
                    planet.isScanned = true;
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

    private void ShowDescription(PlanetInfo planet)
    {
        StartCoroutine(ShowDescriptionCoroutine(planet));
    }

    private IEnumerator ShowDescriptionCoroutine(PlanetInfo planet)
    {
        // show encounter text
        string encounter = encounterSequencer.Next(planet.planetClass);
        yield return textController.ShowText(encounter);

        // if gas giant, trigger refueling sequence
        if (planet.planetClass == PlanetClass.GasGiant)
        {
            yield return GetComponent<FuelTank>().RefuelCoroutine();
        }

        // if we just completed the main story, show the "end" title
        if (encounterSequencer.isMainStoryComplete && !wasMainStoryComplete)
        {
            wasMainStoryComplete = true;
            GameManager.instance.EndGame(true);
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

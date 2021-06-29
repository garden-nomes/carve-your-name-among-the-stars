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

    private bool _isScanning = false;
    public bool isScanning => isScanning;

    private SpaceshipController spaceshipController;
    private FuelTank fuelTank;
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
        // check for a planet in the scan radius
        var closestPlanet = PlanetManager.instance.ClosestPlanet(transform.position);

        // check that the planet meets the conditions to show an arrow towards it
        //   - in range
        //   - not already scanned
        if (closestPlanet == null ||
            closestPlanet.isScanned ||
            (closestPlanet.position - transform.position).sqrMagnitude > scanRadius * scanRadius)
        {
            planetPointer.target = null;
            scannerUI.isVisible = false;
            scannerUI.progress = 0f;
            _isScanning = false;
            return;
        }

        // show arrow towards planet if offscreen
        planetPointer.target = closestPlanet.position;

        // check if we meet the conditions to scan the planet:
        //   - looking directly at it
        //   - not moving
        bool isLookingAtPlanet = closestPlanet.RayIntersect(transform.position, transform.forward) >= 0f;
        bool isMoving = spaceshipController.speed > 0f;

        if (!isLookingAtPlanet || isMoving)
        {
            scannerUI.isVisible = false;
            scannerUI.progress = 0f;
            _isScanning = false;
            return;
        }

        _isScanning = true;

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
                onComplete?.Invoke(closestPlanet);
                ShowEncounter(closestPlanet);
                closestPlanet.isScanned = true;
                scannerUI.progress = 1f;
            }
        }

    }

    private void ShowEncounter(PlanetInfo planet)
    {
        StartCoroutine(ShowEncounterCoroutine(planet));
    }

    private IEnumerator ShowEncounterCoroutine(PlanetInfo planet)
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

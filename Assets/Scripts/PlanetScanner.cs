using System.Collections;
using Unity.PixelText;
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

    public PixelText activePlanetName;
    public PixelText scannerStatus;
    public string playerMovingMessage;
    public string isScannedMessage;
    public string inRangeMessage;

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
        var activePlanet = PlanetManager.instance.ClosestPlanet(transform.position);

        if (activePlanet == null)
        {
            // planet generation is still in progress, so the KDTree for planets hasn't been built yet
            scannerUI.isVisible = false;
            scannerUI.progress = 0f;
            planetPointer.target = null;
            activePlanetName.text = "";
            scannerStatus.text = "";
            return;
        }

        // check for conditions that decide what to do
        bool isInRange = (activePlanet.position - transform.position).sqrMagnitude <= scanRadius * scanRadius;
        bool isLookingAtPlanet = activePlanet.RayIntersect(transform.position, transform.forward) >= 0f;
        bool isMoving = spaceshipController.speed > 0f;

        // show planet name and scanner status at the top of the screen
        if (isInRange)
        {
            activePlanetName.text = activePlanet.name;

            // set status text
            if (activePlanet.isScanned)
            {
                scannerStatus.text = isScannedMessage;
            }
            else if (isMoving)
            {
                scannerStatus.text = playerMovingMessage;
            }
            else if (!isLookingAtPlanet)
            {
                scannerStatus.text = inRangeMessage;
            }
            else
            {
                scannerStatus.text = inRangeMessage;
            }
        }
        else
        {
            activePlanetName.text = "";
            scannerStatus.text = "";
        }

        // add an offscreen arrow indicator towards the planet
        if (isInRange && !activePlanet.isScanned)
        {
            planetPointer.target = activePlanet.position;
        }
        else
        {
            planetPointer.target = null;
        }

        // run planet scan sequence
        if (!activePlanet.isScanned && isInRange && !isMoving && isLookingAtPlanet)
        {
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
                    onComplete?.Invoke(activePlanet);
                    ShowEncounter(activePlanet);
                    activePlanet.isScanned = true;
                    scannerUI.progress = 1f;
                }
            }
        }
        else
        {
            _isScanning = false;
            scannerUI.isVisible = false;
            scannerUI.progress = 0f;
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

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
    public ScannerUI scannerUI;
    public TextController textController;
    public PlanetDescriptions planetDescriptions;

    private SpaceshipController spaceshipController;
    private PlanetInfo planet;
    private bool wasMoving = true;

    void Start()
    {
        spaceshipController = GetComponent<SpaceshipController>();
    }

    void Update()
    {
        if (spaceshipController.Velocity.sqrMagnitude == 0f)
        {
            if (wasMoving)
            {
                // on stop
                planet = null;
                var colliders = Physics.OverlapSphere(transform.position, scanRadius);

                foreach (var collider in colliders)
                {
                    // NOTE: we don't expect to have more than one planet in the scan radius
                    planet = collider.GetComponent<PlanetInfo>();
                    if (planet != null) break;
                }
            }

            if (planet != null && !planet.isScanned)
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
        else
        {
            scannerUI.isVisible = false;
            scannerUI.progress = 0f;
            wasMoving = true;
        }
    }

    private void ShowDescription(PlanetInfo planet)
    {
        string evt = planetDescriptions.GetEncounter(planet.planetClass);
        textController.ShowText(evt.ToUpper());
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
            yield return textController.ShowText(encounter.ToUpper());
        }
    }
#endif
}

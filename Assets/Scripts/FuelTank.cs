using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpaceshipController))]
public class FuelTank : MonoBehaviour
{
    public AudioClip refuelingSound;
    public float refuelingSoundLoop = 0.5f;

    public float fuel = 1000f;
    public float fuelDistance = 300f; // how far the spaceship can travel on a full tank
    public float refuelTime = 5f;
    public ScannerUI refuelingUI;
    public bool isEmpty => fuel == 0f;

    public TextController textController;
    [TextArea] public string outOfFuelMessage = "";
    public float outOfFuelBeat = 1f;

    private SpaceshipController spaceshipController;

    private void Start()
    {
        spaceshipController = GetComponent<SpaceshipController>();
    }

    private void Update()
    {
        if (fuel > 0f)
        {
            // consume fuel
            fuel -= spaceshipController.speed * Time.deltaTime * 1000f / fuelDistance;

            if (fuel <= 0f)
            {
                fuel = 0f;
                StartCoroutine(OutOfFuelCoroutine());
            }
        }
    }

    public IEnumerator RefuelCoroutine()
    {
        refuelingUI.isVisible = true;
        spaceshipController.disableThrottle = true;

        float startingFuel = fuel;

        float soundTimer = 0f;

        while (fuel < 1000f)
        {
            soundTimer -= Time.deltaTime;
            if (soundTimer <= 0f)
            {
                AudioSource.PlayClipAtPoint(refuelingSound, transform.position);
                soundTimer = refuelingSoundLoop;
            }

            fuel += 1000f * Time.deltaTime / refuelTime;
            refuelingUI.progress = (fuel - startingFuel) / (1000f - startingFuel);
            yield return null;
        }

        fuel = 1000f;
        refuelingUI.isVisible = false;
        spaceshipController.disableThrottle = false;
    }

    private IEnumerator OutOfFuelCoroutine()
    {
        yield return new WaitForSeconds(outOfFuelBeat);
        yield return textController.ShowText(outOfFuelMessage);
        GameManager.instance.Restart();
    }
}

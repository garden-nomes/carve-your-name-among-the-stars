using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipHudController : MonoBehaviour
{
    public SpaceshipController spaceshipController;
    public FuelTank fuelTank;

    public float lowFuelWarning = 200f; // when to turn on the low fuel warning

    public Dial speedDial;
    public Dial fuelDial;
    public GameObject fuelLight;
    public GameObject inMotionLight;
    public GameObject fullStopLight;

    private void Update()
    {
        speedDial.value = Mathf.InverseLerp(
            -spaceshipController.maxSpeed,
            spaceshipController.maxSpeed,
            spaceshipController.speed);

        fuelDial.value = fuelTank.fuel / 1000f;

        inMotionLight.SetActive(spaceshipController.speed != 0f);
        fullStopLight.SetActive(spaceshipController.speed == 0f);
        fuelLight.SetActive(fuelTank.fuel > 0f && fuelTank.fuel <= lowFuelWarning);
    }
}

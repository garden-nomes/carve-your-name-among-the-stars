using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipHudController : MonoBehaviour
{
    public SpaceshipController spaceshipController;

    public Dial speedDial;
    public Dial fuelDial;
    public GameObject fuelLight;
    public GameObject inMotionLight;
    public GameObject fullStopLight;

    private void Update()
    {
        float speed = spaceshipController.Velocity.magnitude;

        speedDial.value = speed / spaceshipController.maxSpeed;
        fuelDial.value = 1f;

        inMotionLight.SetActive(speed > 0f);
        fullStopLight.SetActive(speed == 0f);
        fuelLight.SetActive(false);
    }
}

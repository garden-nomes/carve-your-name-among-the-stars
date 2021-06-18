using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipHudController : MonoBehaviour
{
    public SpaceshipController spaceshipController;

    public Dial speedDial;
    public Dial fuelDial;

    private void Update()
    {
        speedDial.value = spaceshipController.Velocity.magnitude / spaceshipController.maxSpeed;
        fuelDial.value = 1f;
    }
}

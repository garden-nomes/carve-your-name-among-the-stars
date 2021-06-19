using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Planet Descriptions")]
public class PlanetDescriptions : ScriptableObject
{
    [TextArea] public string[] gardenWorldEvents;
    [TextArea] public string[] rockyPlanetEvents;
    [TextArea] public string[] gasGiantEvents;

    private int gardenWorldEventIndex;
    private int rockyPlanetEventIndex;
    private int gasGiantEventIndex;

    private void OnEnable()
    {
        Shuffle(gardenWorldEvents);
        Shuffle(gasGiantEvents);
        gardenWorldEventIndex = 0;
        rockyPlanetEventIndex = 0;
        gasGiantEventIndex = 0;
    }

    public string GetEvent(PlanetClass planetClass)
    {
        string evt = "";

        if (planetClass == PlanetClass.RockyPlanet)
        {
            evt = rockyPlanetEvents[rockyPlanetEventIndex];
            if (rockyPlanetEventIndex < rockyPlanetEvents.Length - 1)
                rockyPlanetEventIndex++;
        }
        else if (planetClass == PlanetClass.GasGiant)
        {
            evt = gasGiantEvents[gasGiantEventIndex];

            gasGiantEventIndex++;
            if (gasGiantEventIndex >= gasGiantEvents.Length)
            {
                gasGiantEventIndex = 0;
                Shuffle(gasGiantEvents);
            }
        }
        else if (planetClass == PlanetClass.GardenWorld)
        {
            evt = gardenWorldEvents[gardenWorldEventIndex];

            gardenWorldEventIndex++;
            if (gardenWorldEventIndex >= gardenWorldEvents.Length)
            {
                gardenWorldEventIndex = 0;
                Shuffle(gardenWorldEvents);
            }
        }

        return evt;
    }

    private void Shuffle(string[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }
    }
}

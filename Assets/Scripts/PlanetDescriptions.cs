using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Planet Descriptions")]
public class PlanetDescriptions : ScriptableObject
{
    [TextArea(4, 80)] public string[] gardenWorldEvents;
    [TextArea(4, 80)] public string[] rockyPlanetEvents;
    [TextArea(4, 80)] public string[] gasGiantEvents;

    private int[] gardenWorldEventOrder;
    private int gardenWorldEventIndex;
    private int rockyPlanetEventIndex;
    private int[] gasGiantEventOrder;
    private int gasGiantEventIndex;

    private void OnEnable()
    {
        gardenWorldEventOrder = new int[gardenWorldEvents.Length];
        for (int i = 0; i < gardenWorldEvents.Length; i++)
            gardenWorldEventOrder[i] = i;
        Shuffle(gardenWorldEventOrder);

        gasGiantEventOrder = new int[gasGiantEvents.Length];
        for (int i = 0; i < gasGiantEvents.Length; i++)
            gasGiantEventOrder[i] = i;
        Shuffle(gasGiantEventOrder);

        gardenWorldEventIndex = 0;
        rockyPlanetEventIndex = 0;
        gasGiantEventIndex = 0;
    }

    public string GetEncounter(PlanetClass planetClass)
    {
        string text = "";

        if (planetClass == PlanetClass.RockyPlanet)
        {
            text = rockyPlanetEvents[rockyPlanetEventIndex];
            if (rockyPlanetEventIndex < rockyPlanetEvents.Length - 1)
                rockyPlanetEventIndex++;
        }
        else if (planetClass == PlanetClass.GasGiant)
        {
            text = gasGiantEvents[gasGiantEventOrder[gasGiantEventIndex]];

            gasGiantEventIndex++;
            if (gasGiantEventIndex >= gasGiantEvents.Length)
            {
                gasGiantEventIndex = 0;
                Shuffle(gasGiantEventOrder);
            }
        }
        else if (planetClass == PlanetClass.GardenWorld)
        {
            text = gardenWorldEvents[gardenWorldEventOrder[gardenWorldEventIndex]];

            gardenWorldEventIndex++;
            if (gardenWorldEventIndex >= gardenWorldEvents.Length)
            {
                gardenWorldEventIndex = 0;
                Shuffle(gardenWorldEventOrder);
            }
        }

        return text;
    }

    private void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }
    }
}

using UnityEngine;

// Note to self -- rename to PlanetEncounters when you have the brainspace
[CreateAssetMenu(menuName = "Planet Descriptions")]
public class PlanetDescriptions : ScriptableObject
{
    [TextArea(4, 80)] public string[] gardenWorldEvents;
    [TextArea(4, 80)] public string[] rockyPlanetEvents;
    [TextArea(4, 80)] public string[] gasGiantEvents;

#if UNITY_EDITOR
    [ContextMenu("Test Sequencer")]
    private void TestSequencer()
    {
        var sequence = new PlanetEncounterSequencer(this);

        for (int i = 0; i < 10; i++)
        {
            Debug.Log(sequence.Next(PlanetClass.GardenWorld));
        }
    }
#endif
}

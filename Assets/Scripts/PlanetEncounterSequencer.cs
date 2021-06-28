public class PlanetEncounterSequencer
{
    private readonly PlanetDescriptions encounters;

    private readonly RandomDeck<string> gasGiantDeck;
    private readonly RandomDeck<string> gardenWorldDeck;
    private int rockyPlanetIndex;

    public bool isMainStoryComplete => rockyPlanetIndex >= encounters.rockyPlanetEvents.Length;

    public PlanetEncounterSequencer(PlanetDescriptions encounters)
    {
        this.encounters = encounters;
        this.gasGiantDeck = new RandomDeck<string>(encounters.gasGiantEvents);
        this.gardenWorldDeck = new RandomDeck<string>(encounters.gardenWorldEvents);
        this.rockyPlanetIndex = 0;
    }

    public string Next(PlanetType planet)
    {
        switch (planet)
        {
            case PlanetType.GardenWorld:
                return gardenWorldDeck.Draw();
            case PlanetType.GasGiant:
                return gasGiantDeck.Draw();
            case PlanetType.RockyPlanet:
                if (rockyPlanetIndex < encounters.rockyPlanetEvents.Length)
                {
                    string encounter = encounters.rockyPlanetEvents[rockyPlanetIndex];
                    rockyPlanetIndex++;
                    return encounter;
                }
                else
                {
                    return encounters.rockyPlanetEvents[0];
                }
            default:
                throw new System.Exception("Unreachable code, unless we're missing a switch case");
        }

    }
}

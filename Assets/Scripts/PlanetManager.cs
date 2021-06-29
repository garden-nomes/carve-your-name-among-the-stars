using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataStructures.ViliWonka.KDTree;
using UnityEngine;

public enum PlanetType
{
    GasGiant,
    RockyPlanet,
    GardenWorld
}

public class PlanetManager : MonoBehaviour
{
    private List<PlanetInfo> _planets = new List<PlanetInfo>();
    public List<PlanetInfo> planets => _planets;

    public System.Action<PlanetInfo> onAddPlanet;

    private KDTree kdTree;
    private KDQuery kdQuery;
    private List<int> queryResults = new List<int>();

    // singleton implementation
    private static PlanetManager _instance;
    public static PlanetManager instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            // persist between title/game scenes
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            GameObject.Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        // because I have the fast play mode settings enabled, we need to manually reset static instances
        if (_instance == this)
        {
            _instance = null;
        }
    }
#endif

    public void AddPlanet(PlanetInfo planet)
    {
        _planets.Add(planet);
        onAddPlanet?.Invoke(planet);
    }

    public void GalaxyCreatorComplete()
    {
#if UNITY_EDITOR
        var stopwatch = new Stopwatch();
        stopwatch.Start();
#endif

        var points = new Vector3[planets.Count];
        for (int i = 0; i < planets.Count; i++)
            points[i] = planets[i].position;

        kdTree = new KDTree(points);
        kdQuery = new KDQuery();

#if UNITY_EDITOR
        stopwatch.Stop();
        UnityEngine.Debug.Log($"tree built in {stopwatch.ElapsedMilliseconds} ms");
#endif
    }

    public PlanetInfo ClosestPlanet(Vector3 point)
    {
        queryResults.Clear();
        kdQuery.ClosestPoint(kdTree, point, queryResults);

        if (queryResults.Count == 0)
        {
            return null;
        }

        int index = queryResults[0];
        return planets[index];
    }

    public List<PlanetInfo> KNearestPlanets(Vector3 point, int k)
    {
        queryResults.Clear();
        kdQuery.KNearest(kdTree, point, k, queryResults);
        return queryResults.Select(index => planets[index]).ToList();
    }
}

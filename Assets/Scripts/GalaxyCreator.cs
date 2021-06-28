using System.Collections;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using System.Diagnostics;
using Debug = UnityEngine.Debug;
#endif

public class GalaxyCreator : MonoBehaviour
{
    public PlanetManager planetManager;

    [Header("Planet type ratios")]
    public float gardenWorldRatio = 10f;
    public float rockyPlanetRatio = 10f;
    public float gasGiantRatio = 10f;

    [Header("Size")]
    public float boundingRadius = 500f;
    public float radius = 50f;
    public int maxPerFrame = 100;
    public float maxPerFrameFalloff = 1f;

    private bool _isDone = false;
    public bool isDone => isDone;

    private void Start()
    {
        if (planetManager == null)
        {
            Debug.LogError("planetManager should not be null");
            return;
        }

        StartCoroutine(InstantiatePlanetsCoroutine());
    }

    private IEnumerator InstantiatePlanetsCoroutine()
    {
#if UNITY_EDITOR
        var stopwatch = new Stopwatch();
        stopwatch.Start();
#endif

        var bounds = new Bounds(Vector3.zero, Vector3.one * boundingRadius);
        var sampler = new PoissonDiskSampler3D(bounds, radius);

        float weightSum = gardenWorldRatio + rockyPlanetRatio + gasGiantRatio;

        uint total = 0;
        int frameTotal = 0;
        int frameCount = 0;

        foreach (var point in sampler.Samples(transform.position))
        {
            // select a planet type from weighted choices
            float choice = Random.value * weightSum;
            var type = PlanetType.GasGiant;

            if ((choice -= gardenWorldRatio) < 0)
                type = PlanetType.GardenWorld;
            else if ((choice -= rockyPlanetRatio) < 0)
                type = PlanetType.RockyPlanet;

            // add to planet manager
            planetManager.AddPlanet(new PlanetInfo(point, type));

            total++;

            // slowly decrease the number of planets added per frame as the operation becomes more
            // computationally expensive
            int falloff = Mathf.FloorToInt(Mathf.Pow(total * 0.001f, maxPerFrameFalloff));
            int maxThisFrame = Mathf.Max(maxPerFrame - falloff, 1);

            if (frameTotal++ > maxThisFrame)
            {
                frameTotal = 0;
                frameCount++;
                yield return null;
            }
        }

#if UNITY_EDITOR
        stopwatch.Stop();
        Debug.Log($"{total} planets created in {stopwatch.ElapsedMilliseconds} ms, {frameCount} frames");
#endif
    }

#if UNITY_EDITOR
    [ContextMenu("Test density")]
    private void TestDensity()
    {
        int tests = 100;

        // volume relative to sphere size
        float volume = boundingRadius * boundingRadius * boundingRadius / (radius * radius * radius);
        var bounds = new Bounds(Vector3.zero, Vector3.one * boundingRadius);

        float avgDensity = 0f;
        float minDensity = float.MaxValue;
        float maxDensity = float.MinValue;
        for (int i = 0; i < tests; i++)
        {
            int count = new PoissonDiskSampler3D(bounds, radius).Samples().Count();
            float density = count / volume;
            avgDensity += density / (float) tests;
            minDensity = Mathf.Min(minDensity, density);
            maxDensity = Mathf.Max(maxDensity, density);
        }

        Debug.Log($"Average: {avgDensity}, Min: {minDensity}, Max: {maxDensity}");
    }
#endif
}

using System.Collections;
using System.Linq;
using UnityEngine;

public class GalaxyCreator : MonoBehaviour
{
    public GameObject[] planetPrefabs;
    public float boundingRadius = 500f;
    public float radius = 50f;
    public int maxPerFrame = 100;

    private void Start()
    {
        StartCoroutine(InstantiatePlanetsCoroutine());
    }

    private IEnumerator InstantiatePlanetsCoroutine()
    {
        var bounds = new Bounds(Vector3.zero, Vector3.one * boundingRadius);
        var sampler = new PoissonDiskSampler3D(bounds, radius);

        float estimatedDensity = 0.666f;
        int estimate = (int) (bounds.size.x * bounds.size.y * bounds.size.z * estimatedDensity / (radius * radius * radius));

        Debug.Log("Creating planets...");

        int frameTotal = 0;
        int total = 0;
        int frameCount = 0;

        foreach (var point in sampler.Samples(transform.position))
        {
            var prefab = planetPrefabs[Random.Range(0, planetPrefabs.Length)];
            Instantiate(prefab, point, Random.rotation, transform);

            total++;
            frameTotal++;

            int currentMax = (int) ((1f - Mathf.Pow(total / estimate, 6f)) * maxPerFrame);
            currentMax = Mathf.Max(currentMax, 0);

            if (frameTotal >= currentMax)
            {
                yield return null;
                frameTotal = 0;
                frameCount++;
            }
        }

        Debug.Log($"Done. {total} planets created ({estimate} estimated) in {frameCount} frames.");
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

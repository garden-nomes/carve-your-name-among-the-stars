using System.Collections.Generic;
using UnityEngine;

public class GalaxyCreator : MonoBehaviour
{
    public GameObject[] planetPrefabs;
    public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 100f);
    public float radius = 10f;

    private void Start()
    {
        InstantiatePlanets();
    }

    private void InstantiatePlanets()
    {
        var sampler = new PoissonDiskSampler3D(bounds, radius);

        int i = 0;
        int n = sampler.MaxPointCount;

        Debug.Log("Instantiating planets...");

        foreach (var sample in sampler.Samples())
        {
            var prefab = planetPrefabs[Random.Range(0, planetPrefabs.Length)];
            Instantiate(prefab, sample, Quaternion.identity, transform);
            i++;
        }

        Debug.Log($"Instantiated {i} planets");
    }

#if UNITY_EDITOR
    private List<Vector3> previewSamples = new List<Vector3>();

    private void OnValidate()
    {
        int planetCount = new PoissonDiskSampler3D(bounds, radius).MaxPointCount;
        Debug.Log($"Maximum planet count: {planetCount}");

        var previewBounds = bounds;

        previewSamples = new List<Vector3>();
        int i = 0;

        foreach (var sample in new PoissonDiskSampler3D(previewBounds, radius).Samples())
        {
            previewSamples.Add(sample);

            if (i++ > 30)
            {
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        if (Application.isPlaying)
        {
            return;
        }

        foreach (var sample in previewSamples)
        {
            Gizmos.DrawWireSphere(sample, radius * 0.5f);
        }
    }
#endif
}

using System.Collections.Generic;
using UnityEngine;

// Poisson *sphere* sampler?
// based on https://gist.github.com/hiroakioishi/382a6ecbf741c5e0d463
public class PoissonDiskSampler3D
{
    // max number of tries before marking a node as inactive
    private const int K = 30;

    private float radius;
    private readonly Bounds bounds;
    private readonly float cellSize;
    private Vector3[, , ] grid;
    private List<Vector3> activeSamples;

    public int MaxPointCount => grid.GetLength(0) * grid.GetLength(1) * grid.GetLength(2);

    public PoissonDiskSampler3D(Bounds bounds, float radius)
    {
        this.bounds = bounds;
        this.radius = radius;
        this.cellSize = radius / Mathf.Sqrt(3f);
        this.grid = new Vector3[
            Mathf.CeilToInt(bounds.size.x / cellSize),
            Mathf.CeilToInt(bounds.size.y / cellSize),
            Mathf.CeilToInt(bounds.size.z / cellSize)
        ];
        this.activeSamples = new List<Vector3>();
    }

    public IEnumerable<Vector3> Samples(Vector3? initialPoint = null)
    {
        if (initialPoint is Vector3 point)
        {
            // workaround, since Vector3 isn't nullable then the default (0f, 0f, 0f) indicates an
            // uninitialized value
            if (point == default(Vector3))
                point.x += float.Epsilon;

            yield return AddSample(initialPoint.Value);
        }
        else
        {
            var randomPointInBounds = bounds.min + new Vector3(
                Random.value * bounds.size.x,
                Random.value * bounds.size.y,
                Random.value * bounds.size.z);

            yield return AddSample(randomPointInBounds);
        }

        while (activeSamples.Count > 0)
        {
            int i = Random.Range(0, activeSamples.Count);
            var activeSample = activeSamples[i];

            bool isFound = false;

            for (int j = 0; j < K; j++)
            {
                var sample = GeneratePointAround(activeSample);

                if (IsValidSample(sample))
                {
                    isFound = true;
                    yield return AddSample(sample);
                    break;
                }
            }

            if (!isFound)
            {
                activeSamples[i] = activeSamples[activeSamples.Count - 1];
                activeSamples.RemoveAt(activeSamples.Count - 1);
            }
        }
    }

    private Vector3 AddSample(Vector3 sample)
    {
        activeSamples.Add(sample);
        var indexes = GetGridIndexes(sample);
        grid[indexes.x, indexes.y, indexes.z] = sample;
        return sample;
    }

    private Vector3 GeneratePointAround(Vector3 sample)
    {
        return sample + Random.onUnitSphere * radius * (1f + Random.value);
    }

    private bool IsValidSample(Vector3 sample)
    {
        if (!bounds.Contains(sample))
        {
            return false;
        }

        var indexes = GetGridIndexes(sample);

        var minIndexes = new Vector3Int(
            Mathf.Max(indexes.x - 2, 0),
            Mathf.Max(indexes.y - 2, 0),
            Mathf.Max(indexes.z - 2, 0));

        var maxIndexes = new Vector3Int(
            Mathf.Min(indexes.x + 2, grid.GetLength(0) - 1),
            Mathf.Min(indexes.y + 2, grid.GetLength(1) - 1),
            Mathf.Min(indexes.z + 2, grid.GetLength(2) - 1));

        for (int z = minIndexes.z; z <= maxIndexes.z; z++)
        {
            for (int y = minIndexes.y; y <= maxIndexes.y; y++)
            {
                for (int x = minIndexes.x; x <= maxIndexes.x; x++)
                {
                    var otherSample = grid[x, y, z];

                    if (otherSample != default(Vector3) &&
                        (otherSample - sample).sqrMagnitude < radius * radius)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    Vector3Int GetGridIndexes(Vector3 sample)
    {
        return new Vector3Int(
            (int) ((sample.x - bounds.min.x) / cellSize),
            (int) ((sample.y - bounds.min.y) / cellSize),
            (int) ((sample.z - bounds.min.x) / cellSize)
        );
    }
}

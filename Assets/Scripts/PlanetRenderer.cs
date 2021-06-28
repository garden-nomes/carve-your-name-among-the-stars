using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PlanetRenderer : MonoBehaviour
{
    public PlanetManager planetManager;

    public Material gardenWorldMaterial;
    public Material rockyPlanetMaterial;
    public Material gasGiantMaterial;

    private InstanceRenderer gardenWorldRenderer;
    private InstanceRenderer rockyPlanetRenderer;
    private InstanceRenderer gasGiantRenderer;

    private void Awake()
    {
        // create quad
        var mesh = new Mesh();

        mesh.vertices = new Vector3[]
        {
            new Vector3(-.5f, -.5f, 0f),
            new Vector3(.5f, -.5f, 0f),
            new Vector3(-.5f, .5f, 0f),
            new Vector3(.5f, .5f, 0f),
        };

        mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };

        // because our raytraced planet shaders will billboard the vertices towards the camera, we
        // need to adjust the mesh bounds to make sure it isn't accidentally culled
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one);

        gardenWorldRenderer = new InstanceRenderer(gardenWorldMaterial, mesh);
        rockyPlanetRenderer = new InstanceRenderer(rockyPlanetMaterial, mesh);
        gasGiantRenderer = new InstanceRenderer(gasGiantMaterial, mesh);

        if (planetManager == null)
        {
            Debug.LogError("planetManager should not be null");
            return;
        }

        planetManager.onAddPlanet += AddPlanet;
    }

    private void Update()
    {
        gardenWorldRenderer.Render();
        rockyPlanetRenderer.Render();
        gasGiantRenderer.Render();
    }

    private void AddPlanet(PlanetInfo planet)
    {
        var matrix = Matrix4x4.TRS(planet.position, Quaternion.identity, Vector3.one);

        switch (planet.type)
        {
            case PlanetType.GardenWorld:
                gardenWorldRenderer.Add(matrix);
                break;
            case PlanetType.RockyPlanet:
                rockyPlanetRenderer.Add(matrix);
                break;
            case PlanetType.GasGiant:
                gasGiantRenderer.Add(matrix);
                break;
            default:
                Debug.LogError($"unhandled planet type: {planet.type}");
                break;
        }
    }

    private class InstanceRenderer
    {
        // DrawInstancedMesh can only render 1023 instances in a single call, so we store the matrices
        // as a list of lists rather than a flat list
        private readonly List<List<Matrix4x4>> batchedMatrices = new List<List<Matrix4x4>>();
        private readonly Material material;
        private readonly Mesh mesh;

        public InstanceRenderer(Material material, Mesh mesh)
        {
            this.material = material;
            this.mesh = mesh;
        }

        public void Add(Matrix4x4 matrix)
        {
            if (batchedMatrices.Count == 0 || batchedMatrices[batchedMatrices.Count - 1].Count >= 1023)
            {
                batchedMatrices.Add(new List<Matrix4x4>(1023));
            }

            batchedMatrices[batchedMatrices.Count - 1].Add(matrix);
        }

        public void Render()
        {
            int limitBatches = Mathf.Min((int) (Time.frameCount) + 1, batchedMatrices.Count);

            foreach (var batch in batchedMatrices)
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, batch, null, ShadowCastingMode.Off, false);
            }
        }
    }
}

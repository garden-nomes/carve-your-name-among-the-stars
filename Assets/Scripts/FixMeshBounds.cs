using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FixMeshBounds : MonoBehaviour
{
    void Start()
    {
        // because this uses raytracing magic, we need to manually adjust the mesh bounds to avoid
        // camera culling issues
        GetComponent<MeshFilter>().sharedMesh.bounds = new Bounds(Vector3.zero, Vector3.one);
    }
}

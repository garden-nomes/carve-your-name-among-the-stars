using UnityEngine;

[ExecuteInEditMode]
public class PlanetLight : MonoBehaviour
{
    public Material[] planetMaterials;

    private void Update()
    {
        foreach (var material in planetMaterials)
        {
            material.SetVector("_LightPosition", transform.position);
        }
    }
}

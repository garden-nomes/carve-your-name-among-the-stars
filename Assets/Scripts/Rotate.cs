using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotation;

    void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(rotation * Time.deltaTime, transform.up);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetIndicator : MonoBehaviour
{
    public GameObject planet;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (planet == null)
        {
            return;
        }

        rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(planet.transform.position);
    }
}

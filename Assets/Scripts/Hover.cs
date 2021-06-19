using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float frequency = 1f;
    public float amplitude = 1f;

    private RectTransform rectTransform;
    private Vector2 position;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        position = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;
        rectTransform.anchoredPosition = position + new Vector2(0f, offset);
    }
}

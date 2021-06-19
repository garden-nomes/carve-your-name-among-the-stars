using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerUI : MonoBehaviour
{
    public bool isVisible = false;
    public ProgressBar progressBar;
    public GameObject label;
    [Range(0f, 1f)] public float progress = 0f;

    private Vector2 position;
    private RectTransform rectTransform;
    private float labelFlashTimer;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        position = rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (isVisible)
        {
            // make visible
            rectTransform.anchoredPosition = position;

            // update progress bar
            progressBar.progress = progress;

            // flash label
            labelFlashTimer += Time.deltaTime;
            label.SetActive(labelFlashTimer * 2f % 1f < 0.666f);
        }
        else
        {
            labelFlashTimer = 0f;

            // hide offscreen
            rectTransform.anchoredPosition = new Vector2(0f, -100f);
        }
    }
}

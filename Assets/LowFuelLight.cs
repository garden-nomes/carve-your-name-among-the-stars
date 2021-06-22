using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LowFuelLight : MonoBehaviour
{
    public AudioSource ding;
    public float loopTime = 1f;

    private float timer = 0f;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        timer = 0f;
        ding.Play();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= loopTime)
        {
            timer -= loopTime;
            ding.Play();
        }

        image.enabled = timer / loopTime < 0.5f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipAudio : MonoBehaviour
{
    public AudioSource engineRumble;
    public float engineRumbleFade = 0.1f;

    private SpaceshipController spaceshipController;
    private bool isRumbly = false;

    void Start()
    {
        spaceshipController = GetComponent<SpaceshipController>();
        engineRumble.volume = 0f;
    }

    void Update()
    {
        if (spaceshipController.isAccelerating && !isRumbly)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInCoroutine(engineRumble, engineRumbleFade));
            isRumbly = true;
        }
        else if (!spaceshipController.isAccelerating && isRumbly)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutCoroutine(engineRumble, engineRumbleFade));
            isRumbly = false;
        }
    }

    private IEnumerator FadeInCoroutine(AudioSource source, float time)
    {
        source.Play();

        while (source.volume < 1f)
        {
            yield return null;
            source.volume += Time.deltaTime / time;
        }

        source.volume = 1f;
    }

    private IEnumerator FadeOutCoroutine(AudioSource source, float time)
    {
        while (source.volume > 0f)
        {
            yield return null;
            source.volume -= Time.deltaTime / time;
        }

        source.volume = 0f;
        source.Stop();
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DitheredFadeEffect : MonoBehaviour
{
    public Shader shader;
    public float fadeTime = 1f;
    public bool fadeInOnStart = true;

    private Material material;
    private Image image;
    private int fadeProperty = Shader.PropertyToID("_Fade");
    private int colorProperty = Shader.PropertyToID("_Color");

    void Start()
    {
        material = new Material(shader);
        material.SetFloat(fadeProperty, fadeInOnStart ? 0f : 1f);

        image = GetComponent<Image>();
        image.material = material;
        image.enabled = true;

        material.SetColor(colorProperty, image.color);

        if (fadeInOnStart)
        {
            FadeIn();
        }
    }

    [ContextMenu("Fade In")]
    public Coroutine FadeIn()
    {
        StopAllCoroutines();
        return StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float currentFade = material.GetFloat(fadeProperty);

        while (currentFade < 1f)
        {
            currentFade += Time.deltaTime / fadeTime;
            material.SetFloat(fadeProperty, currentFade);
            yield return null;
        }

        material.SetFloat(fadeProperty, 1f);
    }

    [ContextMenu("Fade Out")]
    public Coroutine FadeOut()
    {
        StopAllCoroutines();
        return StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float currentFade = material.GetFloat(fadeProperty);

        while (currentFade > 0f)
        {
            currentFade -= Time.deltaTime / fadeTime;
            material.SetFloat(fadeProperty, currentFade);
            yield return null;
        }

        material.SetFloat(fadeProperty, 0f);
    }

}

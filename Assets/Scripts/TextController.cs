using System.Collections;
using System.Collections.Generic;
using Unity.PixelText;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public PixelText text;
    public GameObject continueIcon;
    public SpaceshipController spaceship;
    public float minimumTextDelay = 0.5f;

    private RectTransform rectTransform;
    private Vector2 position;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        position = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(0f, -200f);
    }

    public void ShowText(string text)
    {
        // split text into pages on a triple-newline
        var pages = text.Split(new [] { "\n\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        StartCoroutine(ShowTextCoroutine(pages));
    }

    private IEnumerator ShowTextCoroutine(string[] pages)
    {
        rectTransform.anchoredPosition = position;
        spaceship.disableThrottle = true;

        foreach (var page in pages)
        {
            text.text = page;

            // delay to make sure the player doesn't accidentally click through
            yield return new WaitForSeconds(minimumTextDelay);

            while (!Input.GetKeyUp(KeyCode.Z))
            {
                yield return null;
            }
        }

        rectTransform.anchoredPosition = new Vector2(0f, -200f);
        spaceship.disableThrottle = false;
    }
}

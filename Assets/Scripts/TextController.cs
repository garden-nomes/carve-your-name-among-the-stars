using System.Collections;
using System.Collections.Generic;
using Unity.PixelText;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public AudioClip openSound;
    public AudioClip continueSound;
    public AudioClip closeSound;

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

    public Coroutine ShowText(string text)
    {
        // split text into pages on a triple-newline
        var pages = text
            .ToUpper()
            .Split(new [] { "\n\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        return StartCoroutine(ShowTextCoroutine(pages));
    }

    private IEnumerator ShowTextCoroutine(string[] pages)
    {
        rectTransform.anchoredPosition = position;
        spaceship.disableThrottle = true;

        AudioSource.PlayClipAtPoint(openSound, Camera.main.transform.position);

        foreach (var page in pages)
        {
            text.text = page;

            // delay to make sure the player doesn't accidentally click through
            yield return new WaitForSeconds(minimumTextDelay);

            while (!Input.GetKeyUp(KeyCode.Z))
            {
                yield return null;
            }

            if (page != pages[pages.Length - 1])
            {
                AudioSource.PlayClipAtPoint(continueSound, Camera.main.transform.position);
            }
        }

        AudioSource.PlayClipAtPoint(closeSound, Camera.main.transform.position);

        rectTransform.anchoredPosition = new Vector2(0f, -200f);
        spaceship.disableThrottle = false;
    }
}

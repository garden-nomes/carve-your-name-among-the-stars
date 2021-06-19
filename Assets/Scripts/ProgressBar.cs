using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [Range(0f, 1f)] public float progress = 0f;

    private float width;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.sizeDelta.x;
    }

    void Update()
    {
        rectTransform.sizeDelta = new Vector2(width * progress, rectTransform.sizeDelta.y);
    }
}

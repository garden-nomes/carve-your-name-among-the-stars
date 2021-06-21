using UnityEngine;
using UnityEngine.UI;

public class OffscreenPointer : MonoBehaviour
{
    public Sprite lateralArrow;
    public Sprite diagonolArrow;

    public Transform target;
    public RectTransform bounds;

    private RectTransform rectTransform;
    private Image image;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        // hide if no target
        if (target == null)
        {
            rectTransform.anchoredPosition = new Vector2(0f, -100f);
            return;
        }

        // get screen position of target
        var screenPosition = Camera.main.WorldToScreenPoint(target.position);

        // check if onscreen
        if (screenPosition.x >= 0f && screenPosition.x < Screen.width &&
            screenPosition.y >= 0f && screenPosition.y < Screen.height)
        {
            rectTransform.anchoredPosition = new Vector2(0f, -100f);
            return;
        }

        // transform bounding screen position to place (0, 0) at the center of the bounding rect
        screenPosition -= bounds.position;

        // the target is behind the camera, flip coordinates
        screenPosition *= Mathf.Sign(screenPosition.z);

        // calculate the angle and slope of a line from the center to the target
        float angle = Mathf.Atan2(screenPosition.y, screenPosition.x);
        float slope = Mathf.Tan(angle);

        // intersect of our line with the left or right bounds
        var extent = bounds.rect.size * 0.5f;
        var arrowPosition = new Vector2(extent.x, extent.x * slope) * Mathf.Sign(screenPosition.x);

        // if offscreen, instead intersect with top or bottom bounds
        if (Mathf.Abs(arrowPosition.y) > extent.y)
        {
            arrowPosition = new Vector2(extent.y / slope, extent.y) * Mathf.Sign(screenPosition.y);
        }

        // transform back to screen space
        arrowPosition += (Vector2) bounds.position;

        // snap to pixel grid
        arrowPosition.x = Mathf.Round(arrowPosition.x - 0.5f) + 0.5f;
        arrowPosition.y = Mathf.Round(arrowPosition.y - 0.5f) + 0.5f;

        // set position
        rectTransform.anchoredPosition = arrowPosition;

        // snap to 45-degree rotation
        float rotation = Mathf.Round(angle * Mathf.Rad2Deg / 45f) * 45f;

        // set sprite to lateral or diagonal version depending on rotation
        if (rotation % 90f != 0f)
        {
            rotation -= 45f;
            image.sprite = diagonolArrow;
        }
        else
        {
            image.sprite = lateralArrow;
        }

        rectTransform.localRotation = Quaternion.AngleAxis(rotation, Vector3.forward);
    }
}

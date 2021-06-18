using UnityEngine;

public class Dial : MonoBehaviour
{
    public float value = 0f;
    public float fromAngle = 180f;
    public float toAngle = 0f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float angle = Mathf.Lerp(fromAngle, toAngle, value);
        rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}

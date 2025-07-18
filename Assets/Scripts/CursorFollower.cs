using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    public Canvas canvas;  // Assign your Canvas here

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        // Convert screen space mouse position to canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePos,
            canvas.worldCamera,
            out Vector2 localPoint);

        rectTransform.localPosition = localPoint;
    }
}


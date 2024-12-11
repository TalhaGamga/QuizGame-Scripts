using UnityEngine;

public class AnswerHolder : MonoBehaviour
{
    [SerializeField] private Transform target;           // The world object (e.g., the hex or A marker).
    [SerializeField] private RectTransform uiElement;   // The UI element (e.g., the "A" icon).
    [SerializeField] private Camera mainCamera;         // Reference to the main camera.

    void Update()
    {
        if (target == null || uiElement == null || mainCamera == null)
            return;

        // Convert world position to screen position
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position);

        // Check if the target is in front of the camera
        if (screenPosition.z > 0)
        {
            // Convert screen position to UI canvas position
            uiElement.position = screenPosition;
            uiElement.gameObject.SetActive(true); // Ensure it's visible
        }
        else
        {
            // Hide the UI element if the target is behind the camera
            uiElement.gameObject.SetActive(false);
        }
    }
}

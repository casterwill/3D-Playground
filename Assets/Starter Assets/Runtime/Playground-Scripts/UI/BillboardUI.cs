using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // Cache the main camera's transform for better performance
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // Make the UI look at the camera, matching its orientation
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                             mainCameraTransform.rotation * Vector3.up);
        }
    }
}

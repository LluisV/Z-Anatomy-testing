using UnityEngine;

public class DiscController : MonoBehaviour
{
    public float rotateSpeed = 100f;
    public float maxRotationAngle = 45f;

    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            float horizontal = (Input.mousePosition.x - lastMousePosition.x) / Screen.width;

            // Rotate the object horizontally
            float angle = horizontal * rotateSpeed * Time.deltaTime;
            angle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);
            transform.Rotate(Vector3.up, angle, Space.Self);

            lastMousePosition = Input.mousePosition;
        }
    }
}

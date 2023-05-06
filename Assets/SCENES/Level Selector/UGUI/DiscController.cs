using UnityEngine;

public class DiscController : MonoBehaviour
{
    public float rotateSpeed = 100f;

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
            Quaternion rotation = transform.rotation;
            float angle = horizontal * rotateSpeed * Time.deltaTime;
            if (angle > 0)
            {
                angle = Mathf.Min(angle, 45f);
            }
            else if (angle < 0)
            {
                angle = Mathf.Max(angle, -45f);
            }
            rotation *= Quaternion.Euler(0f, angle, 0f);
            transform.rotation = rotation;

            lastMousePosition = Input.mousePosition;
        }
    }
}


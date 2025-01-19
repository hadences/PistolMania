using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] private float rotationSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotatePlayerToMouse();
    }

    void rotatePlayerToMouse() {
        // Get mouse position in world coordinates
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Compute direction and raw angle from the player to the mouse
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust angle and sprite flip based on direction
        if (angle > 90 || angle < -90) {
            spriteRenderer.flipX = true;
            // Mirror the angle across the vertical axis for smoother transition
            angle += 270;
        }
        else {
            spriteRenderer.flipX = false;
        }

        // Get current Z-angle of the transform
        float currentAngle = transform.eulerAngles.z;
        // Compute the new target angle using LerpAngle for smooth interpolation
        float targetAngle = Mathf.LerpAngle(currentAngle, angle, rotationSpeed * Time.deltaTime);

        // Apply the rotation to the player
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }
}

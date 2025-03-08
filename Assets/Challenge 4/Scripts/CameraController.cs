using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Reference to the player's transform (the ball)
    public float rotationSpeed = 5f; // Speed of camera rotation
    public Vector3 offset; // The offset between the camera and player
    private float currentAngleX = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial offset between camera and player
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Get the mouse input for horizontal (X) rotation
        float horizontalInput = Input.GetAxis("Mouse X");

        // Adjust the camera's angle based on mouse input (only horizontal rotation)
        currentAngleX += horizontalInput * rotationSpeed;

        // Calculate the new camera position and rotation (only rotating horizontally)
        Quaternion rotation = Quaternion.Euler(0, currentAngleX, 0); // Lock vertical rotation to 0
        Vector3 newPosition = player.position + rotation * offset;

        // Apply the new position to the camera
        transform.position = newPosition;

        // Make the camera always look at the player
        transform.LookAt(player);
    }
}

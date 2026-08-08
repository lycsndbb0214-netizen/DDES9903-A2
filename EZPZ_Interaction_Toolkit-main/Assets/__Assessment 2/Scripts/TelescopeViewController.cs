using UnityEngine;

public class TelescopeViewController : MonoBehaviour
{
    [Header("Camera References")]
    public GameObject telescopeCamera;         // Child telescope camera
    public GameObject mainCamera;              // Player's main camera
    public MonoBehaviour playerMovementScript; // Player control script
    public ExplorationNode explorationNode;   // Attached ExplorationNode

    [Header("Rotation Limits")]
    public float sensitivity = 2f;
    public float yawLimit = 30f;   // Horizontal rotation limit
    public float pitchLimit = 15f; // Vertical rotation limit

    private bool isUsingTelescope = false;
    private Quaternion baseRotation;
    private float yaw = 0f;
    private float pitch = 0f;

    private void Awake()
    {
        if (telescopeCamera != null)
        {
            // Record original camera rotation from Inspector
            baseRotation = telescopeCamera.transform.localRotation;
            telescopeCamera.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isUsingTelescope) return;

        // Mouse view rotation
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;

        yaw = Mathf.Clamp(yaw, -yawLimit, yawLimit);
        pitch = Mathf.Clamp(pitch, -pitchLimit, pitchLimit);

        if (telescopeCamera != null)
        {
            telescopeCamera.transform.localRotation = baseRotation * Quaternion.Euler(pitch, yaw, 0f);
        }

        // Emergency exit using ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitTelescope();
        }
    }

    public void EnterTelescope()
    {
        isUsingTelescope = true;
        yaw = 0f;
        pitch = 0f;

        if (telescopeCamera != null)
        {
            baseRotation = telescopeCamera.transform.localRotation;
            telescopeCamera.SetActive(true);
        }

        if (mainCamera != null) mainCamera.SetActive(false);
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (explorationNode != null)
        {
            explorationNode.CompleteNode();
        }
    }

    public void ExitTelescope()
    {
        if (!isUsingTelescope) return;

        isUsingTelescope = false;

        if (telescopeCamera != null)
        {
            telescopeCamera.transform.localRotation = baseRotation;
            telescopeCamera.SetActive(false);
        }

        if (mainCamera != null) mainCamera.SetActive(true);
        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }
}
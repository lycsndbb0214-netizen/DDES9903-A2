using UnityEngine;

public class SecondLifeboat : MonoBehaviour
{
    [Header("Gentle Floating Settings")]
    public bool isFloating = true;
    public float pitchAmplitude = 2.5f;   // Pitch angle (tilt forward/backward)
    public float rollAmplitude = 1.8f;    // Roll angle (tilt left/right)
    public float heaveAmplitude = 0.12f;  // Up and down heave height on water
    public float floatSpeed = 1.2f;       // Ocean wave frequency speed

    [Header("Move Away Settings")]
    public float moveSpeed = 3.0f;           // Speed of the lifeboat sailing away
    public Transform playerSeatPoint;        // Target position transform where the player sits
    public Transform moveTargetPoint;        // Distant waypoint on the sea to move towards

    [Tooltip("Increased distance threshold to ensure the trigger detection successfully registers")]
    public float stopDistanceThreshold = 10.0f; // Directly increased to 10 meters for reliable detection

    private Vector3 initialPos;
    private Quaternion initialRot;
    private bool isMovingAway = false;
    private bool hasReachedDestination = false;
    private Transform playerTransform;

    private void OnEnable()
    {
        // Record the baseline position and rotation upon activation on the sea
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
    }

    private void Update()
    {
        // 1. Simulate gentle water buoyancy and rotation harmonics
        if (isFloating)
        {
            float pitch = Mathf.Sin(Time.time * floatSpeed) * pitchAmplitude;
            float roll = Mathf.Cos(Time.time * floatSpeed * 0.8f) * rollAmplitude;
            float heave = Mathf.Sin(Time.time * floatSpeed * 1.5f) * heaveAmplitude;

            transform.localRotation = initialRot * Quaternion.Euler(pitch, 0, roll);

            // Only apply localized heave offset if the boat hasn't started traveling
            if (!isMovingAway)
            {
                transform.localPosition = initialPos + new Vector3(0, heave, 0);
            }
        }

        // 2. Handle the escape navigation logic once player boards
        if (isMovingAway && !hasReachedDestination)
        {
            if (moveTargetPoint != null)
            {
                // Calculate directional vector and apply translational movement
                Vector3 targetDir = (moveTargetPoint.position - transform.position).normalized;
                transform.position += targetDir * moveSpeed * Time.deltaTime;

                // Calculate distance between the lifeboat and the target waypoint
                float distance = Vector3.Distance(transform.position, moveTargetPoint.position);

                // Proximity check using the expanded stop threshold
                if (distance <= stopDistanceThreshold)
                {
                    hasReachedDestination = true;
                    Debug.Log($"[Lifeboat 2] Destination reached! Final Distance: {distance}m");

                    // Notify StoryManager to initiate the terminal blackout
                    if (StoryManager.Instance != null)
                    {
                        StoryManager.Instance.TriggerFadeToBlack();
                    }
                }
            }
            else
            {
                // Fallback translational drive forward if waypoint configuration is missing
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }

            // Lock the player to the seat reference frame to prevent drifting/sliding off
            if (playerTransform != null && playerSeatPoint != null)
            {
                playerTransform.position = playerSeatPoint.position;
            }
        }
    }

    /// <summary>
    /// Attaches the player to the lifeboat seat and initiates the escape cinematic sequence.
    /// Called via WaterRespawnTrigger or localized onboard trigger boundaries.
    /// </summary>
    public void BoardPlayer(Transform player)
    {
        if (isMovingAway) return;
        isMovingAway = true;
        playerTransform = player;

        Debug.Log("[Lifeboat 2] Player boarded successfully! Initiating escape vector.");

        // Snap player transform matrices to the seat specifications and re-parent
        if (playerSeatPoint != null)
        {
            player.position = playerSeatPoint.position;
            player.rotation = playerSeatPoint.rotation;
            player.SetParent(transform);
        }

        // Signal the core framework manager to commence ship hull submersion
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.StartCinematicEnding();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enforce boundary filter checks to validate player or camera interaction layers
        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null)
        {
            BoardPlayer(other.transform.root);
        }
    }
}
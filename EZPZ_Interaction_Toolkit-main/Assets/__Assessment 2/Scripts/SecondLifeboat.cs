using UnityEngine;

public class SecondLifeboat : MonoBehaviour
{
    [Header("Gentle Floating Settings")]
    public bool isFloating = true;
    public float pitchAmplitude = 2.5f;   // Pitch angle (tilt forward/backward)
    public float rollAmplitude = 1.8f;    // Roll angle (tilt left/right)
    public float heaveAmplitude = 0.12f;  // Up and down heave height on water
    public float floatSpeed = 1.2f;       // Ocean wave speed

    private Vector3 initialPos;
    private Quaternion initialRot;
    private bool playerEntered = false;

    private void OnEnable()
    {
        // Record initial transform position/rotation when activated on the sea
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
    }

    private void Update()
    {
        if (!isFloating) return;

        // Calculate ocean wave motion (pitch, roll, heave)
        float pitch = Mathf.Sin(Time.time * floatSpeed) * pitchAmplitude;
        float roll = Mathf.Cos(Time.time * floatSpeed * 0.8f) * rollAmplitude;
        float heave = Mathf.Sin(Time.time * floatSpeed * 1.5f) * heaveAmplitude;

        // Apply gentle floating transformation
        transform.localRotation = initialRot * Quaternion.Euler(pitch, 0, roll);
        transform.localPosition = initialPos + new Vector3(0, heave, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerEntered) return;

        // Detect when the player jumps inside the floating lifeboat
        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null)
        {
            playerEntered = true;
            Debug.Log("[Lifeboat 2] Player jumped into floating lifeboat! Triggering ending.");

            // Notify StoryManager to trigger black screen fade and end UI
            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.OnPlayerEnteredLifeboat();
            }
        }
    }
}

using UnityEngine;

public class ShipMotion : MonoBehaviour
{
    [Header("Motion Toggle")]
    public bool isRocking = true;

    [Header("Normal Mode (Before Phone Call)")]
    public float normalPitch = 0.5f;   // Gentle pitch angle
    public float normalHeave = 0.05f;  // Gentle up/down heave
    public float normalSpeed = 0.8f;   // Gentle wave speed

    [Header("Storm Mode (After Phone Call)")]
    public float stormPitch = 3.5f;    // Violent pitch angle
    public float stormHeave = 0.4f;    // Violent up/down heave
    public float stormSpeed = 1.8f;    // Fast storm speed

    [Header("Transition Settings")]
    public float transitionSpeed = 1.0f; // Speed of lerping to storm mode

    private float currentPitch;
    private float currentHeave;
    private float currentSpeed;

    private float targetPitch;
    private float targetHeave;
    private float targetSpeed;

    private Vector3 initialPos;
    private Quaternion initialRot;

    private void Start()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;

        // Initialize with gentle normal parameters
        currentPitch = normalPitch;
        currentHeave = normalHeave;
        currentSpeed = normalSpeed;

        targetPitch = normalPitch;
        targetHeave = normalHeave;
        targetSpeed = normalSpeed;
    }

    private void Update()
    {
        if (!isRocking) return;

        // Smoothly interpolate current values towards target values
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * transitionSpeed);
        currentHeave = Mathf.Lerp(currentHeave, targetHeave, Time.deltaTime * transitionSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * transitionSpeed);

        // Calculate smooth ocean wave motion
        float pitch = Mathf.Sin(Time.time * currentSpeed) * currentPitch;
        float roll = Mathf.Cos(Time.time * currentSpeed * 0.8f) * (currentPitch * 0.5f);
        float heave = Mathf.Sin(Time.time * currentSpeed * 1.5f) * currentHeave;

        transform.localRotation = initialRot * Quaternion.Euler(pitch, 0, roll);
        transform.localPosition = initialPos + new Vector3(0, heave, 0);
    }

    // Call this function when the phone call finishes to trigger storm motion
    public void TriggerStormMotion()
    {
        targetPitch = stormPitch;
        targetHeave = stormHeave;
        targetSpeed = stormSpeed;
    }
}
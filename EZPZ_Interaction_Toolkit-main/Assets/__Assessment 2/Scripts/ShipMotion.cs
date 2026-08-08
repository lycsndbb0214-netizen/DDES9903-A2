using UnityEngine;

public class ShipMotion : MonoBehaviour
{
    public static ShipMotion Instance;

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

    [Header("Collision Mode (Climax / Iceberg Impact)")]
    public float collisionPitch = 6.0f;     // Extreme pitch angle during collision
    public float collisionHeave = 0.6f;     // Extreme heave during collision
    public float collisionSpeed = 3.0f;     // Fast motion speed during collision
    public float backwardTiltAngle = 12.0f; // Backward tilt angle in degrees

    [Header("Transition Settings")]
    public float transitionSpeed = 1.0f;   // Speed of lerping to target mode

    private float currentPitch;
    private float currentHeave;
    private float currentSpeed;

    private float targetPitch;
    private float targetHeave;
    private float targetSpeed;

    private float pitchOffset = 0f;
    private float targetPitchOffset = 0f;

    private Vector3 initialPos;
    private Quaternion initialRot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;

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
        pitchOffset = Mathf.Lerp(pitchOffset, targetPitchOffset, Time.deltaTime * transitionSpeed);

        // Calculate ocean wave motion including backward tilt angle
        float pitch = Mathf.Sin(Time.time * currentSpeed) * currentPitch + pitchOffset;
        float roll = Mathf.Cos(Time.time * currentSpeed * 0.8f) * (currentPitch * 0.5f);
        float heave = Mathf.Sin(Time.time * currentSpeed * 1.5f) * currentHeave;

        transform.localRotation = initialRot * Quaternion.Euler(pitch, 0, roll);
        transform.localPosition = initialPos + new Vector3(0, heave, 0);
    }

    // Trigger storm motion after phone call
    public void TriggerStormMotion()
    {
        targetPitch = stormPitch;
        targetHeave = stormHeave;
        targetSpeed = stormSpeed;
    }

    // Trigger collision impact and tilt ship backward
    public void TriggerCollisionAndTiltBackward()
    {
        targetPitch = collisionPitch;
        targetHeave = collisionHeave;
        targetSpeed = collisionSpeed;
        targetPitchOffset = backwardTiltAngle; // Tilts ship backward by specified degrees
        transitionSpeed = 2.5f;                // Fast transition speed on impact
    }
}
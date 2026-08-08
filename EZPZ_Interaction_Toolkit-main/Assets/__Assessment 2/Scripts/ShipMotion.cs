using UnityEngine;

public class ShipMotion : MonoBehaviour
{
    public static ShipMotion Instance;

    [Header("Motion Toggle")]
    public bool isRocking = true;

    [Header("Normal Mode (Before Phone Call)")]
    public float normalPitch = 0.5f;
    public float normalHeave = 0.05f;
    public float normalSpeed = 0.8f;

    [Header("Storm Mode (After Phone Call)")]
    public float stormPitch = 3.5f;
    public float stormHeave = 0.4f;
    public float stormSpeed = 1.8f;

    [Header("Collision Mode (Climax / Iceberg Impact)")]
    public float collisionPitch = 6.0f;
    public float collisionHeave = 0.6f;
    public float collisionSpeed = 3.0f;
    public float backwardTiltAngle = 12.0f;

    [Header("Transition Settings")]
    public float transitionSpeed = 1.0f;

    [Header("Sinking Settings")]
    public float sinkSpeed = 0.5f;        // Speed of ship sinking into the sea
    private bool isSinking = false;

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

        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * transitionSpeed);
        currentHeave = Mathf.Lerp(currentHeave, targetHeave, Time.deltaTime * transitionSpeed);
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * transitionSpeed);
        pitchOffset = Mathf.Lerp(pitchOffset, targetPitchOffset, Time.deltaTime * transitionSpeed);

        float pitch = Mathf.Sin(Time.time * currentSpeed) * currentPitch + pitchOffset;
        float roll = Mathf.Cos(Time.time * currentSpeed * 0.8f) * (currentPitch * 0.5f);
        float heave = Mathf.Sin(Time.time * currentSpeed * 1.5f) * currentHeave;

        transform.localRotation = initialRot * Quaternion.Euler(pitch, 0, roll);
        transform.localPosition = initialPos + new Vector3(0, heave, 0);
    }

    private void LateUpdate()
    {
        if (isSinking)
        {
            initialPos.y -= sinkSpeed * Time.deltaTime;
        }
    }

    public void TriggerStormMotion()
    {
        targetPitch = stormPitch;
        targetHeave = stormHeave;
        targetSpeed = stormSpeed;
    }

    public void TriggerCollisionAndTiltBackward()
    {
        targetPitch = collisionPitch;
        targetHeave = collisionHeave;
        targetSpeed = collisionSpeed;
        targetPitchOffset = backwardTiltAngle;
        transitionSpeed = 2.5f;
    }

    public void StartSinking()
    {
        isSinking = true;
    }
}
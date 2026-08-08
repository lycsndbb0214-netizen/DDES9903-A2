using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExplorationNode : MonoBehaviour
{
    [Header("Node Settings")]
    public string nodeID = "Telescope";
    public Light deskLamp;

    [Header("Light Behavior Control")]
    public bool turnLightRedOnVisit = false;
    public bool keepFlashingAfterVisit = false;

    [Header("Audio & Particle Effects")]
    public AudioSource triggerAudio;
    public GameObject[] vfxToEnable;

    [Header("Computer Screen Effects (Optional)")]
    public TMP_Text screenText;
    public float textFlickerSpeed = 4f;

    [Header("Telescope Settings (Optional)")]
    public bool isTelescopeNode = false;
    public GameObject telescopeScopeUI;
    public Camera mainCamera;
    public float telescopeFOV = 15f;

    [Header("Inner Monologue Subtitles")]
    public GameObject subtitleTextObject;
    public string[] monologueLines = new string[] {
        "The fog is so big...",
        "I can't see anything..."
    };
    public float lineDuration = 3.0f;

    private bool isNodeActive = false;
    private bool isVisited = false;
    private float originalFOV;

    private void Start()
    {
        // Store main camera reference and original FOV
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (mainCamera != null)
        {
            originalFOV = mainCamera.fieldOfView;
        }
    }

    private void Update()
    {
        // Lamp gentle flicker before interaction
        if (isNodeActive && !isVisited && deskLamp != null)
        {
            deskLamp.intensity = Mathf.PingPong(Time.time * 5f, 1.2f) + 0.4f;
        }

        // Lamp erratic flash after interaction
        if (isVisited && keepFlashingAfterVisit && deskLamp != null)
        {
            deskLamp.intensity = Random.Range(0.5f, 3.5f);
        }

        // Screen text blinking animation
        if (screenText != null)
        {
            Color c = screenText.color;
            c.a = Mathf.PingPong(Time.time * textFlickerSpeed, 0.9f) + 0.1f;
            screenText.color = c;
        }
    }

    // Called by PostPhoneTransitionManager to activate node
    public void ActivateNode()
    {
        isNodeActive = true;
    }

    // Trigger zone detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null)
        {
            CompleteNode();
        }
    }

    // Core node interaction logic (Can be called by Trigger or Interactable script)
    public void CompleteNode()
    {
        if (isVisited) return;
        isVisited = true;

        // Play audio effect
        if (triggerAudio != null)
        {
            triggerAudio.Play();
        }

        // Enable particle VFX
        if (vfxToEnable != null)
        {
            foreach (GameObject vfx in vfxToEnable)
            {
                if (vfx != null)
                {
                    vfx.SetActive(true);
                    ParticleSystem ps = vfx.GetComponentInChildren<ParticleSystem>();
                    if (ps != null)
                    {
                        ps.Play();
                    }
                }
            }
        }

        // Update light color and state
        if (deskLamp != null)
        {
            if (turnLightRedOnVisit)
            {
                deskLamp.color = Color.red;
            }

            if (!keepFlashingAfterVisit)
            {
                deskLamp.intensity = 2.5f; // Steady solid light
            }
        }

        // Play subtitles and camera sequence
        StartCoroutine(PlayMonologueRoutine());

        Debug.Log($"[Exploration] Node Completed: {nodeID}");

        // Notify StoryManager
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnNodeVisited(nodeID);
        }
    }

    // Subtitle sequence and telescope view control
    private IEnumerator PlayMonologueRoutine()
    {
        // Enter telescope view (Zoom camera and show UI overlay)
        if (isTelescopeNode)
        {
            if (telescopeScopeUI != null) telescopeScopeUI.SetActive(true);
            if (mainCamera != null) mainCamera.fieldOfView = telescopeFOV;
        }

        // Display monologue subtitles line by line
        if (subtitleTextObject != null && monologueLines != null && monologueLines.Length > 0)
        {
            subtitleTextObject.SetActive(true);

            foreach (string line in monologueLines)
            {
                UpdateSubtitleText(line);
                yield return new WaitForSeconds(lineDuration);
            }

            subtitleTextObject.SetActive(false);
        }

        // Exit telescope view (Restore camera FOV and hide UI overlay)
        if (isTelescopeNode)
        {
            if (telescopeScopeUI != null) telescopeScopeUI.SetActive(false);
            if (mainCamera != null) mainCamera.fieldOfView = originalFOV;
        }
    }

    // Update UI text content
    private void UpdateSubtitleText(string content)
    {
        if (subtitleTextObject == null) return;

        Text legacyText = subtitleTextObject.GetComponent<Text>();
        if (legacyText != null)
        {
            legacyText.text = content;
            return;
        }

        TMP_Text tmpText = subtitleTextObject.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = content;
        }
    }
}

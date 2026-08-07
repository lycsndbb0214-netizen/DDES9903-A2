using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExplorationNode : MonoBehaviour
{
    [Header("Node Settings")]
    public string nodeID = "BoilerPipes";
    public Light deskLamp;              // Warning light component

    [Header("Light Behavior Control")]
    public bool turnLightRedOnVisit = false;      // Turn light color to red when triggered
    public bool keepFlashingAfterVisit = false;   // Keep light flashing erratically after visit

    [Header("Audio & Particle Effects")]
    public AudioSource triggerAudio;    // AudioSource for explosion / steam hiss sound
    public GameObject[] vfxToEnable;    // Steam and fire particle effect GameObjects

    [Header("Computer Screen Effects (Optional)")]
    public TMP_Text screenText;         // TextMeshPro component on computer screen (if any)
    public float textFlickerSpeed = 4f;

    [Header("Inner Monologue Subtitles")]
    public GameObject subtitleTextObject; // UI Text Object for subtitles
    public string[] monologueLines = new string[] {
        "The steam pressure is dropping dangerously low...",
        "We're losing control of the engines!"
    };
    public float lineDuration = 3.0f;   // Display duration for each line

    private bool isNodeActive = false;  // Activated by PostPhoneTransition script
    private bool isVisited = false;

    private void Update()
    {
        // 1. Lamp behavior BEFORE visit (gentle flicker)
        if (isNodeActive && !isVisited && deskLamp != null)
        {
            deskLamp.intensity = Mathf.PingPong(Time.time * 5f, 1.2f) + 0.4f;
        }

        // 2. Lamp behavior AFTER visit (if set to keep flashing in red)
        if (isVisited && keepFlashingAfterVisit && deskLamp != null)
        {
            deskLamp.intensity = Random.Range(0.5f, 3.5f);
        }

        // 3. Screen Text Blinking (Optional)
        if (screenText != null)
        {
            Color c = screenText.color;
            c.a = Mathf.PingPong(Time.time * textFlickerSpeed, 0.9f) + 0.1f;
            screenText.color = c;
        }
    }

    public void ActivateNode()
    {
        isNodeActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null)
        {
            CompleteNode();
        }
    }

    public void CompleteNode()
    {
        if (isVisited) return;
        isVisited = true;

        // 1. Play explosion / steam sound effect
        if (triggerAudio != null)
        {
            triggerAudio.Play();
        }

        // 2. Enable steam and fire particle effects
        if (vfxToEnable != null)
        {
            foreach (GameObject vfx in vfxToEnable)
            {
                if (vfx != null)
                {
                    vfx.SetActive(true);
                }
            }
        }

        // 3. Change light color to red and set state
        if (deskLamp != null)
        {
            if (turnLightRedOnVisit)
            {
                deskLamp.color = Color.red;
            }

            if (!keepFlashingAfterVisit)
            {
                deskLamp.intensity = 2.5f; // Solid steady light
            }
        }

        // 4. Play inner monologue subtitles
        StartCoroutine(PlayMonologueRoutine());

        Debug.Log($"[Exploration] Node Completed: {nodeID}");

        // 5. Notify StoryManager to update exploration progress
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnNodeVisited(nodeID);
        }
    }

    private IEnumerator PlayMonologueRoutine()
    {
        if (subtitleTextObject == null || monologueLines == null || monologueLines.Length == 0) yield break;

        subtitleTextObject.SetActive(true);

        foreach (string line in monologueLines)
        {
            UpdateSubtitleText(line);
            yield return new WaitForSeconds(lineDuration);
        }

        subtitleTextObject.SetActive(false);
    }

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

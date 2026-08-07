using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExplorationNode : MonoBehaviour
{
    [Header("Node Settings")]
    public string nodeID = "MainDesk";
    public Light deskLamp;              // Light component associated with this node

    [Header("Computer Screen Effects")]
    public TMP_Text screenText;         // TextMeshPro component on the computer screen
    public float textFlickerSpeed = 4f; // Speed of screen text blinking

    [Header("Inner Monologue Subtitles")]
    public GameObject subtitleTextObject; // UI Text Object for subtitles (e.g. Subtitle_Opening)
    public string[] monologueLines = new string[] {
        "That is weird...",
        "The signal is all gone..."
    };
    public float lineDuration = 3.0f;   // Display duration for each subtitle line

    private bool isNodeActive = false;  // Activated by PostPhoneTransition script
    private bool isVisited = false;

    private void Update()
    {
        // 1. Desk Lamp Flickering (Gently flickers before being visited)
        if (isNodeActive && !isVisited && deskLamp != null)
        {
            deskLamp.intensity = Mathf.PingPong(Time.time * 5f, 1.2f) + 0.4f;
        }

        // 2. Computer Screen Text Blinking (Continuously modulates alpha)
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

        // 1. Switch desk lamp to steady light
        if (deskLamp != null)
        {
            deskLamp.intensity = 2.5f;
        }

        // 2. Play the node's inner monologue subtitles
        StartCoroutine(PlayMonologueRoutine());

        Debug.Log($"[Exploration] Node Completed: {nodeID}");

        // 3. Notify StoryManager to record exploration progress
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnNodeVisited(nodeID);
        }
    }

    // Sequence to display subtitle lines one by one
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

    // Helper method to update subtitle text on UI
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

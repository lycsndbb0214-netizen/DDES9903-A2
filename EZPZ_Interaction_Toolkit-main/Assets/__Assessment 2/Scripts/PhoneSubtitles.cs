using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhoneSubtitles : MonoBehaviour
{
    [Header("UI Subtitle Object")]
    public GameObject subtitleTextObject; // Drag and drop Subtitle_Opening object here
    [Header("Audio Settings")]
    public AudioSource emergencyAudioSource;
    private bool hasPlayed = false;
    public void PlayPhoneSubtitles()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        if (emergencyAudioSource != null)
        {
            emergencyAudioSource.Play();
        }

        if (subtitleTextObject != null)
        {
            subtitleTextObject.SetActive(true);
            StopAllCoroutines(); // Prevent overlapping coroutines if re-triggered
            StartCoroutine(SubtitleSequence());
        }
    }

    private IEnumerator SubtitleSequence()
    {
        // 1. Audio 0.0s - 1.5: "Emergency..." followed by static noise
        UpdateText("Emergency...");
        yield return new WaitForSeconds(1.5f);

        // 2. Audio 1.5s - 3.0s: "Emerg..." cut off by static noise
        UpdateText("Emerg...");
        yield return new WaitForSeconds(1.5f);

        // 3. Audio 3.0s - 6.5s: "Iceberg approaching!"
        UpdateText("Iceberg... approaching...");
        yield return new WaitForSeconds(3.5f);

        // 4. Audio 6.5s - 10.5s: "Slow down..." followed by disconnect beep
        UpdateText("Slow... down...");
        yield return new WaitForSeconds(4.0f);

        //5. 10.5s - 12s "Disconnected"
        UpdateText("(No Signal)");
        yield return new WaitForSeconds(1.5f);

        // 5. Hide subtitle object after sequence completes
        subtitleTextObject.SetActive(false);
    }

    // Helper method to automatically assign text to Legacy UI or TextMeshPro components
    private void UpdateText(string content)
    {
        if (subtitleTextObject == null) return;

        // Try standard UI Text component
        Text legacyText = subtitleTextObject.GetComponent<Text>();
        if (legacyText != null)
        {
            legacyText.text = content;
            return;
        }

        // Try TextMeshPro component
        TMP_Text tmpText = subtitleTextObject.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = content;
        }
    }
}

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("Exploration Progress Settings")]
    public int totalNodesRequired = 3;
    private int visitedNodesCount = 0;

    [Header("Climax (C1) - Iceberg & Radar")]
    public GameObject icebergModel;          // Iceberg model (Set active on collision)
    public AudioSource radarAlarmAudioSource;// Radar alarm sound
    public AudioSource collisionAudioSource; // Massive collision sound

    [Header("Climax (C1) - Lights")]
    public Light[] allShipLights;            // Array of all ship lights (Flicker red on collision)
    public Light lifeboatGuideLight;         // Steady beacon light at lifeboat location (Off at start)

    [Header("Climax (C1) - Interactive Lifeboat")]
    public GameObject lifeboatProp;          // Lifeboat object on deck (Set active after collision)

    [Header("Climax (C1) - Inner Monologue Subtitles")]
    public GameObject subtitleTextObject;    // Subtitle UI GameObject (Text or TMP_Text)

    [Tooltip("Subtitle shown while radar alarm sounds during the 5-second wait")]
    public string alarmMonologue = "What is that sound?!!";

    [Tooltip("Subtitles shown sequentially after collision impact")]
    public string[] collisionMonologueLines = new string[]
    {
        "OH NO! The ship ran aground!",
        "We are sinking!",
        "I need to go to the lifeboat now!"
    };
    public float lineDuration = 2.5f;        // Duration for each monologue line in seconds

    [Header("Resolution (C2) - Ending UI")]
    public GameObject lifeboatEndUI;        // Final black screen or end UI
    public CanvasGroup fadeCanvasGroup;     // Screen fade canvas group

    private bool isClimaxStarted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (icebergModel != null) icebergModel.SetActive(false);
        if (lifeboatProp != null) lifeboatProp.SetActive(false);
        if (lifeboatGuideLight != null) lifeboatGuideLight.gameObject.SetActive(false);
        if (subtitleTextObject != null) subtitleTextObject.SetActive(false);
    }

    public void OnNodeVisited(string nodeID)
    {
        visitedNodesCount++;
        Debug.Log($"[StoryManager] Node visited: {nodeID}. Progress: {visitedNodesCount}/{totalNodesRequired}");
    }

    public bool AreAllNodesVisited()
    {
        return visitedNodesCount >= totalNodesRequired;
    }

    public void TriggerClimaxSequence()
    {
        if (isClimaxStarted) return;
        isClimaxStarted = true;
        StartCoroutine(TriggerStateC1_Climax());
    }

    private IEnumerator TriggerStateC1_Climax()
    {
        Debug.Log("[StoryManager] Climax Phase Started - Radar Alarm Sounding...");

        // 1. Play Radar Alarm Sound
        if (radarAlarmAudioSource != null)
        {
            radarAlarmAudioSource.Play();
        }

        // Show Stage 1 Subtitle during alarm
        if (subtitleTextObject != null && !string.IsNullOrEmpty(alarmMonologue))
        {
            subtitleTextObject.SetActive(true);
            UpdateSubtitleText(alarmMonologue);
        }

        // Wait 5 seconds for alarmÔ¤ÈÈ
        yield return new WaitForSeconds(5.0f);

        // Hide alarm subtitle before collision sequence starts
        if (subtitleTextObject != null)
        {
            subtitleTextObject.SetActive(false);
        }

        Debug.Log("[StoryManager] 5 Seconds Alarm Ended -> Massive Collision Impact!");

        // 2. Collision Impact: Iceberg, Audio, Ship Motion
        if (icebergModel != null) icebergModel.SetActive(true);
        if (collisionAudioSource != null) collisionAudioSource.Play();

        if (ShipMotion.Instance != null)
        {
            ShipMotion.Instance.TriggerCollisionAndTiltBackward();
        }

        // 3. Enable Lifeboat Prop and Guide Light
        if (lifeboatProp != null) lifeboatProp.SetActive(true);

        if (lifeboatGuideLight != null)
        {
            lifeboatGuideLight.gameObject.SetActive(true);
            lifeboatGuideLight.color = Color.yellow;
            lifeboatGuideLight.intensity = 4.0f;
        }

        // 4. Turn all ship lights RED and start flickering loop
        StartCoroutine(FlickerShipLights());

        // 5. Play Stage 2 Collision Monologue Subtitles
        StartCoroutine(PlayCollisionSubtitles());
    }

    private IEnumerator PlayCollisionSubtitles()
    {
        if (subtitleTextObject == null || collisionMonologueLines == null || collisionMonologueLines.Length == 0)
            yield break;

        subtitleTextObject.SetActive(true);

        foreach (string line in collisionMonologueLines)
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

    private IEnumerator FlickerShipLights()
    {
        while (isClimaxStarted)
        {
            float intensity = Mathf.PingPong(Time.time * 12f, 4.5f);
            if (allShipLights != null)
            {
                foreach (Light l in allShipLights)
                {
                    if (l != null && l != lifeboatGuideLight)
                    {
                        l.gameObject.SetActive(true);
                        l.color = Color.red;
                        l.intensity = intensity;
                    }
                }
            }
            yield return null;
        }
    }

    public void OnPlayerEnteredLifeboat()
    {
        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        if (fadeCanvasGroup != null)
        {
            float fadeTimer = 0f;
            while (fadeTimer < 2.0f)
            {
                fadeTimer += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(fadeTimer / 2.0f);
                yield return null;
            }
        }

        if (lifeboatEndUI != null) lifeboatEndUI.SetActive(true);
    }
}
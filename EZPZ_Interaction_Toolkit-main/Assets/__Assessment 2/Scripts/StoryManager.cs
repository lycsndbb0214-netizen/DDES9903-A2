using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Essential for UI Image manipulation

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("Exploration Progress Settings")]
    public int totalNodesRequired = 3;
    private int visitedNodesCount = 0;

    [Header("Climax (C1) - Iceberg & Radar")]
    public GameObject icebergModel;
    public AudioSource radarAlarmAudioSource;
    public AudioSource collisionAudioSource;

    [Header("Climax (C1) - Lights")]
    public Light[] allShipLights;
    public Light lifeboatGuideLight;

    [Header("Climax (C1) - Interactive Lifeboat")]
    public GameObject lifeboatProp;

    [Header("Climax (C1) - Inner Monologue Subtitles")]
    public GameObject subtitleTextObject;

    [Tooltip("Subtitle shown while radar alarm sounds during the 5-second wait")]
    public string alarmMonologue = "What is that sound?!!";

    [Tooltip("Subtitles shown sequentially after collision impact")]
    public string[] collisionMonologueLines = new string[]
    {
        "OH NO! The ship ran aground!",
        "We are sinking!",
        "I need to go to the lifeboat now!"
    };
    public float lineDuration = 3.0f;

    [Header("Resolution (C2) - Ending Fade UI")]
    [Tooltip("Assign the same BlackScreen Image used in the intro here")]
    public Image blackScreenImage;          // Swapped out CanvasGroup for direct Image manipulation
    public GameObject lifeboatEndUI;
    public float fadeDuration = 3.0f;

    private bool isClimaxStarted = false;
    private bool isCinematicStarted = false;
    private bool isFadingToBlack = false;

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
        if (lifeboatEndUI != null) lifeboatEndUI.SetActive(false);
    }

    public void OnNodeVisited(string nodeID)
    {
        visitedNodesCount++;
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
        if (radarAlarmAudioSource != null) radarAlarmAudioSource.Play();

        if (subtitleTextObject != null && !string.IsNullOrEmpty(alarmMonologue))
        {
            subtitleTextObject.SetActive(true);
            UpdateSubtitleText(alarmMonologue);
        }

        yield return new WaitForSeconds(5.0f);

        if (subtitleTextObject != null) subtitleTextObject.SetActive(false);

        if (icebergModel != null) icebergModel.SetActive(true);
        if (collisionAudioSource != null) collisionAudioSource.Play();

        if (ShipMotion.Instance != null)
        {
            ShipMotion.Instance.TriggerCollisionAndTiltBackward();
        }

        if (lifeboatProp != null) lifeboatProp.SetActive(true);

        if (lifeboatGuideLight != null)
        {
            lifeboatGuideLight.gameObject.SetActive(true);
            lifeboatGuideLight.color = Color.green;
            lifeboatGuideLight.intensity = 100.0f;
        }

        StartCoroutine(FlickerShipLights());
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
        if (legacyText != null) { legacyText.text = content; return; }
        TMP_Text tmpText = subtitleTextObject.GetComponent<TMP_Text>();
        if (tmpText != null) tmpText.text = content;
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

    public void StartCinematicEnding()
    {
        if (isCinematicStarted) return;
        isCinematicStarted = true;
        if (ShipMotion.Instance != null) ShipMotion.Instance.StartSinking();
    }

    // Triggered when SecondLifeboat reaches the stop threshold distance
    public void TriggerFadeToBlack()
    {
        if (isFadingToBlack) return;
        isFadingToBlack = true;
        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        Debug.Log("[StoryManager] Direct Image alpha transition initiated.");

        if (blackScreenImage != null)
        {
            // Ensure the black screen GameObject is reactivated (since IntroDirector set it inactive)
            blackScreenImage.gameObject.SetActive(true);

            float fadeTimer = 0f;
            Color currColor = Color.black; // Maintain solid black base color

            while (fadeTimer < fadeDuration)
            {
                fadeTimer += Time.deltaTime;
                // Linearly interpolate the color alpha channel from transparent (0) to opaque (1)
                currColor.a = Mathf.Clamp01(fadeTimer / fadeDuration);
                blackScreenImage.color = currColor;
                yield return null;
            }

            currColor.a = 1.0f;
            blackScreenImage.color = currColor;
        }

        if (lifeboatEndUI != null)
        {
            lifeboatEndUI.SetActive(true);
        }
    }

    public void OnPlayerEnteredLifeboat()
    {
        StartCinematicEnding();
    }
}
using System.Collections;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    [Header("Exploration Progress Settings")]
    public int totalNodesRequired = 3;
    private int visitedNodesCount = 0;

    [Header("Climax (C1) Settings")]
    public AudioSource climaxAudioSource; // Audio for 3 bell sounds + "Iceberg!" shout
    public Light redAlarmLight;           // Emergency flashing red alarm light
    public CanvasGroup fadeCanvasGroup;   // Screen fade out canvas group
    public Camera mainCamera;             // Camera for screen shake effect

    [Header("Resolution (C2) Settings")]
    public GameObject lifeboatEndUI;      // Resolution / Lifeboat end screen UI

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called when player interacts/completes an exploration node (B1, B2, B3)
    public void OnNodeVisited(string nodeID)
    {
        visitedNodesCount++;
        Debug.Log($"[StoryManager] Node visited: {nodeID}. Progress: {visitedNodesCount}/{totalNodesRequired}");

        // Trigger State C1 Climax when all required nodes are visited
        if (visitedNodesCount >= totalNodesRequired)
        {
            StartCoroutine(TriggerStateC1_Climax());
        }
    }

    // Sequence for State C1: Collision Climax
    private IEnumerator TriggerStateC1_Climax()
    {
        Debug.Log("[StoryManager] State C1 Triggered: Iceberg Collision!");

        // 1. Play collision audio (Bell sound & "Iceberg!" shout)
        if (climaxAudioSource != null)
        {
            climaxAudioSource.Play();
        }

        // 2. Enable emergency alarm light
        if (redAlarmLight != null)
        {
            redAlarmLight.gameObject.SetActive(true);
        }

        // 3. Screen shake & alarm light flashing loop
        float timer = 0f;
        float climaxDuration = 4.0f;
        Vector3 originalCamPos = mainCamera != null ? mainCamera.transform.localPosition : Vector3.zero;

        while (timer < climaxDuration)
        {
            timer += Time.deltaTime;

            // Red light flashing
            if (redAlarmLight != null)
            {
                redAlarmLight.intensity = Mathf.PingPong(Time.time * 10f, 4.0f);
            }

            // Camera shake
            if (mainCamera != null)
            {
                mainCamera.transform.localPosition = originalCamPos + (Vector3)Random.insideUnitCircle * 0.15f;
            }

            yield return null;
        }

        // Reset camera position
        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalCamPos;
        }

        // 4. Smooth screen fade out to black
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

        // 5. Trigger State C2 Resolution
        TriggerStateC2_Resolution();
    }

    // Sequence for State C2: Resolution / Lifeboat
    private void TriggerStateC2_Resolution()
    {
        Debug.Log("[StoryManager] State C2 Triggered: Lifeboat Resolution.");
        if (lifeboatEndUI != null)
        {
            lifeboatEndUI.SetActive(true);
        }
    }
}

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
    public AudioSource radarAlarmAudioSource;// Radar alarm sound effect
    public AudioSource collisionAudioSource; // Massive collision sound effect

    [Header("Climax (C1) - Visuals & Light")]
    public Light redAlarmLight;              // Red emergency flickering light
    public Camera mainCamera;               // Main camera for tremor effect
    public CanvasGroup fadeCanvasGroup;     // Screen fade out canvas group (Black scene)

    [Header("Resolution (C2) - Lifeboat")]
    public GameObject lifeboatEndUI;        // Lifeboat resolution UI / Scene overlay
    public AudioSource oceanWavesAudioSource;// Gentle ocean waves sound for lifeboat phase

    private bool isClimaxStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure iceberg is hidden at start
        if (icebergModel != null)
        {
            icebergModel.SetActive(false);
        }
    }

    // Called when player interacts with an exploration node
    public void OnNodeVisited(string nodeID)
    {
        visitedNodesCount++;
        Debug.Log($"[StoryManager] Node visited: {nodeID}. Progress: {visitedNodesCount}/{totalNodesRequired}");
    }

    // Verify all 3 nodes have been visited
    public bool AreAllNodesVisited()
    {
        return visitedNodesCount >= totalNodesRequired;
    }

    // Triggered by CabinClimaxTrigger when returning to cabin
    public void TriggerClimaxSequence()
    {
        if (isClimaxStarted) return;
        isClimaxStarted = true;
        StartCoroutine(TriggerStateC1_Climax());
    }

    // Sequence matching your flowchart: Radar Alarm -> Collision & Tilt -> Black Scene -> Lifeboat
    private IEnumerator TriggerStateC1_Climax()
    {
        Debug.Log("[StoryManager] Climax Phase Started!");

        // 1. Radar Alarm Sound
        if (radarAlarmAudioSource != null)
        {
            radarAlarmAudioSource.Play();
        }

        yield return new WaitForSeconds(1.0f); // Brief delay before impact

        // 2. Collision Event: Show Iceberg & Play Collision Sound
        if (icebergModel != null)
        {
            icebergModel.SetActive(true); // Show iceberg at ship bow
        }

        if (collisionAudioSource != null)
        {
            collisionAudioSource.Play();
        }

        // 3. Tilt ship backward and intensify rocking via ShipMotion
        if (ShipMotion.Instance != null)
        {
            ShipMotion.Instance.TriggerCollisionAndTiltBackward();
        }

        // 4. Enable Red Alarm Light
        if (redAlarmLight != null)
        {
            redAlarmLight.gameObject.SetActive(true);
        }

        // 5. Massive Tremor (Camera Shake) & Red Light Flickering
        float timer = 0f;
        float climaxDuration = 4.5f;
        Vector3 originalCamPos = mainCamera != null ? mainCamera.transform.localPosition : Vector3.zero;

        while (timer < climaxDuration)
        {
            timer += Time.deltaTime;

            // Red light flickering
            if (redAlarmLight != null)
            {
                redAlarmLight.intensity = Mathf.PingPong(Time.time * 12f, 4.5f);
            }

            // Massive camera tremor
            if (mainCamera != null)
            {
                mainCamera.transform.localPosition = originalCamPos + (Vector3)Random.insideUnitCircle * 0.2f;
            }

            yield return null;
        }

        // Reset camera position
        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalCamPos;
        }

        // 6. Black Scene Transition (Fade to Black)
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

        // 7. Transition to Resolution (Lifeboat Phase)
        TriggerStateC2_Resolution();
    }

    // Sequence for Resolution: Lifeboat Phase
    private void TriggerStateC2_Resolution()
    {
        Debug.Log("[StoryManager] Resolution Phase Started: Lifeboat.");

        // Stop radar alarm if still playing
        if (radarAlarmAudioSource != null && radarAlarmAudioSource.isPlaying)
        {
            radarAlarmAudioSource.Stop();
        }

        // Play gentle ocean wave sound
        if (oceanWavesAudioSource != null)
        {
            oceanWavesAudioSource.Play();
        }

        // Display Lifeboat Resolution UI
        if (lifeboatEndUI != null)
        {
            lifeboatEndUI.SetActive(true);
        }
    }
}
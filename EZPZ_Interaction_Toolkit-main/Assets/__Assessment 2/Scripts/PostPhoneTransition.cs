using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostPhoneTransition : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource thunderAudioSource; // Single initial thunderclap sound
    public AudioSource stormAudioSource;   // Continuous background storm ambient sound

    [Header("Lighting & Effects")]
    public Light lightningLight;           // Lightning light component
    public Light[] roomLights;             // Array of bridge room lights
    public float roomFlickerDuration = 3.0f; // Room flickering duration in seconds
    public ShipMotion shipMotion;         // Ship rocking motion component

    [Header("Periodic Lightning Settings")]
    public float minLightningInterval = 4.0f; // Minimum seconds between lightning flashes
    public float maxLightningInterval = 9.0f; // Maximum seconds between lightning flashes

    [Header("Inner Monologue Subtitles")]
    public GameObject monologueTextObject; // UI Text Object for subtitles

    [Header("Exploration Nodes to Activate")]
    public ExplorationNode[] explorationNodes; // Nodes to activate after room stops flickering

    private float[] originalIntensities;
    private bool hasTriggered = false;

    private void Start()
    {
        // Store original intensities of room lights
        if (roomLights != null && roomLights.Length > 0)
        {
            originalIntensities = new float[roomLights.Length];
            for (int i = 0; i < roomLights.Length; i++)
            {
                if (roomLights[i] != null)
                {
                    originalIntensities[i] = roomLights[i].intensity;
                }
            }
        }
    }

    public void TriggerStormTransition()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        StartCoroutine(StormSequence());
    }

    private IEnumerator StormSequence()
    {
        // 1. Play initial thunder sound ONCE
        if (thunderAudioSource != null)
        {
            thunderAudioSource.Play();
        }

        // 2. Start continuous periodic lightning flashes in background
        StartCoroutine(RepeatingLightningRoutine());

        // 3. Play continuous ambient storm sound and trigger ship rocking motion
        if (stormAudioSource != null)
        {
            stormAudioSource.loop = true;
            stormAudioSource.Play();
        }
        if (shipMotion != null)
        {
            shipMotion.TriggerStormMotion();
        }

        // 4. Play inner monologue subtitles concurrently
        StartCoroutine(PlayInnerMonologue());

        // 5. Room lights flicker for specified duration
        float timer = 0f;
        while (timer < roomFlickerDuration)
        {
            for (int i = 0; i < roomLights.Length; i++)
            {
                if (roomLights[i] != null)
                {
                    roomLights[i].intensity = Random.Range(0.2f, 1.8f);
                }
            }
            float waitTime = Random.Range(0.05f, 0.15f);
            timer += waitTime;
            yield return new WaitForSeconds(waitTime);
        }

        // 6. Restore original room light intensities
        for (int i = 0; i < roomLights.Length; i++)
        {
            if (roomLights[i] != null && originalIntensities != null && i < originalIntensities.Length)
            {
                roomLights[i].intensity = originalIntensities[i];
            }
        }

        // 7. Activate exploration nodes
        if (explorationNodes != null)
        {
            foreach (ExplorationNode node in explorationNodes)
            {
                if (node != null)
                {
                    node.ActivateNode();
                }
            }
        }
    }

    // Coroutine to flash lightning at random intervals periodically
    private IEnumerator RepeatingLightningRoutine()
    {
        while (true)
        {
            if (lightningLight != null)
            {
                StartCoroutine(FlashLightning());
            }

            // Wait for a random interval between min and max settings
            float randomWait = Random.Range(minLightningInterval, maxLightningInterval);
            yield return new WaitForSeconds(randomWait);
        }
    }

    // Single lightning double-flash effect
    private IEnumerator FlashLightning()
    {
        if (lightningLight == null)
        {
            Debug.LogWarning("[Lightning] Lightning Light slot is empty in Inspector!");
            yield break;
        }

        Debug.Log("[Lightning] Flash Triggered!");
        // 1. Ensure both the GameObject and Light component are active
        lightningLight.gameObject.SetActive(true);
        lightningLight.enabled = true;

        // First high intensity flash
        lightningLight.intensity = 10.0f;
        yield return new WaitForSeconds(0.08f);

        // Dim flash
        lightningLight.intensity = 0.8f;
        yield return new WaitForSeconds(0.05f);

        // Second high intensity flash
        lightningLight.intensity = 8.0f;
        yield return new WaitForSeconds(0.15f);

        // Turn off light component
        lightningLight.enabled = false;
    }

    // Coroutine for inner monologue subtitle sequence
    private IEnumerator PlayInnerMonologue()
    {
        if (monologueTextObject == null) yield break;

        monologueTextObject.SetActive(true);

        UpdateMonologueText("I need to check around this ship...");
        yield return new WaitForSeconds(3.0f);

        UpdateMonologueText("Hope everything is fine...");
        yield return new WaitForSeconds(3.0f);

        monologueTextObject.SetActive(false);
    }

    private void UpdateMonologueText(string content)
    {
        if (monologueTextObject == null) return;

        Text legacyText = monologueTextObject.GetComponent<Text>();
        if (legacyText != null)
        {
            legacyText.text = content;
            return;
        }

        TMP_Text tmpText = monologueTextObject.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = content;
        }
    }
}
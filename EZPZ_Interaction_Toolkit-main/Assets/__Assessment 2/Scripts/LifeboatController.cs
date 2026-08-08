using UnityEngine;

public class LifeboatController : MonoBehaviour
{
    [Header("Lifeboat Colliders")]
    public Collider playerEntryTrigger; // Trigger collider inside the lifeboat for player jump-in

    private bool hasHitWater = false; // Flag checking if lifeboat has hit the sea surface

    private void Start()
    {
        // Disable the player entry trigger until the lifeboat hits the sea water
        if (playerEntryTrigger != null)
        {
            playerEntryTrigger.enabled = false;
        }
    }

    // Detect when the lifeboat hits the sea water mesh / collider
    private void OnCollisionEnter(Collision collision)
    {
        CheckWaterContact(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if lifeboat enters water trigger zone
        CheckWaterContact(other.gameObject);

        // If lifeboat is already in the water, check if the player jumps inside
        if (hasHitWater && (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null))
        {
            Debug.Log("[Lifeboat] Player jumped inside the lifeboat! Triggering final resolution.");
            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.OnPlayerEnteredLifeboat();
            }
        }
    }

    private void CheckWaterContact(GameObject obj)
    {
        if (hasHitWater) return;

        // Check by tag or name
        if (obj.CompareTag("Water") || obj.name.ToLower().Contains("water") || obj.name.ToLower().Contains("sea"))
        {
            hasHitWater = true;
            Debug.Log("[Lifeboat] Lifeboat touched the sea! Player entry trigger activated.");

            // Enable player entry trigger
            if (playerEntryTrigger != null)
            {
                playerEntryTrigger.enabled = true;
            }
        }
    }
}

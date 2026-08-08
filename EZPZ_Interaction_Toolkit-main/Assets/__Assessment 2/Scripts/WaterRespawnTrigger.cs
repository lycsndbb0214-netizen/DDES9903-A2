using UnityEngine;

public class WaterRespawnTrigger : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform cabinRespawnPoint; // Assign SpawnPoint_Cabin here

    [Header("Ending Phase Settings")]
    public SecondLifeboat secondLifeboat; // Assign Lifeboat 2 (the new floating lifeboat) here

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering water is the Player or Main Camera
        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null)
        {
            RespawnPlayer(other.gameObject);
        }
    }

    private void RespawnPlayer(GameObject playerObj)
    {
        // 1. Determine target spawn point (Cabin vs Lifeboat)
        Transform targetSpawnPoint = cabinRespawnPoint;
        bool isEndingPhase = false;

        // If Lifeboat 2 is active on the sea, switch respawn target to the lifeboat
        if (secondLifeboat != null && secondLifeboat.gameObject.activeInHierarchy)
        {
            isEndingPhase = true;
            if (secondLifeboat.playerSeatPoint != null)
            {
                targetSpawnPoint = secondLifeboat.playerSeatPoint;
            }
        }

        if (targetSpawnPoint == null)
        {
            Debug.LogWarning("[WaterRespawn] Target Respawn Point is missing in Inspector!");
            return;
        }

        Transform targetTransform = playerObj.transform.root;

        // 2. Handle CharacterController (Must be disabled temporarily to allow teleportation)
        CharacterController cc = playerObj.GetComponentInParent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            cc.transform.position = targetSpawnPoint.position;
            cc.transform.rotation = targetSpawnPoint.rotation;
            cc.enabled = true;
            Debug.Log($"[WaterRespawn] Player respawned via CharacterController to {(isEndingPhase ? "Lifeboat" : "Cabin")}.");
        }
        else
        {
            // Fallback Transform relocation
            targetTransform.position = targetSpawnPoint.position;
            targetTransform.rotation = targetSpawnPoint.rotation;
            Debug.Log($"[WaterRespawn] Player fell into water and respawned at {(isEndingPhase ? "Lifeboat" : "Cabin")}.");
        }

        // 3. Handle Rigidbody physics reset (Clear falling velocity)
        Rigidbody rb = playerObj.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 4. If in ending phase, board player onto lifeboat to trigger cinematic movement
        if (isEndingPhase && secondLifeboat != null)
        {
            secondLifeboat.BoardPlayer(targetTransform);
        }
    }
}
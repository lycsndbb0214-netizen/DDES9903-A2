using UnityEngine;

public class WaterRespawnTrigger : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform cabinRespawnPoint; // Assign SpawnPoint_Cabin here

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
        if (cabinRespawnPoint == null)
        {
            Debug.LogWarning("[WaterRespawn] Cabin Respawn Point is missing in Inspector!");
            return;
        }

        // 1. Handle CharacterController (Must be disabled temporarily to allow teleportation)
        CharacterController cc = playerObj.GetComponentInParent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            cc.transform.position = cabinRespawnPoint.position;
            cc.transform.rotation = cabinRespawnPoint.rotation;
            cc.enabled = true;
            Debug.Log("[WaterRespawn] Player respawned via CharacterController.");
            return;
        }

        // 2. Handle Rigidbody physics reset (Clear falling velocity)
        Rigidbody rb = playerObj.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 3. Fallback Transform relocation
        Transform targetTransform = playerObj.transform.root;
        targetTransform.position = cabinRespawnPoint.position;
        targetTransform.rotation = cabinRespawnPoint.rotation;

        Debug.Log("[WaterRespawn] Player fell into water and respawned inside the cabin.");
    }
}
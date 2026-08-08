using UnityEngine;

public class FirstLifeboat : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Ensure it stays frozen on the wall at the start
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    // Call this method when player interacts/holds/drops the lifeboat
    public void EnablePhysics()
    {
        if (rb != null)
        {
            rb.isKinematic = false; // Enable gravity & physical movement
            rb.useGravity = true;
        }
    }
}

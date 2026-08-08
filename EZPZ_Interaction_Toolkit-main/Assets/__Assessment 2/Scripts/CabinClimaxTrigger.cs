using UnityEngine;

public class CabinClimaxTrigger : MonoBehaviour
{
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        // Check if player steps into trigger zone
        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null)
        {
            // Verify all 3 nodes have been visited
            if (StoryManager.Instance != null && StoryManager.Instance.AreAllNodesVisited())
            {
                isTriggered = true;
                Debug.Log("[CabinClimaxTrigger] Player returned to cabin with all 3 nodes completed. Starting Climax!");
                StoryManager.Instance.TriggerClimaxSequence();
            }
        }
    }
}
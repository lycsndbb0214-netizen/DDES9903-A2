using UnityEngine;

public class ExplorationNode : MonoBehaviour
{
    [Header("Node Settings")]
    public string nodeID = "MainDesk";
    public Light deskLamp;              // Light component associated with this node

    private bool isNodeActive = false;  // Disabled by default, activated after global room flickering finishes
    private bool isVisited = false;

    private void Update()
    {
        // Gently flickers only when activated by storm transition and not yet visited
        if (isNodeActive && !isVisited && deskLamp != null)
        {
            deskLamp.intensity = Mathf.PingPong(Time.time * 5f, 1.2f) + 0.4f;
        }
    }

    // Called by PostPhoneTransition script after the initial 3-second room flickering ends
    public void ActivateNode()
    {
        isNodeActive = true;
    }

    // Triggered when the player enters the node boundary
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null)
        {
            CompleteNode();
        }
    }

    public void CompleteNode()
    {
        if (isVisited) return;
        isVisited = true;

        // Visual Feedback: Switch desk lamp from flickering to solid steady light
        if (deskLamp != null)
        {
            deskLamp.intensity = 2.5f;
        }

        Debug.Log($"[Exploration] Node Completed: {nodeID}");

        // Notify StoryManager to update exploration progress
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnNodeVisited(nodeID);
        }
    }
}

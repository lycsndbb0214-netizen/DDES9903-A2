using UnityEngine;

public class ReceiverReset : MonoBehaviour
{
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Transform initialParent;

    private void Awake()
    {
        // Record the initial position and rotation relative to the phone base
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
        initialParent = transform.parent;
    }

    // Call this method when dropping or releasing the receiver
    public void ResetToCradle()
    {
        transform.SetParent(initialParent);
        transform.localPosition = initialLocalPosition;
        transform.localRotation = initialLocalRotation;
    }
}

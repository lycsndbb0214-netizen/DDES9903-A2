using UnityEngine;
using System.Collections;

public class IntroCutscene : MonoBehaviour
{
    [Header("Player Control Script Binding")]
    public MonoBehaviour playerController;
    [Header("Opening scene duration")]
    public float cutsceneDuration = 5.0f;

    void Start()
    {
        // 1. At the beginning, the player is locked in place (cannot move or turn their head).
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // 2. Countdown: Wait for cutsceneDuration seconds, then execute the UnlockPlayer function
        Invoke("UnlockPlayer", cutsceneDuration);
    }
    void UnlockPlayer()
    {
        // 3. Monologue ends, player control unlocked
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        Debug.Log("[Opening animation ends] Player has regained control!");
    }
}

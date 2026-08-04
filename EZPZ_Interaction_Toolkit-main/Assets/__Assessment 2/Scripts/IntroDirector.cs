using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroDirector : MonoBehaviour
{
    public Image blackScreen;
    public GameObject subtitleText;
    public float fadeDuration = 3.0f;

    void Start()
    {
        if (blackScreen != null)
        {
            blackScreen.color = Color.black;

            StartCoroutine(FadeInEyes());
        }

        Invoke("ShowOpeningSubtitle", 2.0f);
    }
    IEnumerator FadeInEyes()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            if (blackScreen != null)
            {
                blackScreen.color = new Color(0f, 0f, 0f, alpha);
            }
            yield return null;
        }

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(false);
        }
    }
    void ShowOpeningSubtitle()
    {
        Debug.Log("[Plot Trigger] A subtitle pops up on screen with an inner monologue: Who is calling?");

        if (subtitleText != null)
        {
            subtitleText.SetActive(true);
            // Let the subtitle automatically call the "HideOpeningSubtitle" function to hide after being displayed for seconds.
            Invoke("HideOpeningSubtitle", 5.0f);
        }
    }
    void HideOpeningSubtitle()
    {
        if (subtitleText != null)
        {
            subtitleText.SetActive(false);
            Debug.Log("[Plot Trigger] Subtitles have been automatically hidden.");
        }
    }
}

using UnityEngine;

public class PlaySoundOnHit : MonoBehaviour
{   
    private AudioSource audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      audioSource = GetComponent<AudioSource>();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("I Am Hit");

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        else
        {
            Debug.LogWarning("You Forgot To Put An AudioSource On And Assign a Clip");
        }
        
    }
}

using UnityEngine;

public class SpinOnBump : MonoBehaviour
{
    public Rigidbody subject;
    public float spinForce;
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
        subject.AddTorque(0, 0, spinForce);
        audioSource.Play();
    }
}

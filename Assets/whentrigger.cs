using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    // Reference to the audio source
    [SerializeField] private AudioSource audioSource;
    
    // The audio clip to play
    [SerializeField] private AudioClip audioClip;

    private void Start()
    {
        // If no audio source was assigned, try to get one from this game object
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            
            // If there's still no audio source, add one
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // If an audio clip was assigned, set it on the audio source
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
    }

    // This function is called when another collider enters this object's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        PlayAudio();
    }
    
    // This function is called when another collider makes contact with this object
    private void OnCollisionEnter(Collision collision)
    {
        PlayAudio();
    }
    
    // Play the audio
    private void PlayAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
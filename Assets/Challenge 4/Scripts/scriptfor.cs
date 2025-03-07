using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreTrigger : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int scoreIncrement = 1;
    [SerializeField] private float restartDelay = 0.5f;
    
    // Add audio source reference
    [SerializeField] private AudioClip goalSound;
    [SerializeField] private float volume = 1.0f;
    
    // Static score variable to persist between triggers
    private static int currentScore = 0;
    
    private void Start()
    {
        // Display the current score when the game starts
        UpdateScoreDisplay();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Score a goal whenever ANY object enters the trigger
        // Increase the score
        currentScore += scoreIncrement;
        
        // Update the score display
        UpdateScoreDisplay();
        
        // Play the goal sound
        PlayGoalSound();
        
        // Restart with delay
        if (restartDelay <= 0)
        {
            RestartGame();
        }
        else
        {
            Invoke("RestartGame", restartDelay);
        }
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }
    
    private void PlayGoalSound()
    {
        // Play sound if audio clip is assigned
        if (goalSound != null)
        {
            AudioSource.PlayClipAtPoint(goalSound, Camera.main.transform.position, volume);
        }
    }
    
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

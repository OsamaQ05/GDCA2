using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Challenge4
{
    public class OpponentScoreTrigger : MonoBehaviour
    {
        [SerializeField] private TMP_Text opponentScoreText;
        [SerializeField] private int scoreIncrement = 1;
        [SerializeField] private float restartDelay = 0.5f;
        
        // Add audio source reference
        [SerializeField] private AudioClip goalSound;
        [SerializeField] private float volume = 1.0f;
        
        // Static score variable to persist between triggers
        private static int opponentScore = 0;
        
        // Reference to the ball object (assign in inspector)
        [SerializeField] private GameObject ballObject;
        
        // Flag to prevent multiple scoring
        private bool hasScored = false;
        
        private void Start()
        {
            // Display the current score when the game starts
            UpdateScoreDisplay();
            // Reset the scoring flag on start
            hasScored = false;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Prevent multiple scoring
            if (hasScored)
                return;
                
            // Check if this is the ball by direct reference
            if (ballObject != null && other.gameObject == ballObject)
            {
                ScoreBall(other.gameObject);
            }
            // Alternatively, check by name if you prefer
            else if (other.gameObject.name.Contains("Ball") || other.gameObject.name.Contains("ball"))
            {
                ScoreBall(other.gameObject);
            }
        }
        
        private void ScoreBall(GameObject ball)
        {
            // Set the flag to prevent multiple scoring
            hasScored = true;
            
            Debug.Log("Opponent Score Triggered by: " + ball.name);
            
            // Increase the opponent score
            opponentScore += scoreIncrement;
            
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
            if (opponentScoreText != null)
            {
                opponentScoreText.text = "Opponent: " + opponentScore;
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 500f;
    private float turboBoost = 10f;
    public ParticleSystem effect;

    public static bool hasPowerup; 
    public static bool hasSmashPowerup;
    public static bool hasSpeedPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;
    public int count = 0;
    private float normalStrength = 10f;
    private float powerupStrength = 25f;

    private GameObject enemyGoal;
    public float goalInfluenceStrength =0.5f;
    public float goalPoweredInflunceStrength =0.85f;
    public static int scoreMultiplier=1;
    
    public float jumpForce = 5f;
    public ParticleSystem jumpEffect;
    private bool isGrounded;
    private bool wasInAir = false; // New: track if player was in the air
    public float groundCheckDistance = 1.1f;
    public LayerMask groundLayer;

    // Shockwave parameters
    public float shockwaveRadius = 20f;
    public float shockwaveForce = 40f;
    public ParticleSystem landingEffect; 
    GameObject focalPoint;

    public GameObject pauseMenuUI; // Drag Pause Menu UI here in Inspector
    public GameObject gameManager; // Drag GameManager here in Inspector

    private bool isPaused = false;
    private PauseMenu pauseMenu;

    public Transform cameraTransform; // Reference to the camera

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        enemyGoal = GameObject.Find("Enemy Goal");
        cameraTransform = Camera.main.transform; // Get main camera's transform
        focalPoint=GameObject.Find("Enemy Goal");
        if (gameManager != null)
            pauseMenu = gameManager.GetComponent<PauseMenu>(); // Get PauseMenu from GameManager

    }
    
    void Update()
    {
        // Ground checking logic
        bool wasGrounded = isGrounded;
        CheckGrounded();
        
        if (!wasGrounded && isGrounded && wasInAir)
        {
            OnPlayerLanding();
            wasInAir = false;
        }
        
        if (!isGrounded)
        {
            wasInAir = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            count++;

            if (count%2 == 1){
                pauseMenuUI.SetActive(true);
                pauseMenu.ResumeGame(); // Call ResumeGame from PauseMenu
            }
            else{
                pauseMenuUI.SetActive(false);
                PauseGame();
            }
        }

        // Camera-based movement
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 moveDirection = (cameraTransform.forward * verticalInput + cameraTransform.right * horizontalInput).normalized;
        moveDirection.y = 0;

        playerRb.AddForce(moveDirection * speed * Time.deltaTime, ForceMode.Force);

        // Speed powerup turbo boost
        if (hasSpeedPowerup && Input.GetKeyDown(KeyCode.Space))
        {
            playerRb.AddForce(moveDirection * turboBoost, ForceMode.Impulse);
            effect.Play();
        }

        // Smash powerup jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && hasSmashPowerup)
            {
                Debug.Log("Jumping!");
                Jump();
            }
        }

        powerupIndicator.transform.position = transform.position + new Vector3(0, 0.6f, 0);
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; // Freeze physics updates
        pauseMenuUI.SetActive(true); // Show pause menu
    }
    
    private void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
            Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.green);
        }
        else
        {
            isGrounded = false;
            Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
        }
    }
    
    // Jump method
    private void Jump()
    {
        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);

        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        if (jumpEffect != null) {
            jumpEffect.Play();
        }
    }
    
    // New method: triggered when player lands after being in the air
    private void OnPlayerLanding()
    {
        Debug.Log("Player landed! Creating shockwave");
        CreateShockwave();
        
        // Play landing effect if available
        if (landingEffect != null) {
            landingEffect.Play();
        }
    }
    
    // Creates a shockwave that pushes nearby objects away
    private void CreateShockwave()
    {
        // Only create shockwave if player has jump powerup
        if (!hasSmashPowerup) return;
        
        Debug.Log("Creating shockwave with radius: " + shockwaveRadius);
        
        // Find all colliders within the shockwave radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, shockwaveRadius);
        
        // Push objects with Rigidbody away from player
        foreach (Collider hitCollider in hitColliders)
        {
            // Skip the player itself
            if (hitCollider.gameObject == gameObject)
                continue;
                
            // Try to get a rigidbody from the hit object
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            
            // If it has a rigidbody and is an enemy, apply force away from the player
            if (rb != null && hitCollider.CompareTag("Enemy"))
            {
                // Calculate direction away from player (mostly horizontal)
                Vector3 rawDirection = (hitCollider.transform.position - transform.position);
                Vector3 direction = new Vector3(rawDirection.x, 0.1f, rawDirection.z).normalized;
                
                // Force decreases with distance
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float forceMultiplier = 1f - (distance / shockwaveRadius);
                
                // Apply force based on powerup
                float finalForce = shockwaveForce * forceMultiplier;
                
                // Point toward enemy goal for better gameplay
                Vector3 directionToGoal = (enemyGoal.transform.position - hitCollider.transform.position).normalized;
                Vector3 shootDirection = Vector3.Lerp(direction, directionToGoal, goalInfluenceStrength);
                
                // Apply the force
                rb.AddForce(shootDirection * finalForce, ForceMode.Impulse);
                
                // Small upward force for visual effect
                rb.AddForce(Vector3.up * 5f * forceMultiplier, ForceMode.Impulse);
                
                // Set multiplier for scoring
             
            }
        }
    }


    // Powerup collection
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
            scoreMultiplier=GenerateMultiplier();
        }
        else if (  other.gameObject.CompareTag("Smash Powerup")){
             Destroy(other.gameObject);
            hasSmashPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
            scoreMultiplier=GenerateMultiplier();
        }
        else if (other.gameObject.CompareTag("Speed Powerup")){
            Destroy(other.gameObject);
            hasSpeedPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
            scoreMultiplier=GenerateMultiplier();
        }
    }

    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        hasSmashPowerup= false;
        powerupIndicator.SetActive(false);
        scoreMultiplier=1;
    }

    // Collision with enemies
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = -(transform.position - other.gameObject.transform.position).normalized;
            Vector3 directionToGoal = (enemyGoal.transform.position - other.gameObject.transform.position).normalized;

            if (hasPowerup)
            {
                Vector3 shootDirection = Vector3.Lerp(awayFromPlayer, directionToGoal, goalPoweredInflunceStrength);
                enemyRigidbody.AddForce(shootDirection * powerupStrength, ForceMode.Impulse);
            }

            else // if no powerup, hit enemy with normal strength 
            {
                Vector3 shootDirection = Vector3.Lerp(awayFromPlayer, directionToGoal, goalInfluenceStrength);
                enemyRigidbody.AddForce(shootDirection * normalStrength, ForceMode.Impulse);
            }
        }
    }
    int GenerateMultiplier(){
        int []array= {1,1,1,1,1,2,2,2,2,3,3};
        return array[Random.Range(0,array.Length)];
    }



}
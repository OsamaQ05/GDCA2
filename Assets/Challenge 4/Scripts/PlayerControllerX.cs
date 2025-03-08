using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500;
    private GameObject focalPoint;
    private float turboBoost=10f; //added those two
    public ParticleSystem effect;

    public bool hasPowerup; 
    public bool hasSmashPowerup;
    public bool hasJumpPowerup; // New flag for jump powerup
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;
    
    // Jump powerup parameters
    public float jumpForce = 15f; // Increased from 10 to 15
    public ParticleSystem jumpEffect; // Optional particle effect for jump
    private bool isGrounded;
    public float groundCheckDistance = 1.1f; // Increased from 0.5 to 1.1
    public LayerMask groundLayer; // Add this to your inspector to specify what counts as ground

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup

    private GameObject enemyGoal;
    public float goalInfluenceStrength =0.5f;
    public float goalPoweredInflunceStrength =0.85f;
    public static int scoreMultiplier=1;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        enemyGoal= GameObject.Find("Enemy Goal");
        
        // If no ground layer is set, default to everything
        if (groundLayer.value == 0)
            groundLayer = ~0;
    }

    void Update()
    {
        // Improved ground check with debug visualization
        CheckGrounded();
        
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, 0.6f, 0);
        
        // Turbo boost (sprint) with Space key
        if (Input.GetKeyDown(KeyCode.Space)){
            // Normal turbo boost behavior
            playerRb.AddForce(focalPoint.transform.forward * turboBoost, ForceMode.Impulse);
            effect.Play();
        }
        
        // Jump with F key when jump powerup is active
        if (Input.GetKeyDown(KeyCode.F)){
            if (hasJumpPowerup) {
                if (isGrounded) {
                    Jump();
                    Debug.Log("JUMP EXECUTED!");
                } else {
                    Debug.Log("Can't jump - not grounded!");
                }
            } else {
                Debug.Log("Can't jump - no jump powerup!");
            }
        }
    }
    
    // Improved ground check with visualization
    private void CheckGrounded()
    {
        // Cast a ray straight down from the player's position
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
    
    // New jump method with debug info and shockwave effect
    private void Jump()
    {
        Debug.Log("Applying jump force: " + jumpForce);
        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z); // Reset any downward velocity
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        // Create a shockwave that pushes nearby objects away
        CreateJumpShockwave();
        
        if (jumpEffect != null) {
            jumpEffect.Play();
        }
    }
    
    // Creates a shockwave that pushes nearby objects away
    private void CreateJumpShockwave()
    {
        // Define the radius of the shockwave - moderate range
        float shockwaveRadius = 20f;
        // Define the force of the shockwave - reduced for better gameplay
        float shockwaveForce = 40f;
        
        Debug.Log("Creating balanced shockwave with radius: " + shockwaveRadius);
        
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
            
            // If it has a rigidbody, apply force away from the player
            if (rb != null)
            {
                // Calculate direction away from player (mostly horizontal)
                Vector3 rawDirection = (hitCollider.transform.position - transform.position);
                Vector3 direction = new Vector3(rawDirection.x, 0.1f, rawDirection.z).normalized;
                
                // Force decreases with distance
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float forceMultiplier = 1f - (distance / shockwaveRadius);
                
                // Apply moderate force
                rb.AddForce(direction * shockwaveForce * forceMultiplier, ForceMode.Impulse);
                
                // Small upward force for visual effect
                rb.AddForce(Vector3.up * 5f * forceMultiplier, ForceMode.Impulse);
            }
        }
    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
        else if (other.gameObject.CompareTag("Smash Powerup")){
            Destroy(other.gameObject);
            hasSmashPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
        else if (other.gameObject.CompareTag("Jump Powerup")){ // New powerup type
            Destroy(other.gameObject);
            hasJumpPowerup = true;
            Debug.Log("JUMP POWERUP ACTIVATED!");
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
    }

    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        hasSmashPowerup = false;
        
        if (hasJumpPowerup) {
            Debug.Log("JUMP POWERUP DEACTIVATED");
            hasJumpPowerup = false;
        }
        
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = - (transform.position - other.gameObject.transform.position).normalized; 
            Vector3 directionToGoal = (enemyGoal.transform.position - other.gameObject.transform.position).normalized;

           
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                Vector3 shootDirection= Vector3.Lerp(awayFromPlayer,directionToGoal,goalPoweredInflunceStrength);
                enemyRigidbody.AddForce(shootDirection * powerupStrength, ForceMode.Impulse);
                scoreMultiplier= GenerateMultiplier();
            }
            else if (hasSmashPowerup){
                Vector3 shootDirection= Vector3.Lerp(awayFromPlayer,directionToGoal,goalInfluenceStrength);
                enemyRigidbody.AddForce(shootDirection * normalStrength, ForceMode.Impulse);
                scoreMultiplier= GenerateMultiplier();
            }
            else if (hasJumpPowerup && !isGrounded) { // Special case: if we have jump powerup and are in the air
                // Enemy gets pushed down when we land on them
                Vector3 downForce = Vector3.down * powerupStrength;
                enemyRigidbody.AddForce(downForce, ForceMode.Impulse);
                
                // Give player a small bounce
                playerRb.AddForce(Vector3.up * jumpForce/2, ForceMode.Impulse);
                scoreMultiplier = GenerateMultiplier();
            }
            else // if no powerup, hit enemy with normal strength 
            {
                Vector3 shootDirection= Vector3.Lerp(awayFromPlayer,directionToGoal,goalInfluenceStrength);
                enemyRigidbody.AddForce(shootDirection * normalStrength, ForceMode.Impulse);
                scoreMultiplier=1;
            }
        }
    }
    
    int GenerateMultiplier(){
        int []array= {1,1,1,1,1,2,2,2,2,3,3};
        return array[Random.Range(0,array.Length)];
    }
    
    // Optional: visualize the ground check in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
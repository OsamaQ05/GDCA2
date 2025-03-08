using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500f;
    private float turboBoost = 10f;
    public ParticleSystem effect;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10f;
    private float powerupStrength = 25f;

    private GameObject enemyGoal;
    public float goalInfluenceStrength = 0.5f;
    public float goalPoweredInflunceStrength = 0.85f;

    public Transform cameraTransform; // Reference to the camera

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        enemyGoal = GameObject.Find("Enemy Goal");
        cameraTransform = Camera.main.transform; // Get main camera's transform
    }

    void Update()
    {
        // Get movement input
        float verticalInput = Input.GetAxis("Vertical");   // W/S
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D

        // Calculate movement direction relative to the camera
        Vector3 moveDirection = (cameraTransform.forward * verticalInput + cameraTransform.right * horizontalInput).normalized;
        moveDirection.y = 0; // Prevent movement from affecting the Y-axis

        // Apply force in the calculated direction
        playerRb.AddForce(moveDirection * speed * Time.deltaTime, ForceMode.Force);

        // Jump and smash down
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, 10f, playerRb.velocity.z); // Jump
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, -20f, playerRb.velocity.z); // Smash down
        }

        // Keep powerup indicator following the player
        powerupIndicator.transform.position = transform.position + new Vector3(0, 0.6f, 0);
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
        }
    }

    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
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
            else
            {
                Vector3 shootDirection = Vector3.Lerp(awayFromPlayer, directionToGoal, goalInfluenceStrength);
                enemyRigidbody.AddForce(shootDirection * normalStrength, ForceMode.Impulse);
            }
        }
    }
}

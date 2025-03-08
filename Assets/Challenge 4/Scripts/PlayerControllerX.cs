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

    public static bool hasPowerup; 
    public static bool hasSmashPowerup;
    public static bool hasSpeedPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

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
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, 0.6f, 0);
        if (hasSpeedPowerup){
             if (Input.GetKeyDown(KeyCode.Space)){
            playerRb.AddForce(focalPoint.transform.forward* turboBoost,ForceMode.Impulse);
            effect.Play();
        }

        }
        else if (hasSmashPowerup){
            // writre code heer hasooooona :) 
            // u might include thiws if (Input.GetKeyDown(KeyCode.Space)) 
            // compareTag("Enemy")
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
        else if (  other.gameObject.CompareTag("Smash Powerup")){
             Destroy(other.gameObject);
            hasSmashPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
        else if (other.gameObject.CompareTag("Speed Powerup")){
            Destroy(other.gameObject);
            hasSpeedPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
    }

    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        hasSmashPowerup= false;
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



}
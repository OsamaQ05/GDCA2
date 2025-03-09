using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.SimpleMenu; // For menu functionality

public class SpawnManagerX : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject opponentPrefab;
    public GameObject[] powerupPrefabs;
    public GameObject goalkeeperPrefab;
    
    // Audio sources for win and lose sounds
    public AudioSource winAudio;
    public AudioSource loseAudio;
    public GameObject mainMenuPrefab; // Reference to the main menu

    private float spawnRangeX = 38f;
    private float spawnZMin = -5f;
    private float spawnZMax = -35f;
    private float spawnY = 41f;

    public int enemyCount;
    public int waveCount = 1;
    public int enemySpeed= 25;
    private TimerX timer;
    public int maxWaves=4;
    public static string UItext;
    public GameObject powerupIndicator;

    public GameObject player; 
    public SM_OptionList difficultySelector; // Reference to the difficulty selector

    private int numAttackers; // Store the number of attackers based on difficulty
    private float attackForceMultiplier = 0.01f; // Reduce attack force

    void Start()
    {
        SetDifficulty(); // Initialize difficulty settings
    }

    void Start(){
        timer= FindObjectOfType<TimerX>();
    }

    // Update is called once per frame
    void Update()
{
    if (waveCount <= maxWaves)
    { 
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (!timer.running)
        {
            TimeUp();
        }
        else if (enemyCount == 0)
        {
           
                UItext = "Round " + waveCount; // Show last round before ending
                SpawnEnemyWave(waveCount);
                timer.ResetTimer();
        }
    }
    
    else if (waveCount > maxWaves) // Check win/loss AFTER max rounds
    {
        // Ensure enemies won't spawn until the main menu is closed
        if (mainMenuPrefab.activeSelf)
        {
            return; // Skip spawning if the main menu is still active
        }

        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount!=0){
            UItext = "Final Round";
        }
        else if (enemyCount==0){
            GameObject[] opponents = GameObject.FindGameObjectsWithTag("Opponent");
            GameObject[] keepers = GameObject.FindGameObjectsWithTag("Goalkeeper");
            // Destroy all opponents
            foreach (GameObject opponent in opponents)
            {
                Destroy(opponent);
            }

            // Destroy all goalkeepers
            foreach (GameObject keeper in keepers)
            {
                Destroy(keeper);
}

            if (EnemyX.playerScore > EnemyX.enemyScore)
            {
              
                UItext = "You Win!";
                // Play winning audio
                if (winAudio != null && !winAudio.isPlaying)
                {
                    winAudio.Play();
                }
            }
            else if (EnemyX.playerScore < EnemyX.enemyScore)
            {
                UItext = "You lose :(";
                // Play losing audio
                if (loseAudio != null && !loseAudio.isPlaying)
                {
                    loseAudio.Play();
                }
            }
            else {
                UItext = "Draw";
            }
        }
        
        if (enemyCount == 0)
        {
            SpawnEnemyWave(waveCount); // Spawn the next wave of enemies
        }
    }
}


    void SetDifficulty()
    {
        if (difficultySelector == null) return;

        // Set the number of attackers based on the selected difficulty
        switch (difficultySelector.currentOption)
        {
            case "Easy":
                numAttackers = 1; // 1 attacker
                break;
            case "Normal":
                numAttackers = 2; // 2 attackers
                break;
            case "Hard":
                numAttackers = 3; // 3 attackers
                break;
            default:
                numAttackers = 1; // Default to 1 attacker if something goes wrong
                break;
        }
    }

    Vector3 GenerateSpawnPosition()
    {
        float xPos = Random.Range(-spawnRangeX, spawnRangeX);
        float zPos = Random.Range(spawnZMin, spawnZMax);
        return new Vector3(xPos, spawnY, zPos);
    }

    void SpawnEnemyWave(int enemiesToSpawn)

    {
        PlayerControllerX.hasSmashPowerup=false;
        PlayerControllerX.hasSpeedPowerup=false;
        PlayerControllerX.hasPowerup=false;
        powerupIndicator.SetActive(false);
        Vector3 powerupSpawnOffset = new Vector3(-1, 29, 16); // make powerups spawn at player end
        if (GameObject.FindGameObjectsWithTag("Powerup").Length + GameObject.FindGameObjectsWithTag("Smash Powerup").Length+GameObject.FindGameObjectsWithTag("Speed Powerup").Length==0)
        SetDifficulty(); // Ensure the difficulty is set every time a new wave spawns

        // Log the wave and how many attackers and enemies will be spawned
        Debug.Log($"Wave {waveCount}: Spawning {numAttackers} attackers, 1 goalkeeper, and {enemiesToSpawn} enemies.");

        Vector3 powerupSpawnOffset = new Vector3(-1, 29, 16);

        // Check if any powerups are present, if none spawn a new one
        if (GameObject.FindGameObjectsWithTag("Powerup").Length + GameObject.FindGameObjectsWithTag("Smash Powerup").Length == 0)
        {
            int randomPowerup = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomPowerup], powerupSpawnOffset, powerupPrefabs[randomPowerup].transform.rotation);
        }

        // Always spawn a goalkeeper
        Instantiate(goalkeeperPrefab, new Vector3(0, 41, -40), goalkeeperPrefab.transform.rotation);
        Debug.Log("Spawned 1 Goalkeeper");

        // Spawn attackers based on the difficulty
        for (int i = 0; i < numAttackers; i++)
        {
            GameObject attacker = Instantiate(opponentPrefab, GenerateSpawnPosition(), opponentPrefab.transform.rotation);
            Debug.Log($"Spawned Attacker {i + 1}");

            Rigidbody rb = attacker.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = 1.5f; // Reduce mass for weaker hits
                rb.linearDamping = 2f; // Increase drag to slow them down
                rb.angularDamping = 2f; // Prevent excessive spinning
                rb.linearVelocity *= 0.2f; // Reduce movement speed
            }
        }

        // Spawn number of enemy balls based on wave number
        if (waveCount==4){
            for (int i = 0; i < 3; i++)
            {
                Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
            }
            
        }
        else {
            for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }}
        
     
        Debug.Log("yes");
        
        enemySpeed+=5;
        waveCount++;
        ResetPlayerPosition(); // put player back at start
        ResetOpponentPosition ();
        ResetGoalkeeperPosition ();
            Debug.Log($"Spawned Enemy {i + 1}");
        }

        waveCount++; // Increase wave count
        enemySpeed += 5; // Increase enemy speed for the next wave
        ResetPlayerPosition();
        ResetOpponentPosition();
        ResetGoalkeeperPosition();
    }

    // Reset player's position to the start point
    void ResetPlayerPosition()
    {
        player.transform.position = new Vector3(-2, 41, 25);
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    // Reset opponent's position (including all enemy objects)
    void ResetOpponentPosition()
    {
        foreach (GameObject opponent in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            opponent.transform.position = GenerateSpawnPosition();
            Rigidbody rb = opponent.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    // Reset goalkeeper's position
    void ResetGoalkeeperPosition()
    {
        goalkeeperPrefab.transform.position = new Vector3(0, 41, -40);
        Rigidbody rb = goalkeeperPrefab.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
     void TimeUp()
    {
        // Destroy all existing enemies
        if (waveCount<=maxWaves){ 
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject enemy in enemies)
            {
                Destroy(enemy);
            }

            ResetPlayerPosition();
            ResetOpponentPosition();
            ResetGoalkeeperPosition();
            SpawnEnemyWave(++waveCount);
            timer.ResetTimer();}
       
    }


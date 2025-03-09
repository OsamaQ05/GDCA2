using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.SimpleMenu; // For menu functionality

public class SpawnManagerX : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject opponentPrefab;
    public GameObject ezopponentPrefab;
    public GameObject midopponentPrefab;
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
        timer = FindObjectOfType<TimerX>();
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
        difficultySelector.currentOption;
        Vector3 powerupSpawnOffset = new Vector3(-1, 29, 16); // make powerups spawn at player end
        if (GameObject.FindGameObjectsWithTag("Powerup").Length + GameObject.FindGameObjectsWithTag("Smash Powerup").Length+GameObject.FindGameObjectsWithTag("Speed Powerup").Length==0)
        {
            int randomPowerup = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomPowerup], powerupSpawnOffset, powerupPrefabs[randomPowerup].transform.rotation);
        }
 
        // If no powerups remain, spawn a powerup
       
 
        if (waveCount==2){
            if (difficultySelector.currentOption.Equals("Hard")){
            Instantiate(opponentPrefab, GenerateSpawnPosition(), opponentPrefab.transform.rotation);}
            else if (difficultySelector.currentOption.Equals("Midium")){
                Instantiate(midopponentPrefab, GenerateSpawnPosition(), opponentPrefab.transform.rotation);
            }
            else if (difficultySelector.currentOption.Equals("Small")){
                Instantiate(ezopponentPrefab, GenerateSpawnPosition(), opponentPrefab.transform.rotation);

            }
        }
         if (waveCount==3){
            if ( difficultySelector.currentOption.Equals("Medium")|| difficultySelector.currentOption.Equals("Hard")){
                  Instantiate(goalkeeperPrefab,new Vector3(0,41,-40), goalkeeperPrefab.transform.rotation);

            }
          
        }
 
        // Spawn number of enemy balls based on wave number
        if (waveCount==4){
            if ( difficultySelector.currentOption.Equals("Hard")){
                 Instantiate(opponentPrefab, GenerateSpawnPosition(), opponentPrefab.transform.rotation);

            }
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

}
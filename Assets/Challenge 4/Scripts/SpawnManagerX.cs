using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuantumTek.SimpleMenu; // For menu functionality

public class SpawnManagerX : MonoBehaviour
{
    public GameObject enemyPrefab; //enemy is the football. Opponenets are defenders against player
    public GameObject opponentPrefabHard;
    public GameObject opponentPrefabNormal;
    public GameObject opponentPrefabEasy;
    public GameObject[] powerupPrefabs;
    public GameObject goalkeeperPrefab;
    public GameObject mainMenuPrefab; // Reference to the main menu
    private GameObject opponentPrefab; // This will store the selected opponent type
    public static string UItext;
    private string dif;
    private TimerX timer;
    public int maxWaves=4;
    public GameObject powerupIndicator;


    
    private float spawnRangeX = 38f;
    private float spawnZMin = -5f;
    private float spawnZMax = -35f;
    private float spawnY = 41f;

    public int enemyCount;
    public int waveCount = 1;
    public int enemySpeed = 25;

    public GameObject player; 
    public SM_OptionList difficultySelector; // Reference to the difficulty selector

    private int numAttackers; // Store the number of attackers based on difficulty
    private float attackForceMultiplier = 0.01f; // Reduce attack force

    void Start()
    {
        SetDifficulty(); // Initialize difficulty settings
        timer= FindObjectOfType<TimerX>();
        
    }

    void Update()
    {
        // Ensure enemies won't spawn until the main menu is closed
        if (mainMenuPrefab.activeSelf)
        {
            return; // Skip spawning if the main menu is still active
        }

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
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount!=0){
            UItext = "Final Round";
        }
        else if (enemyCount==0){
            des();
            if (EnemyX.playerScore > EnemyX.enemyScore)
            {
                UItext = "You Win!";
                // Winning audio 
            }
            else
            {
                UItext = "You lose :(";
                // loosing audsio 
            }
        }
        
    }
    }

    void SetDifficulty()
    {
        if (difficultySelector == null) return;

        // Assign the correct opponent prefab based on difficulty
        switch (difficultySelector.currentOption)
        {
            case "Easy":
                //numAttackers = 1;
                dif="Easy";
                opponentPrefab = opponentPrefabEasy; // Assign easy opponent prefab
                enemySpeed = 20; // Lower speed
                break;
            case "Normal":
                //numAttackers = 1;
                dif="Medium";
                opponentPrefab = opponentPrefabNormal; // Assign normal opponent prefab
                enemySpeed = 30; // Medium speed
                break;
            case "Hard":
                //numAttackers = 1;
                dif="Hard";
                opponentPrefab = opponentPrefabHard; // Assign hard opponent prefab
                enemySpeed = 200; // Higher speed
                break;
            default:
                numAttackers = 1;
                opponentPrefab = opponentPrefabEasy; // Default to easy if error occurs
                enemySpeed = 20;
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
        SetDifficulty();
    
        PlayerControllerX.hasSmashPowerup=false;
        PlayerControllerX.hasSpeedPowerup=false;
        PlayerControllerX.hasPowerup=false;
        powerupIndicator.SetActive(false);
        Vector3 powerupSpawnOffset = new Vector3(-1, 29, 16); // make powerups spawn at player end
        if (GameObject.FindGameObjectsWithTag("Powerup").Length + GameObject.FindGameObjectsWithTag("Smash Powerup").Length+GameObject.FindGameObjectsWithTag("Speed Powerup").Length==0)
        {
            int randomPowerup = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomPowerup], powerupSpawnOffset, powerupPrefabs[randomPowerup].transform.rotation);
        }


        // Always spawn a goalkeeper
        if (waveCount==3){
            if ( dif.Equals("Medium")|| dif.Equals("Hard")){
                  Instantiate(goalkeeperPrefab,new Vector3(0,32,-40), goalkeeperPrefab.transform.rotation);
            }
        }

        if (waveCount==2){
             Instantiate(opponentPrefab, GenerateSpawnPosition(), opponentPrefab.transform.rotation);}
        
        if (waveCount==3 && dif.Equals("Hard")){
             Instantiate(opponentPrefab, GenerateSpawnPosition(), opponentPrefab.transform.rotation);}
        

          for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
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
        foreach (GameObject opponent in GameObject.FindGameObjectsWithTag("Opponent"))
        {
            opponent.transform.position = GenerateSpawnPosition();
            Rigidbody rb = opponent.GetComponent<Rigidbody>();

        }
    }

    // Reset goalkeeper's position
    void ResetGoalkeeperPosition()
    {
        goalkeeperPrefab.transform.position = new Vector3(0, 41, -40);
        Rigidbody rb = goalkeeperPrefab.GetComponent<Rigidbody>();

       
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
    
    public void des(){
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

    }

}

  
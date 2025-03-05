using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerX : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;
    public GameObject opponentPrefab;

    private float spawnRangeX = 38f;
    private float spawnZMin = -5f; // set min spawn Z
    private float spawnZMax = -35f; // set max spawn Z
    private float spawnY=41f;

    public int enemyCount;
    public int waveCount = 1;


    public GameObject player; 

    // Update is called once per frame
    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount == 0)
        {
            SpawnEnemyWave(waveCount);
        }

    }

    // Generate random spawn position for powerups and enemy balls
    Vector3 GenerateSpawnPosition ()
    {
        float xPos = Random.Range(-spawnRangeX, spawnRangeX);
        float zPos = Random.Range(spawnZMin, spawnZMax);
        return new Vector3(xPos, spawnY, zPos);
    }


    void SpawnEnemyWave(int enemiesToSpawn)
    {
        Vector3 powerupSpawnOffset = new Vector3(0, 0, -15); // make powerups spawn at player end

        // If no powerups remain, spawn a powerup
        if (GameObject.FindGameObjectsWithTag("Powerup").Length == 0) // check that there are zero powerups
        {
            Instantiate(powerupPrefab, GenerateSpawnPosition() + powerupSpawnOffset, powerupPrefab.transform.rotation);
        }

        if (waveCount==2){
            Instantiate(opponentPrefab, GenerateSpawnPosition(), opponentPrefab.transform.rotation);
        }

        // Spawn number of enemy balls based on wave number
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }

        waveCount++;
        ResetPlayerPosition(); // put player back at start
        ResetOpponentPosition ();

    }

    // Move player back to position in front of own goal
    void ResetPlayerPosition ()
    {
        player.transform.position = new Vector3(-2, 41, 25);
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    }
    void ResetOpponentPosition ()
    {
        opponentPrefab.transform.position =  GenerateSpawnPosition();
        opponentPrefab.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        opponentPrefab.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    }

}

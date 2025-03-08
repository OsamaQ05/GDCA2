﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerX : MonoBehaviour
{
    public GameObject enemyPrefab;
    //public GameObject powerupPrefab;
    public GameObject opponentPrefab;
    public GameObject[] powerupPrefabs;
    public GameObject goalkeeperPrefab;

    private float spawnRangeX = 38f;
    private float spawnZMin = -5f; // set min spawn Z
    private float spawnZMax = -35f; // set max spawn Z
    private float spawnY=41f;

    public int enemyCount;
    public int waveCount = 1;
    public int enemySpeed= 25;
    private TimerX timer;
    public int maxWaves=4;
    public static string UItext;



    public GameObject player; 

    void Start(){
        timer= FindObjectOfType<TimerX>();
    }

    // Update is called once per frame
    void Update()
    {
        if (waveCount<=maxWaves)
        { 
            enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (!timer.running){
                TimeUp();
            }

            else if (enemyCount == 0)
            {
                UItext= "Round "+waveCount;
                SpawnEnemyWave(waveCount);
                timer.ResetTimer();
            }
        }
        else if (EnemyX.playerScore>EnemyX.enemyScore){
            // show in UI win 
            UItext= "You Wins!";
        }
        else if (EnemyX.playerScore<EnemyX.enemyScore){
            // show in UI lost 
            UItext="You lose :(";
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
        Vector3 powerupSpawnOffset = new Vector3(-1, 29, 16); // make powerups spawn at player end
        if (GameObject.FindGameObjectsWithTag("Powerup").Length + GameObject.FindGameObjectsWithTag("Smash Powerup").Length==0)
        {
            int randomPowerup = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomPowerup], powerupSpawnOffset, powerupPrefabs[randomPowerup].transform.rotation);
        }

        // If no powerups remain, spawn a powerup
        

        if (waveCount==2){
            Instantiate(opponentPrefab, GenerateSpawnPosition(), opponentPrefab.transform.rotation);
        }
         if (waveCount==1){
            Instantiate(goalkeeperPrefab,new Vector3(0,41,-40), goalkeeperPrefab.transform.rotation);
        }

        // Spawn number of enemy balls based on wave number
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
        Debug.Log("yes");
        waveCount++;
        enemySpeed+=5;
        ResetPlayerPosition(); // put player back at start
        ResetOpponentPosition ();
        ResetGoalkeeperPosition ();

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
    void ResetGoalkeeperPosition ()
    {
        goalkeeperPrefab.transform.position =  new Vector3(0,41,-40);
        goalkeeperPrefab.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        goalkeeperPrefab.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

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

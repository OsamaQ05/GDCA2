using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyX : MonoBehaviour
{
    public float speed=25;
    private Rigidbody enemyRb;
    private GameObject playerGoal;
    private SpawnManagerX x;
    public static int playerScore=0;
    public static int enemyScore=0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerGoal=GameObject.Find("Player Goal");
        x=GameObject.Find("Spawn Manager").GetComponent<SpawnManagerX>();
        speed=x.enemySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Set enemy direction towards player goal and move there
        Vector3 lookDirection = (playerGoal.transform.position - transform.position).normalized;
        enemyRb.AddForce(lookDirection * speed * Time.deltaTime);

    }

    private void OnCollisionEnter(Collision other)
    {
        // If enemy collides with either goal, destroy it
        if (other.gameObject.name == "Enemy Goal")
        {
            Destroy(gameObject);
            playerScore+=PlayerControllerX.scoreMultiplier;
           
        } 
        else if (other.gameObject.name == "Player Goal")
        {
            Destroy(gameObject);
            enemyScore++;
            
        }

    }

}

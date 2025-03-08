using UnityEngine;
using TMPro;

public class ScoreX : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
    {
        scoreText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        
        scoreText.text = EnemyX.playerScore + " : " + EnemyX.enemyScore;
    }
}
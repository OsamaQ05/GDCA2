using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBehaviour : MonoBehaviour
{
    public float time = 0;
    public bool timeIsRunning = true;
    public TMP_Text timeText; 

    void Start()
    {
        timeIsRunning = true;   
    }

    void Update()
    {
        if (timeIsRunning)
        {
            time += Time.deltaTime;
            DisplayTime(time);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

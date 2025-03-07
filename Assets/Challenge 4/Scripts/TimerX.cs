using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimerX : MonoBehaviour


{

    public float timeRemaining=180f;
    public bool running = true;
    public TMP_Text Text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         running = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (running && timeRemaining>0 ){
            timeRemaining-=Time.deltaTime;
            DisplayTime(timeRemaining);}
        else{
            running=false;
            timeRemaining=0;
        }
            
        }
    

    void DisplayTime(float timeToDisplay){
        float mins= Mathf.FloorToInt(timeToDisplay/60);
        float sec = Mathf.FloorToInt(timeToDisplay%60);
        Text.text= string.Format("{0:00}:{1:00}", mins,sec);
    }

    public void ResetTimer(){
        timeRemaining=180f;
        running=true;
    }
}

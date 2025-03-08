using UnityEngine;
using TMPro;

public class PowerupUIX : MonoBehaviour
{
    public TMP_Text powerupText;

    void Start()
    {
        powerupText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (PlayerControllerX.hasPowerup){
            powerupText.text = "Power "+ PlayerControllerX.scoreMultiplier+"X";

        }
        else if (PlayerControllerX.hasSmashPowerup){
            powerupText.text = "Smash "+ PlayerControllerX.scoreMultiplier+"X";

        }
        else if (PlayerControllerX.hasSpeedPowerup){
            powerupText.text = "Speed "+ PlayerControllerX.scoreMultiplier+"X";

        }
        else {
            powerupText.text ="";
        }
       
    }
}
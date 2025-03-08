using UnityEngine;

public class BackToMainMenu : MonoBehaviour
{
    public GameObject mainWindowPanel; // Reference to Main Window
    public GameObject gameplayPanel;   // Reference to Gameplay Panel

    public void GoBack()
    {
        gameplayPanel.SetActive(false); // Hide the Gameplay Panel
        mainWindowPanel.SetActive(true); // Show the Main Window
    }
}

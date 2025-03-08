using UnityEngine;
using QuantumTek.SimpleMenu; // Import namespace

public class GameStart : MonoBehaviour
{
    public SM_OptionList difficultySelector; // Reference to difficulty selector
    public SpawnManagerX spawnManager; // Reference to the spawn manager
    public GameObject mainMenuPrefab; // Reference to the Simple Main Menu prefab
    public GameObject startButtonPrefab; // Reference to the Start Button prefab

    public void StartGame()
    {
        if (difficultySelector == null || spawnManager == null)
        {
            Debug.LogError("Difficulty selector or spawn manager not assigned!");
            return;
        }

        string selectedDifficulty = difficultySelector.currentOption;

        switch (selectedDifficulty)
        {
            case "Easy":
                spawnManager.waveCount = 1;
                spawnManager.enemySpeed = 20;
                break;
            case "Medium":
                spawnManager.waveCount = 2;
                spawnManager.enemySpeed = 30;
                break;
            case "Hard":
                spawnManager.waveCount = 3;
                spawnManager.enemySpeed = 40;
                break;
            default:
                Debug.LogWarning("Unknown difficulty selected: " + selectedDifficulty);
                break;
        }

        Debug.Log("Game started with difficulty: " + selectedDifficulty);

        // Hide the Simple Main Menu prefab
        if (mainMenuPrefab != null)
        {
            mainMenuPrefab.SetActive(false); // Disables the Main Menu
        }
        else
        {
            Debug.LogError("Main Menu prefab is missing in GameStart script!");
        }

        // Hide the Start Button prefab
        if (startButtonPrefab != null)
        {
            startButtonPrefab.SetActive(false); // Disables the Start Button
        }
        else
        {
            Debug.LogError("Start Button prefab is missing in GameStart script!");
        }
    }
}

using UnityEngine;
using TMPro;
using QuantumTek.SimpleMenu;

public class GameStart : MonoBehaviour
{
    public SM_OptionList difficultySelector;
    public SpawnManagerX spawnManager;
    public GameObject mainMenuPrefab;
    public GameObject startButtonPrefab;
    public GameObject background;

    public TMP_Text timer;
    public TMP_Text waves;
    public TMP_Text Score;
    public GameObject pauseMenu;

    private void Start()
    {
        closeAll(); // Automatically hides elements when the game starts
    }

    public void closeAll()
    {
        timer.gameObject.SetActive(false);
        waves.gameObject.SetActive(false);
        Score.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
    }

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
            case "Normal":
            case "Hard":
                Debug.Log("Difficulty selected: " + selectedDifficulty);
                break;
            default:
                Debug.LogWarning("Unknown difficulty selected: " + selectedDifficulty);
                break;
        }

        Debug.Log("Game started with difficulty: " + selectedDifficulty);

        if (mainMenuPrefab != null)
        {
            mainMenuPrefab.SetActive(false);
            background.SetActive(false);
            timer.gameObject.SetActive(true);
            waves.gameObject.SetActive(true);
            Score.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Main Menu prefab is missing in GameStart script!");
        }

        if (startButtonPrefab != null)
        {
            startButtonPrefab.SetActive(false);
            background.SetActive(false);
            timer.gameObject.SetActive(true);
            waves.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Start Button prefab is missing in GameStart script!");
        }
    }
}

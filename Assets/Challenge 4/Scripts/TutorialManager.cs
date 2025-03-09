using UnityEngine;
using TMPro;
using System.Collections;
using QuantumTek.SimpleMenu;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public GameObject powerup;
    public GameObject goal;
    public GameObject menu;
    public SM_OptionList difficultySelector;

    private int tutorialStep = 0;
    private bool tutorialActive = false;
    private bool tutorialStarted = false;

    void Start()
    {
        if (tutorialText != null)
        {
            tutorialText.gameObject.SetActive(false);
            Debug.Log("Tutorial is hidden at start.");
        }
        else
        {
            Debug.LogError("Tutorial text reference is missing!");
        }
    }

    void Update()
    {
        // Check if we should start the tutorial
        if (!tutorialStarted && !tutorialActive && 
            menu != null && !menu.activeSelf && 
            DifficultySelected())
        {
            tutorialStarted = true;
            Debug.Log("Menu is closed and difficulty selected, starting tutorial...");
            StartCoroutine(StartTutorial());
        }

        // Process tutorial steps - Using a serial approach
        if (tutorialActive)
        {
            // Only check for the current active step
            switch (tutorialStep)
            {
                case 1: // WASD Movement
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || 
                        Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                    {
                        Debug.Log("Player moved with WASD!");
                        StartCoroutine(NextStep());
                    }
                    break;

                case 2: // Mouse Movement (only after WASD is completed)
                    if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                    {
                        Debug.Log("Player moved the mouse!");
                        StartCoroutine(NextStep());
                    }
                    break;

                // Other steps are handled by event methods
            }
        }
    }

    IEnumerator StartTutorial()
    {
        tutorialActive = true;
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Tutorial officially started.");
        
        // Start the first step
        tutorialStep = 1;
        UpdateTutorialText();
    }

    void UpdateTutorialText()
    {
        if (tutorialText == null)
        {
            Debug.LogError("Cannot update tutorial text - reference is null!");
            return;
        }

        // Make sure the text is visible
        if (!tutorialText.gameObject.activeSelf)
        {
            tutorialText.gameObject.SetActive(true);
            Debug.Log("Activating tutorial text object.");
        }

        Debug.Log("Setting tutorial text for step: " + tutorialStep);

        switch (tutorialStep)
        {
            case 1:
                tutorialText.text = "Move using WASD";
                Debug.Log("Tutorial Step 1: Move using WASD");
                break;
            case 2:
                tutorialText.text = "Move the camera using the mouse";
                Debug.Log("Tutorial Step 2: Move the camera");
                break;
            case 3:
                tutorialText.text = "Collect the power-up";
                Debug.Log("Tutorial Step 3: Collect the power-up");
                break;
            case 4:
                tutorialText.text = "Score a goal!";
                Debug.Log("Tutorial Step 4: Score a goal");
                break;
            case 5:
                tutorialText.text = "Tutorial Complete! Good luck!";
                Debug.Log("Tutorial Complete!");
                StartCoroutine(HideTutorialAfterDelay(5f));
                break;
            default:
                Debug.LogWarning("Invalid tutorial step: " + tutorialStep);
                break;
        }
    }

    IEnumerator HideTutorialAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (tutorialText != null)
        {
            tutorialText.gameObject.SetActive(false);
            Debug.Log("Tutorial text hidden after completion.");
        }
    }

    IEnumerator NextStep()
    {
        yield return new WaitForSeconds(1.5f); // Grace period before next instruction
        tutorialStep++;
        Debug.Log("Moving to Step: " + tutorialStep);
        UpdateTutorialText();
    }

    public void OnPowerupCollected()
    {
        // Only respond if we're on the correct step
        if (tutorialActive && tutorialStep == 3)
        {
            Debug.Log("Power-up collected!");
            StartCoroutine(NextStep());
        }
    }

    public void OnGoalScored()
    {
        // Only respond if we're on the correct step
        if (tutorialActive && tutorialStep == 4)
        {
            Debug.Log("Goal Scored!");
            StartCoroutine(NextStep());
        }
    }

    bool DifficultySelected()
    {
        // Check specifically for "Tutorial" difficulty
        return difficultySelector != null && 
               difficultySelector.currentOption != null && 
               difficultySelector.currentOption.ToLower() == "tutorial";
    }
}
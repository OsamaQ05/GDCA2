using UnityEngine;
using UnityEditor;
public class PauseMenu : MonoBehaviour
{
    private TimerX timer;

    private void Start()
    {
        timer = FindObjectOfType<TimerX>(); // Locate TimerX in the scene
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // Resume physics updates
        gameObject.SetActive(false); // Hide pause menu

        // Resume the timer
        if (timer != null)
            timer.running = true;

        // Resume player & camera movement
        FindObjectOfType<PlayerControllerX>().enabled = true;
        Camera.main.GetComponent<CameraController>().enabled = true;

        // Resume all tagged prefabs
        ResumePrefabsByTag("Goalkeeper");
        ResumePrefabsByTag("Opponent");
        ResumePrefabsByTag("Enemy");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode(); // Exits Play Mode in the editor
        #else
            Application.Quit(); // Works in standalone build
        #endif
    }

    private void ResumePrefabsByTag(string tag)
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject prefab in prefabs)
        {
            MovingPrefab movementScript = prefab.GetComponent<MovingPrefab>();
            if (movementScript != null)
            {
                movementScript.Resume();
            }
        }
    }
}

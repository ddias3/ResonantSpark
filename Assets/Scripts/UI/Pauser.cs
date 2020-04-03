using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


public class Pauser : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;

    public void Awake()
    {
        EventManager.StartListening<ResonantSpark.Events.TogglePauseGame>(new UnityAction(TogglePause));
    }

    

    public void TogglePause()
    {
        if (!isPaused)
        {
            Debug.Log("is this thing on");
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
        else {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
        
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("TestMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

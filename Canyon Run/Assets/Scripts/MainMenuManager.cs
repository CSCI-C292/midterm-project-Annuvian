using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Loads the mission briefing screen
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    // Exits the application completely
    public void EndGame()
    {
        Application.Quit();
    }
}
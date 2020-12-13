using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    // Restarts the mission
    public void RestartGame()
    {
        SceneManager.LoadScene(2);
    }

    // Exits the game
    public void ExitGame()
    {
        Application.Quit();
    }
}
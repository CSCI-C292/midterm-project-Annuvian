using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // References
    // UI
    [Header("UI")]
    [SerializeField] TextMeshProUGUI endGameMessage;
    [SerializeField] Button exitButton;
    [SerializeField] Button restartButton;
    // Game Control
    [Header("Game Control")]
    [SerializeField] PlayerController playerController;

    private void Start()
    {
        // Unpauses the game
        Time.timeScale = 1;

        // Resumes all audio
        AudioListener.pause = false;

        // Hides end game UI objects
        exitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        endGameMessage.enabled = false;
    }

    // Triggers end game after crashing
    public void Crashed()
    {
        endGameMessage.text = "You have crashed! Mission failed.";
        DisplayEndScreen();
    }

    // Triggers end game after being killed
    public void Killed()
    {
        endGameMessage.text = "You have been shot down! Mission failed.";
        DisplayEndScreen();
    }

    // Triggers end game after running out of ammo
    public void OutOfAmmo()
    {
        endGameMessage.text = "You have ran out of all your ammo! Mission failed.";
        DisplayEndScreen();
    }

    // Triggers end game when all targets are destroyed
    public void Win()
    {
        endGameMessage.text = "Congratulations, you have destroyed all your targets. Demo completed.";
        DisplayEndScreen();
    }

    // Triggers end game when player travels out of bounds
    public void OutOfBoundsTooLong()
    {
        endGameMessage.text = "You flew outside of the canyon for too long! Try to stay beneath the top edges of the canyon!";
        DisplayEndScreen();
    }

    // Displays the end game UI and pauses all audio and motion
    public void DisplayEndScreen()
    {
        AudioListener.pause = true;
        exitButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        endGameMessage.enabled = true;
        Time.timeScale = 0;
    }
}
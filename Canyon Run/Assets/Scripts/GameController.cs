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
        endGameMessage.text = "Mission failed. You have crashed! Keep an eye on your altitude and make sure you steer clear from the ground and trees!";
        DisplayEndScreen();
    }

    // Triggers end game after being killed
    public void Killed()
    {
        endGameMessage.text = "Mission Failed. You have been shot down by a surface-to-air missile. Remember to release a few flares 'C' when you hear the SAM warning alarm! You start with 10 flares and they burn for 10 seconds each. Try dropping them at 2 or 3 second intervals.";
        DisplayEndScreen();
    }

    // Triggers end game after running out of ammo
    public void OutOfAmmo()
    {
        endGameMessage.text = "Mission Failed. You have ran out of all your ammo! You only have 4 missiles to destroy all 3 targets. Keep in mind the missiles will not always hit your target. " +
            "For the best chance of a hit, release your missiles from further away (but not too far), from a higher altitude, and with a clear straight line of sight to your target.";
        DisplayEndScreen();
    }

    // Triggers end game when all targets are destroyed
    public void Win()
    {
        endGameMessage.text = "Mission complete. Congratulations, you have destroyed all your targets. Demo completed. Thank you for playing.";
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
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI endGameMessage;
    [SerializeField] Button exitButton;
    [SerializeField] Button restartButton;
    [SerializeField] PlayerController playerController;

    private void Start()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        exitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        endGameMessage.enabled = false;
    }

    public void Crashed()
    {
        endGameMessage.text = "You have crashed! Mission failed.";
        DisplayEndScreen();
    }

    public void Killed()
    {
        endGameMessage.text = "You have been shot down! Mission failed.";
        DisplayEndScreen();
    }

    public void OutOfAmmo()
    {
        endGameMessage.text = "You have ran out of all your ammo! Mission failed.";
        DisplayEndScreen();
    }

    public void Win()
    {
        endGameMessage.text = "Congratulations, you have destroyed all your targets. Demo completed.";
        DisplayEndScreen();
    }

    public void OutOfBoundsTooLong()
    {
        endGameMessage.text = "You flew outside of the canyon for too long! Try to stay beneath the top edges of the canyon!";
        DisplayEndScreen();
    }

    public void DisplayEndScreen()
    {
        AudioListener.pause = true;
        exitButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        endGameMessage.enabled = true;
        Time.timeScale = 0;
    }
}
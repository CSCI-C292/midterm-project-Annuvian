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
        exitButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        endGameMessage.enabled = false;
    }

    public void Crashed()
    {
        endGameMessage.text = "You have crashed!";
        DisplayEndScreen();
    }

    public void Killed()
    {
        endGameMessage.text = "You have been shot down!";
        DisplayEndScreen();
    }

    public void Win()
    {
        int hour = playerController.runHour;
        int minute = playerController.runMinutes;
        int second = playerController.runSeconds;
        string totalTime = hour.ToString() + ":" + minute.ToString() + ":" + second.ToString();
        endGameMessage.text = "Congratulations, you completed the course with a time of " + totalTime + "!";
        DisplayEndScreen();
    }

    public void OutOfBoundsTooLong()
    {
        endGameMessage.text = "You flew outside of the canyon for too long! Try to stay beneath the top edges of the canyon!";
        DisplayEndScreen();
    }

    public void DisplayEndScreen()
    {
        exitButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        endGameMessage.enabled = true;
        Time.timeScale = 0;
    }
}
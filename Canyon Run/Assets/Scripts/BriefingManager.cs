using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BriefingManager : MonoBehaviour
{
    // Fields
    string missionBriefing = "Intel reports that the enemy is preparing for a large scale ground assault on the nearby city of Batumi within the next couple weeks. " +
        "Our recon teams have informed us that a high ranking officer that will play a major role in the assault is briefing some of his soldiers that have also been considered to be high value targets. " +
        "They are meeting in a small camp at the base of some mountains not far from the coast. " +
        "You are tasked with destroying any vehicles you see around the camp to ensure none of the targets will be able to escape. " +
        "Intel also reports there are enemy surface-to-air missile launchers defending the camp. " +
        "You must also destroy these so that our ground forces can safely infiltrate and exfiltrate after they have captured and secured the targets. " +
        "You are equipped with 4 AGM-65F infrared surface-to-air missiles. " +
        "Intelligence reports indicate two vehicles that must be destroyed inside the camp, as well as one SAM launcher. " +
        "Make every shot count as rearming will not be possible during this operation. " +
        "Running out of ammunition before the three targets are destroyed will be considered a failure of the mission objectives. ";
    [SerializeField] float typingSpeed;

    // References
    [SerializeField] TextMeshProUGUI briefText;

    void Start()
    {
        // Begins typing out the mission brief text
        StartCoroutine(TypeText());
    }

    // Update is called once per frame
    void Update()
    {
        // If player clicks the left mouse button the text immediately all appears
        if (Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
            briefText.text = missionBriefing;
        }
    }

    // Starts mission
    public void BegingMission()
    {
        SceneManager.LoadScene(2);
    }

    // Returns to main menu
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    // Method to type out all the text one letter at a time
    IEnumerator TypeText()
    {
        for (int i = 0; i < missionBriefing.Length; i++)
        {
            briefText.text += missionBriefing[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
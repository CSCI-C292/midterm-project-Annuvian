using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GameController gameController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (this.gameObject == player.currentWaypoint.gameObject)
            {
                if (gameObject.name != "Finish")
                {
                    player.CycleToNextWaypoint();
                }
                else
                {
                    gameController.Win();
                }
                
            }
        }
    }
}
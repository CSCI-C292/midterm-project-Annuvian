using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiteningPod : MonoBehaviour
{
    // References
    [SerializeField] PlayerController player;
    Vector3 target;

    private void Update()
    {
        // As long as the player's target isn't null, camera faces the target of the player
        if (player.target != null)
        {
            transform.LookAt(player.target.transform.position);
        }
    }
}
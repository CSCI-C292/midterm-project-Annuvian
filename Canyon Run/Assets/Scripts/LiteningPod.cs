using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiteningPod : MonoBehaviour
{
    [SerializeField] PlayerController player;
    Vector3 target;
    private void Update()
    {
        if (player.target != null)
        {
            transform.LookAt(player.target.transform.position);
        }
    }
}
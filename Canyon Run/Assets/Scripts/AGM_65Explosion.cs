using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGM_65Explosion : MonoBehaviour
{
    // Fields
    float lifeTime = 5;

    private void OnCollisionStay(Collision collision)
    {
        // If explosion hits something that isn't terrain, it destoys it
        if (collision.gameObject.tag != "Terrain")
        {
            Destroy(collision.gameObject);
        }
    }

    private void Update()
    {
        // Explosion lasts for its lifetime and is then destroyed
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }
}
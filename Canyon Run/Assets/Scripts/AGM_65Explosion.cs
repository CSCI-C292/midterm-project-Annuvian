using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGM_65Explosion : MonoBehaviour
{
    float lifeTime = 5;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "Terrain")
        {
            Destroy(collision.gameObject);
        }
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }
}

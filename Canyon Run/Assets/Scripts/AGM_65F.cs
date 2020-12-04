using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGM_65F : MonoBehaviour
{
    PlayerController player;
    GameObject target;
    Vector3 targetPosition;
    bool hasLaunched = false;
    float primaryThrust = 44557.836f;
    float secondaryThrust = 9674.882f;
    float thrustTime1 = 0.575f;
    float thrustTime2 = 3.495f;
    Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (hasLaunched)
        {
            if (thrustTime1 > 0)
            {
                thrustTime1 -= Time.deltaTime;
            }
            else if (thrustTime2 > 0)
            {
                thrustTime2 -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (hasLaunched)
        {
            transform.LookAt(targetPosition);
            if (thrustTime1 > 0)
            {
                rb.AddForce(transform.forward * primaryThrust);
            }
            else if (thrustTime2 > 0)
            {
                rb.AddForce(transform.forward * secondaryThrust);
            }
        }
    }

    public void Launch(GameObject target)
    {
        hasLaunched = true;
        transform.SetParent(null, true);
        this.target = target;
        targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Explosion stuff goes here
        Destroy(gameObject);
    }
}
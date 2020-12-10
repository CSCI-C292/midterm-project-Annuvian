using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_75 : MonoBehaviour
{
    GameObject target;
    [SerializeField] GameController gameController;
    bool hasLaunched = false;
    float primaryThrust = 44557.836f;
    float secondaryThrust = 9674.882f;
    float thrustTime1 = 0.575f;
    float thrustTime2 = 3.495f;
    Rigidbody rb;
    [SerializeField] AudioSource samWarning;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, target.transform.position) <= 2000 && !hasLaunched)
        {
            Launch();
        }

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
            transform.LookAt(target.transform.position);
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

    public void Launch()
    {
        samWarning.Play();
        hasLaunched = true;
        transform.SetParent(null, true);
        transform.LookAt(target.transform.position);
        rb.AddForce(transform.forward * 308, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameController.Killed();
        }
    }
}
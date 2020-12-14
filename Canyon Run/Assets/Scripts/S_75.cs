using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_75 : MonoBehaviour
{
    // Fields
    // Engine
    bool hasLaunched = false;
    float lifeTime = 20f;
    float primaryThrust = 44557.836f;
    float secondaryThrust = 9674.882f;
    float thrustTime1 = 0.575f;
    float thrustTime2 = 10.495f;
    float speed = 1200f;

    // References
    Rigidbody rb;
    // Targeting
    GameObject target;
    // Game Control
    [SerializeField] GameController gameController;
    // Audio
    [SerializeField] AudioSource samWarning;

    private void Start()
    {
        // Searches for player and sets them as the target
        target = GameObject.FindGameObjectWithTag("Player");

        // Accesses the Rigidbody component of this game object
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Launches the SAM if the player is within 5,000 meters
        if (Vector3.Distance(transform.position, target.transform.position) <= 7500 && !hasLaunched)
        {
            Launch();
        }

        // Updates after launch
        if (hasLaunched)
        {
            if (GameObject.FindGameObjectWithTag("Flare") != null)
            {
                target = GameObject.FindGameObjectWithTag("Flare");
            }
            else
            {
                target = GameObject.FindGameObjectWithTag("Player");
            }

            // Destroys missile when lifetime is over
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0)
            {
                Destroy(gameObject);
            }

            // Decreases thrust times for each stage of the engine
            if (thrustTime1 > 0)
            {
                thrustTime1 -= Time.deltaTime;
            }
            else if (thrustTime2 > 0)
            {
                thrustTime2 -= Time.deltaTime;
            }

            // Missile looks at player and moves towards them
            transform.LookAt(target.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        
    }

    /*
    private void FixedUpdate()
    {
        if (hasLaunched)
        {
            // SAM faces player
            transform.LookAt(target.transform.position);

            // If the first stage of the engine is lit, provides appropriate thrust in the forward direction
            if (thrustTime1 > 0)
            {
                rb.AddForce(transform.forward * primaryThrust);
            }
            // Switches over to second stage thrust when 2nd stage of the engine is lit
            else if (thrustTime2 > 0)
            {
                rb.AddForce(transform.forward * secondaryThrust);
            }
        }
    }
    */

    // Launches the SAM
    public void Launch()
    {
        // Starts the SAM warning audio alert
        samWarning.Play();

        hasLaunched = true;

        // Removes the SAM as child of the launcher
        transform.SetParent(null, true);

        // Face the player
        transform.LookAt(target.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If it collides with the player, the player is killed
        if (collision.gameObject.tag == "Player")
        {
            gameController.Killed();
        }
        else if (collision.gameObject.tag == "Flare")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}